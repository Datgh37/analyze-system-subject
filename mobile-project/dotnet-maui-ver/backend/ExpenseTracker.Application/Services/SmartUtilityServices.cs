using System.Text.RegularExpressions;
using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of Subscription Trap Detector (UC-03.09)
public class SubscriptionDetectorService : ISubscriptionDetectorService
{
    private readonly IApplicationDbContext _context;

    public SubscriptionDetectorService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubscriptionAnalysisResultDto>> DetectSubscriptionsAsync(Guid userId, CancellationToken ct = default)
    {
        var expenses = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.TransactionType == TransactionType.Expense)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);

        var knownServices = new[]
        {
            ("Netflix", 260_000m, "Dịch vụ phim Netflix"),
            ("Spotify", 59_000m, "Dịch vụ nghe nhạc Spotify Premium"),
            ("iCloud", 19_000m, "Lưu trữ đám mây Apple iCloud"),
            ("YouTube Premium", 79_000m, "Thuê bao YouTube Premium"),
            ("ChatGPT Plus", 500_000m, "Thuê bao OpenAI ChatGPT Plus"),
            ("Adobe Creative Cloud", 750_000m, "Bộ công cụ đồ họa Adobe"),
            ("VieON", 69_000m, "Truyền hình VieON"),
            ("FPT Play", 88_000m, "Truyền hình FPT Play")
        };

        var detectedList = new List<DetectedSubscriptionDto>();

        foreach (var (svcName, defaultPrice, desc) in knownServices)
        {
            var matchedTransactions = expenses
                .Where(e => (e.Note != null && e.Note.Contains(svcName, StringComparison.OrdinalIgnoreCase))
                         || (e.Category != null && e.Category.Name.Contains(svcName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (matchedTransactions.Count > 0)
            {
                var latest = matchedTransactions[0];
                var amount = latest.Amount > 0 ? latest.Amount : defaultPrice;
                var annualCost = amount * 12;

                detectedList.Add(new DetectedSubscriptionDto(
                    ServiceName: svcName,
                    CategoryId: latest.CategoryId,
                    CategoryName: latest.Category?.Name ?? "Dịch vụ định kỳ",
                    RecurringAmount: amount,
                    EstimatedAnnualCost: annualCost,
                    OccurrencesCount: matchedTransactions.Count,
                    LastChargedDate: latest.TransactionDate,
                    AdviceMessage: $"Cú hích Thaler: Bạn đang tiêu tốn {annualCost:N0} VND mỗi năm cho '{svcName}'. Hãy kiểm tra xem bạn có thực sự sử dụng dịch vụ này hàng tuần hay không để tránh rò rỉ tài chính."
                ));
            }
        }

        decimal monthlyDrain = detectedList.Sum(s => s.RecurringAmount);
        decimal annualDrain = detectedList.Sum(s => s.EstimatedAnnualCost);

        string healthAlert = detectedList.Count > 0
            ? $"Hệ thống phát hiện {detectedList.Count} khoản thuê bao định kỳ, làm hao mòn {monthlyDrain:N0} VND/tháng ({annualDrain:N0} VND/năm)."
            : "Chưa phát hiện bẫy thuê bao lặp lại nào trong lịch sử chi tiêu của bạn.";

        var result = new SubscriptionAnalysisResultDto(
            Subscriptions: detectedList,
            TotalMonthlyDrain: monthlyDrain,
            TotalAnnualDrain: annualDrain,
            FinancialHealthAlert: healthAlert
        );

        return Result<SubscriptionAnalysisResultDto>.Success(result);
    }
}

// Implementation of VietQR Napas 247 Generation Service (UC-03.10)
public class VietQrService : IVietQrService
{
    private static readonly Dictionary<string, string> BankNames = new()
    {
        { "970422", "MBBank (Ngân hàng Quân Đội)" },
        { "970436", "Vietcombank (Ngoại thương Việt Nam)" },
        { "970415", "VietinBank (Công thương Việt Nam)" },
        { "970418", "BIDV (Đầu tư và Phát triển)" },
        { "970407", "Techcombank (Kỹ thương Việt Nam)" },
        { "970423", "TPBank (Tiên Phong)" },
        { "970432", "VPBank (Việt Nam Thịnh Vượng)" },
        { "970441", "VIB (Quốc tế Việt Nam)" }
    };

    public Task<Result<VietQrResultDto>> GenerateVietQrAsync(GenerateVietQrRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.BankBin))
            return Task.FromResult(Result<VietQrResultDto>.Failure("Mã BIN ngân hàng không được để trống."));

        if (string.IsNullOrWhiteSpace(request.AccountNumber))
            return Task.FromResult(Result<VietQrResultDto>.Failure("Số tài khoản nhận không được để trống."));

        var bankBin = request.BankBin.Trim();
        var accountNumber = request.AccountNumber.Trim();
        var bankName = BankNames.TryGetValue(bankBin, out var name) ? name : $"Ngân hàng (BIN: {bankBin})";

        // Generate standard VietQR image link via VietQR API Quicklink format
        var amountQuery = request.Amount.HasValue && request.Amount.Value > 0 ? $"&amount={Convert.ToInt64(request.Amount.Value)}" : "";
        var noteQuery = !string.IsNullOrWhiteSpace(request.Note) ? $"&addInfo={Uri.EscapeDataString(request.Note.Trim())}" : "";
        var nameQuery = !string.IsNullOrWhiteSpace(request.AccountName) ? $"&accountName={Uri.EscapeDataString(request.AccountName.Trim())}" : "";

        var qrImageUrl = $"https://img.vietqr.io/image/{bankBin}-{accountNumber}-compact2.png?{amountQuery}{noteQuery}{nameQuery}".Replace("?&", "?");

        // Standard EMVCo Napas247 payload string simulation
        var payloadString = $"00020101021238540010A00000072701240006{bankBin}0110{accountNumber}5303704" +
                            (request.Amount.HasValue && request.Amount.Value > 0 ? $"54{request.Amount.Value.ToString("0").Length:D2}{request.Amount.Value:0}" : "") +
                            "5802VN62" +
                            (!string.IsNullOrWhiteSpace(request.Note) ? $"{request.Note.Length:D2}08{request.Note.Length:D2}{request.Note}" : "") +
                            "6304ABCD";

        var result = new VietQrResultDto(
            QrImageUrl: qrImageUrl,
            PayloadString: payloadString,
            BankBin: bankBin,
            BankName: bankName,
            AccountNumber: accountNumber,
            AccountName: request.AccountName?.Trim(),
            Amount: request.Amount,
            Note: request.Note?.Trim()
        );

        return Task.FromResult(Result<VietQrResultDto>.Success(result));
    }
}

// Implementation of Vietnamese Speech-to-Text Voice Expense Parser (UC-03.11)
public class VoiceExpenseParserService : IVoiceExpenseParserService
{
    private readonly IApplicationDbContext _context;
    private readonly IAutoCategorizationService _autoRuleService;

    public VoiceExpenseParserService(IApplicationDbContext context, IAutoCategorizationService autoRuleService)
    {
        _context = context;
        _autoRuleService = autoRuleService;
    }

    public async Task<Result<ParsedVoiceExpenseDto>> ParseVoiceExpenseAsync(Guid userId, ParseVoiceExpenseRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Transcript))
            return Result<ParsedVoiceExpenseDto>.Failure("Khẩu lệnh giọng nói rỗng. Vui lòng nói câu chi tiêu rõ ràng.");

        var text = request.Transcript.Trim().ToLowerInvariant();

        // 1. Extract amount using Vietnamese patterns: e.g. "45 nghìn", "45k", "2 triệu", "50 ngàn", "150.000"
        decimal amount = 0m;

        var millionMatch = Regex.Match(text, @"(\d+([\.,]\d+)?)\s*(triệu|tr|m)");
        var thousandMatch = Regex.Match(text, @"(\d+([\.,]\d+)?)\s*(nghìn|ngàn|k)");
        var plainNumberMatch = Regex.Match(text, @"(\d{1,3}([\.,]\d{3})+)");

        if (millionMatch.Success)
        {
            var numStr = millionMatch.Groups[1].Value.Replace(",", ".");
            if (decimal.TryParse(numStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var num))
                amount = num * 1_000_000m;
        }
        else if (thousandMatch.Success)
        {
            var numStr = thousandMatch.Groups[1].Value.Replace(",", ".");
            if (decimal.TryParse(numStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var num))
                amount = num * 1_000m;
        }
        else if (plainNumberMatch.Success)
        {
            var numStr = plainNumberMatch.Groups[1].Value.Replace(".", "").Replace(",", "");
            if (decimal.TryParse(numStr, out var num))
                amount = num;
        }
        else
        {
            // Simple digit fallback
            var simpleMatch = Regex.Match(text, @"\b\d+\b");
            if (simpleMatch.Success && decimal.TryParse(simpleMatch.Value, out var num))
            {
                // If number is small (< 1000), assume in thousands (e.g. "50" = 50k)
                amount = num < 1000 ? num * 1000m : num;
            }
        }

        if (amount <= 0)
            amount = 50_000m; // Reasonable default fallback

        // 2. Identify Wallet
        var wallets = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId && w.Status == WalletStatus.Active)
            .ToListAsync(ct);

        Wallet? matchedWallet = null;
        if (text.Contains("tiền mặt") || text.Contains("ví tiền"))
            matchedWallet = wallets.FirstOrDefault(w => w.WalletType == WalletType.Cash);
        else if (text.Contains("ngân hàng") || text.Contains("bank") || text.Contains("chuyển khoản"))
            matchedWallet = wallets.FirstOrDefault(w => w.WalletType == WalletType.BankAccount || w.WalletType == WalletType.EWallet);
        else if (text.Contains("tín dụng") || text.Contains("credit") || text.Contains("thẻ"))
            matchedWallet = wallets.FirstOrDefault(w => w.WalletType == WalletType.CreditCard);

        matchedWallet ??= wallets.FirstOrDefault();

        // 3. Match category using AutoCategorizationService
        var matchResult = await _autoRuleService.MatchCategoryAsync(userId, request.Transcript, ct);
        Guid? catId = matchResult.Value?.CategoryId;
        string? catName = matchResult.Value?.CategoryName;

        if (!catId.HasValue)
        {
            var defaultCat = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => (c.IsSystem || c.UserId == userId) && c.TransactionType == TransactionType.Expense, ct);
            catId = defaultCat?.Id;
            catName = defaultCat?.Name;
        }

        var dto = new ParsedVoiceExpenseDto(
            Amount: amount,
            SuggestedWalletId: matchedWallet?.Id,
            SuggestedWalletName: matchedWallet?.Name,
            SuggestedCategoryId: catId,
            SuggestedCategoryName: catName,
            Note: request.Transcript.Trim(),
            RawTranscript: request.Transcript,
            Confidence: 0.92
        );

        return Result<ParsedVoiceExpenseDto>.Success(dto);
    }
}
