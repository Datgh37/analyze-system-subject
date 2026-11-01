using ExpenseTracker.Application.DTOs.EmergencyFund;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Application Service managing Emergency Fund recommendations, setup, and behavioral discipline (UC-06.04)
public class EmergencyFundService : IEmergencyFundService
{
    private readonly IApplicationDbContext _context;

    public EmergencyFundService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmergencyFundRecommendationDto> GetRecommendationAsync(Guid userId, CancellationToken ct = default)
    {
        var monthlyEssential = await CalculateAverageMonthlyEssentialNeedsAsync(userId, ct);
        var fund = await GetEmergencyGoalAsync(userId, ct);

        var currentAmount = fund?.CurrentAmount ?? 0m;
        var monthsCovered = monthlyEssential > 0 ? Math.Round(currentAmount / monthlyEssential, 1) : 0m;

        string note;
        if (monthsCovered < 3m)
        {
            note = "Khuyến nghị ưu tiên: Bạn nên tích lũy tối thiểu 3 tháng chi phí thiết yếu để phòng vệ trước các rủi ro mất việc hoặc viện phí đột xuất.";
        }
        else if (monthsCovered < 6m)
        {
            note = "Rất tốt! Bạn đã đạt lá chắn cơ bản 3 tháng. Hãy tiếp tục hướng tới mốc chuẩn mực 6 tháng để bảo vệ toàn diện gia đình.";
        }
        else
        {
            note = "Tuyệt vời! Quỹ dự phòng khẩn cấp của bạn rất vững chắc, sẵn sàng ứng phó mọi biến cố cuộc sống.";
        }

        return new EmergencyFundRecommendationDto
        {
            MonthlyEssentialExpenses = monthlyEssential,
            ThreeMonthsAmount = monthlyEssential * 3m,
            SixMonthsAmount = monthlyEssential * 6m,
            TwelveMonthsAmount = monthlyEssential * 12m,
            CurrentEmergencyFundAmount = currentAmount,
            CurrentMonthsCovered = monthsCovered,
            RecommendationNote = note
        };
    }

    public async Task<EmergencyFundStatusDto> GetStatusAsync(Guid userId, CancellationToken ct = default)
    {
        var fund = await GetEmergencyGoalAsync(userId, ct);
        var monthlyEssential = await CalculateAverageMonthlyEssentialNeedsAsync(userId, ct);

        if (fund == null)
        {
            return new EmergencyFundStatusDto
            {
                HasFundConfigured = false,
                Status = "NotConfigured",
                MonthlyEssentialExpenses = monthlyEssential,
                IsLocked = true
            };
        }

        var monthsCovered = monthlyEssential > 0 ? Math.Round(fund.CurrentAmount / monthlyEssential, 1) : 0m;
        var lastDeposit = await _context.SavingsGoalContributions
            .Where(c => c.SavingsGoalId == fund.Id && c.Type == ContributionType.Deposit)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => (DateTime?)c.CreatedAt)
            .FirstOrDefaultAsync(ct);

        int targetMonths = 6;
        if (monthlyEssential > 0 && fund.TargetAmount > 0)
        {
            var calculatedMonths = (int)Math.Round(fund.TargetAmount / monthlyEssential, MidpointRounding.AwayFromZero);
            if (calculatedMonths is 3 or 6 or 12) targetMonths = calculatedMonths;
        }

        return new EmergencyFundStatusDto
        {
            SavingsGoalId = fund.Id,
            HasFundConfigured = true,
            TargetMonths = targetMonths,
            TargetAmount = fund.TargetAmount,
            CurrentAmount = fund.CurrentAmount,
            MonthlyEssentialExpenses = monthlyEssential,
            MonthsCovered = monthsCovered,
            ProgressPercent = fund.ProgressPercent,
            IsLocked = true,
            Status = fund.Status.ToString(),
            LastDepositAt = lastDeposit
        };
    }

    public async Task<EmergencyFundStatusDto> SetupFundAsync(Guid userId, SetupEmergencyFundDto request, CancellationToken ct = default)
    {
        if (request.TargetMonths <= 0)
            throw new ArgumentException("Số tháng mục tiêu tích lũy phải lớn hơn 0.", nameof(request.TargetMonths));

        var monthlyEssential = await CalculateAverageMonthlyEssentialNeedsAsync(userId, ct);
        var targetAmount = request.TargetAmount ?? (monthlyEssential * request.TargetMonths);

        if (targetAmount <= 0)
            targetAmount = 15_000_000m; // Default minimum fallback

        var fund = await GetEmergencyGoalAsync(userId, ct);

        if (fund == null)
        {
            fund = new SavingsGoal(
                userId,
                "Quỹ Dự Phòng Khẩn Cấp",
                targetAmount,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(Math.Max(request.TargetMonths * 2, 6)),
                SavingsGoalType.EmergencyFund,
                SavingsGoalPriority.High,
                0m,
                "Quỹ dự phòng an sinh cá nhân, phòng ngừa rủi ro mất việc, viện phí, sự cố cấp bách.",
                "shield-check",
                "#DC2626");

            await _context.SavingsGoals.AddAsync(fund, ct);
        }
        else
        {
            fund.UpdateDetails(
                "Quỹ Dự Phòng Khẩn Cấp",
                fund.Description,
                targetAmount,
                DateTime.UtcNow.AddMonths(Math.Max(request.TargetMonths * 2, 6)),
                SavingsGoalPriority.High,
                "shield-check",
                "#DC2626");
        }

        // Process initial deposit if requested
        if (request.InitialDepositAmount.HasValue && request.InitialDepositAmount.Value > 0)
        {
            if (!request.InitialDepositWalletId.HasValue || request.InitialDepositWalletId.Value == Guid.Empty)
                throw new ArgumentException("Vui lòng chọn ví nguồn trích nạp ban đầu.", nameof(request.InitialDepositWalletId));

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == request.InitialDepositWalletId.Value && w.UserId == userId, ct);

            if (wallet == null)
                throw new KeyNotFoundException("Ví nguồn thanh toán không tồn tại hoặc không thuộc quyền sở hữu.");

            wallet.Debit(request.InitialDepositAmount.Value);
            var contribution = fund.Deposit(request.InitialDepositAmount.Value, wallet.Id, "Khởi tạo Quỹ Dự Phòng Khẩn Cấp");
            await _context.SavingsGoalContributions.AddAsync(contribution, ct);
        }

        await _context.SaveChangesAsync(ct);
        return await GetStatusAsync(userId, ct);
    }

    public async Task<EmergencyFundStatusDto> DepositAsync(Guid userId, DepositEmergencyFundDto request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Số tiền nạp vào quỹ khẩn cấp phải lớn hơn 0 đ.", nameof(request.Amount));

        var fund = await GetEmergencyGoalAsync(userId, ct);
        if (fund == null)
            throw new InvalidOperationException("Chưa thiết lập Quỹ Khẩn Cấp. Vui lòng thiết lập quỹ trước khi nạp tiền.");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == request.SourceWalletId && w.UserId == userId, ct);

        if (wallet == null)
            throw new KeyNotFoundException("Ví nguồn thanh toán không tồn tại hoặc không thuộc quyền sở hữu.");

        wallet.Debit(request.Amount);
        var contribution = fund.Deposit(request.Amount, wallet.Id, request.Note ?? "Nạp tích lũy Quỹ Khẩn Cấp");
        await _context.SavingsGoalContributions.AddAsync(contribution, ct);

        await _context.SaveChangesAsync(ct);
        return await GetStatusAsync(userId, ct);
    }

    public async Task<EmergencyFundStatusDto> WithdrawAsync(Guid userId, WithdrawEmergencyFundDto request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Số tiền rút phải lớn hơn 0 đ.", nameof(request.Amount));

        if (string.IsNullOrWhiteSpace(request.EmergencyReason))
            throw new ArgumentException("Vui lòng chọn lý do rút tiền khẩn cấp chính đáng.", nameof(request.EmergencyReason));

        if (string.IsNullOrWhiteSpace(request.DetailedExplanation) || request.DetailedExplanation.Trim().Length < 10)
            throw new ArgumentException("Vui lòng cung cấp giải trình chi tiết (tối thiểu 10 ký tự) để vượt qua ma sát tâm lý kỷ luật.", nameof(request.DetailedExplanation));

        var fund = await GetEmergencyGoalAsync(userId, ct);
        if (fund == null)
            throw new InvalidOperationException("Chưa thiết lập Quỹ Khẩn Cấp.");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == request.TargetWalletId && w.UserId == userId, ct);

        if (wallet == null)
            throw new KeyNotFoundException("Ví đích không tồn tại hoặc không thuộc quyền sở hữu.");

        var reason = $"[Khẩn cấp: {request.EmergencyReason}] {request.DetailedExplanation.Trim()}";
        var contribution = fund.Withdraw(request.Amount, wallet.Id, reason);
        wallet.Credit(request.Amount);
        await _context.SavingsGoalContributions.AddAsync(contribution, ct);

        // Generate Richard Thaler behavioral nudge warning
        var nudge = new FinancialNudge(
            userId,
            NudgeType.PayYourselfFirst,
            "Cảnh báo giải ngân Quỹ Khẩn Cấp",
            $"Bạn vừa rút {request.Amount:N0} đ từ Quỹ Khẩn Cấp cho sự kiện: '{request.EmergencyReason}'. Hãy sớm bù đắp lại số tiền này để duy trì lá chắn an toàn.",
            "EmergencyFundWithdrawal");
        await _context.FinancialNudges.AddAsync(nudge, ct);

        await _context.SaveChangesAsync(ct);
        return await GetStatusAsync(userId, ct);
    }

    private async Task<SavingsGoal?> GetEmergencyGoalAsync(Guid userId, CancellationToken ct)
    {
        return await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.Type == SavingsGoalType.EmergencyFund && g.Status != SavingsGoalStatus.Cancelled, ct);
    }

    private async Task<decimal> CalculateAverageMonthlyEssentialNeedsAsync(Guid userId, CancellationToken ct)
    {
        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);

        // 1. Calculate historical Needs expenses (Categories mapped to Needs or standard essential categories)
        var needsExpenseTotal = await _context.Transactions
            .Where(t => t.UserId == userId && 
                        t.TransactionType == TransactionType.Expense && 
                        t.TransactionDate >= ninetyDaysAgo &&
                        (t.Category.BudgetRuleMapping == BudgetRuleMapping.Needs || 
                         t.Category.Name.Contains("Ăn uống") || 
                         t.Category.Name.Contains("Nhà") || 
                         t.Category.Name.Contains("Điện") || 
                         t.Category.Name.Contains("Y tế")))
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

        // If user has 90 days of expense data
        if (needsExpenseTotal > 0)
        {
            var averageMonthly = Math.Round(needsExpenseTotal / 3m, 0);
            return averageMonthly > 0 ? averageMonthly : 5_000_000m;
        }

        // 2. Fallback to general monthly expense if no explicit needs mapped
        var totalRecentExpense = await _context.Transactions
            .Where(t => t.UserId == userId && t.TransactionType == TransactionType.Expense && t.TransactionDate >= ninetyDaysAgo)
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

        if (totalRecentExpense > 0)
        {
            // Assume 50% of total expenses are essential needs (50/30/20 rule)
            var estimatedNeeds = Math.Round((totalRecentExpense / 3m) * 0.5m, 0);
            return estimatedNeeds > 0 ? estimatedNeeds : 5_000_000m;
        }

        // 3. Fallback standard default: 5,000,000 VND / month
        return 5_000_000m;
    }
}
