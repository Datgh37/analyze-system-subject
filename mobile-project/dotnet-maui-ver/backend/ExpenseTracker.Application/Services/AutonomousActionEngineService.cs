using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Autonomous;
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of Server-Driven Action Protocol & Jev-inspired Probabilistic Decision Gating (Chặng 10.5 / PLAN-47)
public class AutonomousActionEngineService : IAutonomousActionEngineService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITransactionService _transactionService;
    private readonly IFinancialRealtimeNotifier? _realtimeNotifier;
    private readonly IAutoCategorizationService? _autoRuleService;

    // Thread-safe in-memory inbox of transient pending actions per user
    private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, AutonomousActionDto>> _userActionInboxes = new();

    public AutonomousActionEngineService(
        IApplicationDbContext dbContext,
        ITransactionService transactionService,
        IFinancialRealtimeNotifier? realtimeNotifier = null,
        IAutoCategorizationService? autoRuleService = null)
    {
        _dbContext = dbContext;
        _transactionService = transactionService;
        _realtimeNotifier = realtimeNotifier;
        _autoRuleService = autoRuleService;
    }

    public Task<Result<AutonomousActionInboxResponse>> GetPendingActionsAsync(Guid userId, CancellationToken ct = default)
    {
        var inbox = GetUserInbox(userId);
        var actions = inbox.Values
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        var response = new AutonomousActionInboxResponse(actions.Count, actions);
        return Task.FromResult(Result<AutonomousActionInboxResponse>.Success(response));
    }

    public async Task<Result<AutonomousActionDto?>> ProcessRawBankNotificationAsync(
        Guid userId,
        RawBankNotificationRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return Result<AutonomousActionDto?>.Failure("Nội dung thông báo trống.");

        // 1. Resolve Bank / E-Wallet Name
        string bankName = ResolveBankName(request.PackageName, request.Title);

        // 2. Extract Monetary Amount
        decimal amount = 0m;
        if (request.DetectedAmount.HasValue && request.DetectedAmount.Value > 0)
        {
            amount = request.DetectedAmount.Value;
        }
        else
        {
            amount = ExtractAmountFromText(request.Message);
        }

        if (amount <= 0m)
        {
            return Result<AutonomousActionDto?>.Failure("Không trích xuất được số tiền hợp lệ từ thông báo ngân hàng.");
        }

        // 3. Jev-inspired Probabilistic Categorization & Confidence Evaluation
        var (categoryName, confidence, badgeColor, choices) = EvaluateCategoryProbability(request.Message);

        string actionId = $"act_{Guid.NewGuid():N}";
        DateTime now = request.NotificationTime ?? DateTime.UtcNow;
        string cleanTitle = ExtractCleanMerchantTitle(request.Message, amount);
        string subtitle = $"{bankName} • {now:HH:mm}";

        AutonomousActionDto actionDto;

        // BRANCH A: HIGH CONFIDENCE (>= 0.85) -> 1-Tap Quick Commit Button
        if (confidence >= 0.85)
        {
            actionDto = new AutonomousActionDto(
                ActionId: actionId,
                ActionType: AutonomousActionTypes.QuickTransactionConfirm,
                ConfidenceScore: confidence,
                Display: new ActionDisplayDto(
                    Title: cleanTitle,
                    Subtitle: subtitle,
                    BadgeText: categoryName,
                    BadgeColor: badgeColor,
                    Amount: amount
                ),
                PrimaryAction: new ActionCommandDto(
                    Label: "Lưu nhanh",
                    ActionKind: ActionKinds.QuickCommit,
                    Endpoint: "/api/actions/quick-commit",
                    Payload: new Dictionary<string, string>
                    {
                        ["actionId"] = actionId,
                        ["amount"] = amount.ToString("G29"),
                        ["categoryName"] = categoryName,
                        ["walletName"] = bankName,
                        ["note"] = cleanTitle
                    }
                ),
                SecondaryAction: new ActionCommandDto(
                    Label: "Đổi",
                    ActionKind: ActionKinds.Navigate,
                    Route: $"AddTransactionPage?amount={amount:G29}&category={Uri.EscapeDataString(categoryName)}&source={Uri.EscapeDataString(bankName)}"
                ),
                Choices: null,
                CreatedAt: now
            );
        }
        // BRANCH B: MEDIUM CONFIDENCE (0.50 <= Confidence < 0.85) -> Jev Choice Disambiguation (Peer Options)
        else if (confidence >= 0.50)
        {
            actionDto = new AutonomousActionDto(
                ActionId: actionId,
                ActionType: AutonomousActionTypes.ChoiceDisambiguation,
                ConfidenceScore: confidence,
                Display: new ActionDisplayDto(
                    Title: cleanTitle,
                    Subtitle: subtitle,
                    BadgeText: $"{categoryName} ({choices?.FirstOrDefault()?.FormattedProbability ?? "60%"})",
                    BadgeColor: "#3B82F6",
                    Amount: amount
                ),
                PrimaryAction: null, // No single-click assumption, let user choose from peer choices
                SecondaryAction: new ActionCommandDto(
                    Label: "Sửa chi tiết",
                    ActionKind: ActionKinds.Navigate,
                    Route: $"AddTransactionPage?amount={amount:G29}&source={Uri.EscapeDataString(bankName)}"
                ),
                Choices: choices,
                CreatedAt: now
            );
        }
        // BRANCH C: LOW CONFIDENCE (< 0.50) -> Silent Non-intrusive Unclassified Notice
        else
        {
            actionDto = new AutonomousActionDto(
                ActionId: actionId,
                ActionType: AutonomousActionTypes.UnclassifiedNotice,
                ConfidenceScore: confidence,
                Display: new ActionDisplayDto(
                    Title: $"Khoản chi {amount:N0} đ",
                    Subtitle: $"{bankName} • {now:HH:mm}",
                    BadgeText: "Chờ phân loại",
                    BadgeColor: "#6B7280",
                    Amount: amount
                ),
                PrimaryAction: null,
                SecondaryAction: new ActionCommandDto(
                    Label: "Phân loại ngay",
                    ActionKind: ActionKinds.Navigate,
                    Route: $"AddTransactionPage?amount={amount:G29}&source={Uri.EscapeDataString(bankName)}"
                ),
                Choices: null,
                CreatedAt: now
            );
        }

        // Store into transient in-memory inbox
        var inbox = GetUserInbox(userId);
        inbox[actionId] = actionDto;

        // Non-intrusive realtime push if client is connected
        if (_realtimeNotifier != null)
        {
            try
            {
                await _realtimeNotifier.NotifyInAppAsync(userId, new InAppNotificationDto(
                    Id: Guid.NewGuid(),
                    Title: actionDto.Display.Title,
                    Message: $"{actionDto.Display.Subtitle} - Gợi ý: {actionDto.Display.BadgeText}",
                    Type: "AUTONOMOUS_ACTION",
                    PayloadJson: actionId,
                    IsRead: false,
                    ReadAt: null,
                    CreatedAt: now
                ), ct);
            }
            catch
            {
                // Non-blocking resilience
            }
        }

        return Result<AutonomousActionDto?>.Success(actionDto);
    }

    public async Task<Result<AutonomousActionDto>> CommitQuickActionAsync(
        Guid userId,
        QuickCommitActionRequest request,
        CancellationToken ct = default)
    {
        var inbox = GetUserInbox(userId);
        inbox.TryGetValue(request.ActionId, out var action);

        decimal amount = request.Amount ?? action?.Display.Amount ?? 0m;
        if (amount <= 0m)
            return Result<AutonomousActionDto>.Failure("Số tiền giao dịch không hợp lệ.");

        string categoryName = request.CategoryName ?? action?.Display.BadgeText ?? "Chi tiêu";
        string walletName = request.WalletName ?? "Ví Mặc Định";
        string note = request.Note ?? action?.Display.Title ?? "[Tự động] Ghi nhận từ thông báo ngân hàng";

        // 1. Resolve User Wallet
        var wallets = await _dbContext.Wallets
            .Where(w => w.UserId == userId && !w.IsDeleted)
            .ToListAsync(ct);

        var wallet = wallets.FirstOrDefault(w => w.Name.Contains(walletName, StringComparison.OrdinalIgnoreCase))
            ?? wallets.FirstOrDefault();

        if (wallet == null)
            return Result<AutonomousActionDto>.Failure("Không tìm thấy ví thanh toán hợp lệ của người dùng.");

        // 2. Resolve Category
        var categories = await _dbContext.Categories
            .Where(c => (c.UserId == userId || c.IsSystem) && !c.IsDeleted && c.TransactionType == TransactionType.Expense)
            .ToListAsync(ct);

        var category = categories.FirstOrDefault(c => c.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase))
            ?? categories.FirstOrDefault();

        if (category == null)
            return Result<AutonomousActionDto>.Failure("Không tìm thấy danh mục chi tiêu hợp lệ.");

        // 3. Record Expense Transaction via TransactionService (enforces ACID & Wallet Balance Deductions)
        var createRequest = new CreateExpenseTransactionRequest(
            WalletId: wallet.Id,
            CategoryId: category.Id,
            Amount: amount,
            TransactionDate: DateTime.UtcNow,
            Note: note,
            IsEssential: false,
            IsRecurring: false,
            ExcludeFromReport: false
        );

        var txResult = await _transactionService.CreateExpenseAsync(userId, createRequest, ct);
        if (txResult.IsFailure)
            return Result<AutonomousActionDto>.Failure($"Không thể lưu giao dịch: {txResult.Error}");

        // 4. Remove from Pending Inbox upon successful commit
        inbox.TryRemove(request.ActionId, out _);

        // 5. Build committed response action
        var committedAction = action != null
            ? action with
            {
                ActionType = "COMMITTED",
                Display = action.Display with { Subtitle = $"Đã lưu vào ví {wallet.Name} • Vừa xong" },
                PrimaryAction = null,
                SecondaryAction = null,
                Choices = null
            }
            : new AutonomousActionDto(
                ActionId: request.ActionId,
                ActionType: "COMMITTED",
                ConfidenceScore: 1.0,
                Display: new ActionDisplayDto(note, $"Đã lưu vào ví {wallet.Name} • Vừa xong", category.Name, "#10B981", amount),
                PrimaryAction: null,
                SecondaryAction: null,
                Choices: null,
                CreatedAt: DateTime.UtcNow
            );

        return Result<AutonomousActionDto>.Success(committedAction);
    }

    public Task<Result<bool>> DismissActionAsync(Guid userId, string actionId, CancellationToken ct = default)
    {
        var inbox = GetUserInbox(userId);
        bool removed = inbox.TryRemove(actionId, out _);
        return Task.FromResult(Result<bool>.Success(removed));
    }

    // --- Helper Methods ---

    private static ConcurrentDictionary<string, AutonomousActionDto> GetUserInbox(Guid userId)
    {
        return _userActionInboxes.GetOrAdd(userId, _ => new ConcurrentDictionary<string, AutonomousActionDto>());
    }

    private static string ResolveBankName(string packageName, string title)
    {
        if (packageName.Contains("acb", StringComparison.OrdinalIgnoreCase)) return "ACB";
        if (packageName.Contains("momo", StringComparison.OrdinalIgnoreCase)) return "MoMo";
        if (packageName.Contains("zalopay", StringComparison.OrdinalIgnoreCase)) return "ZaloPay";
        if (packageName.Contains("vcb", StringComparison.OrdinalIgnoreCase)) return "Vietcombank";
        if (packageName.Contains("mbmobile", StringComparison.OrdinalIgnoreCase)) return "MBBank";
        if (packageName.Contains("techcombank", StringComparison.OrdinalIgnoreCase)) return "Techcombank";
        if (packageName.Contains("bidv", StringComparison.OrdinalIgnoreCase)) return "BIDV";
        if (packageName.Contains("vpbank", StringComparison.OrdinalIgnoreCase)) return "VPBank";
        if (packageName.Contains("tpb", StringComparison.OrdinalIgnoreCase) || packageName.Contains("tpbank", StringComparison.OrdinalIgnoreCase)) return "TPBank";

        if (!string.IsNullOrWhiteSpace(title))
        {
            if (title.Contains("ACB", StringComparison.OrdinalIgnoreCase)) return "ACB";
            if (title.Contains("MoMo", StringComparison.OrdinalIgnoreCase)) return "MoMo";
            if (title.Contains("ZaloPay", StringComparison.OrdinalIgnoreCase)) return "ZaloPay";
            if (title.Contains("Vietcombank", StringComparison.OrdinalIgnoreCase)) return "Vietcombank";
            if (title.Contains("MB", StringComparison.OrdinalIgnoreCase)) return "MBBank";
            if (title.Contains("Techcombank", StringComparison.OrdinalIgnoreCase)) return "Techcombank";
            if (title.Contains("BIDV", StringComparison.OrdinalIgnoreCase)) return "BIDV";
            if (title.Contains("VPBank", StringComparison.OrdinalIgnoreCase)) return "VPBank";
            if (title.Contains("TPBank", StringComparison.OrdinalIgnoreCase)) return "TPBank";
        }

        return "Ngân hàng";
    }

    private static decimal ExtractAmountFromText(string message)
    {
        // Matches e.g. "-185,000", "-50.000d", "185.000VND", "GD: -45000", "tt 35,000"
        var match = Regex.Match(message, @"(?:-|\bchi\b|\btt\b|\bthanh toan\b)?\s*([0-9]{1,3}(?:[.,][0-9]{3})+)\s*(?:vnd|vnđ|d|đ)?", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            string rawNum = match.Groups[1].Value.Replace(".", "").Replace(",", "");
            if (decimal.TryParse(rawNum, out decimal val) && val > 0)
                return val;
        }

        // Match shorthand e.g. "50k", "185k"
        var shortMatch = Regex.Match(message, @"\b([0-9]+)\s*k\b", RegexOptions.IgnoreCase);
        if (shortMatch.Success && decimal.TryParse(shortMatch.Groups[1].Value, out decimal shortVal))
        {
            return shortVal * 1000m;
        }

        return 0m;
    }

    private static string ExtractCleanMerchantTitle(string message, decimal amount)
    {
        string text = message;
        if (text.Contains("Shopee", StringComparison.OrdinalIgnoreCase)) return $"Shopee: {amount:N0} đ";
        if (text.Contains("Lazada", StringComparison.OrdinalIgnoreCase)) return $"Lazada: {amount:N0} đ";
        if (text.Contains("GrabFood", StringComparison.OrdinalIgnoreCase)) return $"GrabFood: {amount:N0} đ";
        if (text.Contains("ShopeeFood", StringComparison.OrdinalIgnoreCase)) return $"ShopeeFood: {amount:N0} đ";
        if (text.Contains("WinMart", StringComparison.OrdinalIgnoreCase)) return $"WinMart: {amount:N0} đ";
        if (text.Contains("Co.opmart", StringComparison.OrdinalIgnoreCase)) return $"Co.opmart: {amount:N0} đ";
        if (text.Contains("Circle K", StringComparison.OrdinalIgnoreCase)) return $"Circle K: {amount:N0} đ";
        if (text.Contains("Highlands", StringComparison.OrdinalIgnoreCase)) return $"Highlands Coffee: {amount:N0} đ";
        if (text.Contains("Phuc Long", StringComparison.OrdinalIgnoreCase) || text.Contains("Phúc Long", StringComparison.OrdinalIgnoreCase)) return $"Phúc Long: {amount:N0} đ";
        if (text.Contains("CGV", StringComparison.OrdinalIgnoreCase)) return $"CGV Cinemas: {amount:N0} đ";
        if (text.Contains("Netflix", StringComparison.OrdinalIgnoreCase)) return $"Netflix: {amount:N0} đ";
        if (text.Contains("Spotify", StringComparison.OrdinalIgnoreCase)) return $"Spotify: {amount:N0} đ";
        if (text.Contains("iCloud", StringComparison.OrdinalIgnoreCase)) return $"iCloud: {amount:N0} đ";

        return $"Chi tiêu: {amount:N0} đ";
    }

    private static (string CategoryName, double Confidence, string BadgeColor, List<ActionChoiceDto>? Choices) EvaluateCategoryProbability(string message)
    {
        string text = message.ToLowerInvariant();

        // 1. High Confidence Matches (Confidence >= 0.85)
        if (text.Contains("shopee") || text.Contains("lazada") || text.Contains("tiki") || text.Contains("tiktok shop"))
            return ("Mua sắm", 0.92, "#F59E0B", null);

        if (text.Contains("grabfood") || text.Contains("shopeefood") || text.Contains("baemin") ||
            text.Contains("highlands") || text.Contains("starbucks") || text.Contains("phuc long") || text.Contains("phúc long") ||
            text.Contains("phở") || text.Contains("cơm") ||
            text.Contains("trà sữa") || text.Contains("cà phê") || text.Contains("cafe"))
            return ("Ăn uống", 0.95, "#10B981", null);

        if (text.Contains("cgv") || text.Contains("bhd") || text.Contains("lotte cinema") || text.Contains("galaxy cinema") || text.Contains("cinema") || text.Contains("rạp chiếu phim"))
            return ("Giải trí", 0.90, "#8B5CF6", null);

        if (text.Contains("winmart") || text.Contains("co.opmart") || text.Contains("bách hóa xanh") || text.Contains("siêu thị"))
            return ("Đi chợ / Thực phẩm", 0.94, "#059669", null);

        if (text.Contains("grab") || text.Contains("be") || text.Contains("xăng") || text.Contains("petrolimex") || text.Contains("vé xe"))
            return ("Đi lại", 0.91, "#3B82F6", null);

        if (text.Contains("netflix") || text.Contains("spotify") || text.Contains("icloud") || text.Contains("youtube premium"))
            return ("Bẫy thuê bao", 0.96, "#DC2626", null);

        if (text.Contains("evn") || text.Contains("điện lực") || text.Contains("cấp nước") || text.Contains("internet") ||
            text.Contains("viettel") || text.Contains("vnpt") || text.Contains("fpt"))
            return ("Hóa đơn & Tiện ích", 0.95, "#6366F1", null);

        // 2. Medium Confidence Matches (0.50 <= Confidence < 0.85) -> Jev Choice Disambiguation
        if (text.Contains("circle k") || text.Contains("7-eleven") || text.Contains("gs25") || text.Contains("familymart"))
        {
            var choices = new List<ActionChoiceDto>
            {
                new("c1", "Ăn uống", 0.60, "60%", new Dictionary<string, string> { ["categoryName"] = "Ăn uống" }),
                new("c2", "Vật dụng sinh hoạt", 0.40, "40%", new Dictionary<string, string> { ["categoryName"] = "Vật dụng sinh hoạt" })
            };
            return ("Cửa hàng tiện lợi", 0.65, "#3B82F6", choices);
        }

        if (text.Contains("pharmacity") || text.Contains("long châu") || text.Contains("nhà thuốc") || text.Contains("thuốc"))
        {
            var choices = new List<ActionChoiceDto>
            {
                new("c1", "Sức khỏe", 0.75, "75%", new Dictionary<string, string> { ["categoryName"] = "Sức khỏe" }),
                new("c2", "Chăm sóc cá nhân", 0.25, "25%", new Dictionary<string, string> { ["categoryName"] = "Chăm sóc cá nhân" })
            };
            return ("Nhà thuốc", 0.75, "#10B981", choices);
        }

        if (text.Contains("cgv") || text.Contains("bhd") || text.Contains("lotte cinema") || text.Contains("rạp chiếu phim"))
        {
            var choices = new List<ActionChoiceDto>
            {
                new("c1", "Giải trí", 0.80, "80%", new Dictionary<string, string> { ["categoryName"] = "Giải trí" }),
                new("c2", "Bạn bè / Xã hội", 0.20, "20%", new Dictionary<string, string> { ["categoryName"] = "Bạn bè / Xã hội" })
            };
            return ("Xem phim", 0.80, "#8B5CF6", choices);
        }

        // 3. Low Confidence (< 0.50) -> Silent notice, unclassified
        return ("Chi tiêu khác", 0.40, "#6B7280", null);
    }
}
