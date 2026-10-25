using ExpenseTracker.Application.DTOs.SavingsGoals;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of savings goals, deposit/withdrawal ledger sync, feasibility evaluation, and jars aggregation (UC-06.01 - 06.05)
public class SavingsGoalService : ISavingsGoalService
{
    private readonly IApplicationDbContext _context;

    public SavingsGoalService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SavingsGoalDto>> GetUserGoalsAsync(Guid userId, CancellationToken ct = default)
    {
        var goals = await _context.SavingsGoals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.Priority)
            .ThenBy(g => g.TargetDate)
            .ToListAsync(ct);

        return goals.Select(MapToDto).ToList();
    }

    public async Task<SavingsGoalDto?> GetGoalByIdAsync(Guid userId, Guid goalId, CancellationToken ct = default)
    {
        var goal = await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

        return goal == null ? null : MapToDto(goal);
    }

    public async Task<SavingsGoalDto> CreateGoalAsync(Guid userId, CreateSavingsGoalDto request, CancellationToken ct = default)
    {
        var goal = new SavingsGoal(
            userId,
            request.Name,
            request.TargetAmount,
            request.StartDate,
            request.TargetDate,
            (SavingsGoalType)request.Type,
            (SavingsGoalPriority)request.Priority,
            request.InitialAmount,
            request.Description,
            request.IconKey,
            request.ColorHex
        );

        _context.SavingsGoals.Add(goal);
        await _context.SaveChangesAsync(ct);

        return MapToDto(goal);
    }

    public async Task<SavingsGoalDto> UpdateGoalAsync(Guid userId, Guid goalId, UpdateSavingsGoalDto request, CancellationToken ct = default)
    {
        var goal = await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

        if (goal == null)
            throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

        goal.UpdateDetails(
            request.Name,
            request.Description,
            request.TargetAmount,
            request.TargetDate,
            (SavingsGoalPriority)request.Priority,
            request.IconKey,
            request.ColorHex
        );

        await _context.SaveChangesAsync(ct);
        return MapToDto(goal);
    }

    public async Task<SavingsGoalDto> PatchGoalAsync(Guid userId, Guid goalId, PatchSavingsGoalDto request, CancellationToken ct = default)
    {
        var goal = await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

        if (goal == null)
            throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

        goal.PatchDetails(
            name: request.Name,
            description: request.Description,
            targetAmount: request.TargetAmount,
            targetDate: request.TargetDate,
            priority: request.Priority.HasValue ? (SavingsGoalPriority)request.Priority.Value : null,
            iconKey: request.IconKey,
            colorHex: request.ColorHex,
            status: request.Status.HasValue ? (SavingsGoalStatus)request.Status.Value : null
        );

        await _context.SaveChangesAsync(ct);
        return MapToDto(goal);
    }

    public async Task<SavingsGoalDto> PatchGoalStatusAsync(Guid userId, Guid goalId, int status, CancellationToken ct = default)
    {
        var goal = await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

        if (goal == null)
            throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

        if (!Enum.IsDefined(typeof(SavingsGoalStatus), status))
            throw new ArgumentException("Trạng thái mục tiêu không hợp lệ.", nameof(status));

        goal.UpdateStatus((SavingsGoalStatus)status);

        await _context.SaveChangesAsync(ct);
        return MapToDto(goal);
    }

    public async Task DeleteGoalAsync(Guid userId, Guid goalId, CancellationToken ct = default)
    {
        var goal = await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

        if (goal == null)
            throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

        if (goal.CurrentAmount > 0)
        {
            throw new InvalidOperationException(
                $"Mục tiêu '{goal.Name}' vẫn còn số dư {goal.CurrentAmount:N0} đ. Vui lòng rút hết tiền về ví trước khi xóa mục tiêu.");
        }

        _context.SavingsGoals.Remove(goal);
        await _context.SaveChangesAsync(ct);
    }

    // Deposit money from a wallet into savings goal (UC-06.02)
    public async Task<SavingsGoalContributionDto> DepositAsync(Guid userId, Guid goalId, DepositSavingsGoalDto request, CancellationToken ct = default)
    {
        using var tx = await _context.BeginTransactionAsync(ct);
        try
        {
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

            if (goal == null)
                throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == request.WalletId && w.UserId == userId, ct);

            if (wallet == null)
                throw new KeyNotFoundException("Ví nguồn thanh toán không tồn tại hoặc không thuộc quyền sở hữu.");

            // 1. Debit wallet balance (ACID verification inside Wallet entity)
            wallet.Debit(request.Amount);

            // 2. Deposit into goal and generate contribution record
            var contribution = goal.Deposit(request.Amount, request.WalletId, request.Note);
            await _context.SavingsGoalContributions.AddAsync(contribution, ct);
            await _context.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);

            return MapContributionToDto(contribution, wallet.Name);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    // Withdraw money from savings goal with Richard Thaler Nudge (UC-06.03)
    public async Task<SavingsGoalContributionDto> WithdrawAsync(Guid userId, Guid goalId, WithdrawSavingsGoalDto request, CancellationToken ct = default)
    {
        using var tx = await _context.BeginTransactionAsync(ct);
        try
        {
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

            if (goal == null)
                throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == request.TargetWalletId && w.UserId == userId, ct);

            if (wallet == null)
                throw new KeyNotFoundException("Ví nhận tiền không tồn tại hoặc không thuộc quyền sở hữu.");

            // 1. Withdraw from goal (validation & cognitive friction check inside entity)
            var contribution = goal.Withdraw(request.Amount, request.TargetWalletId, request.Reason);
            await _context.SavingsGoalContributions.AddAsync(contribution, ct);

            // 2. Credit target wallet
            wallet.Credit(request.Amount);
            await _context.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);

            return MapContributionToDto(contribution, wallet.Name);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<List<SavingsGoalContributionDto>> GetGoalContributionsAsync(Guid userId, Guid goalId, CancellationToken ct = default)
    {
        var goalExists = await _context.SavingsGoals
            .AnyAsync(g => g.Id == goalId && g.UserId == userId, ct);

        if (!goalExists)
            throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

        var contributions = await _context.SavingsGoalContributions
            .Include(c => c.Wallet)
            .Where(c => c.SavingsGoalId == goalId)
            .OrderByDescending(c => c.ContributionDate)
            .ToListAsync(ct);

        return contributions
            .Select(c => MapContributionToDto(c, c.Wallet?.Name ?? "Ví không xác định"))
            .ToList();
    }

    // Evaluate financial feasibility comparing required monthly deposit with active budget (UC-06.01)
    public async Task<GoalFeasibilityDto> EvaluateGoalFeasibilityAsync(Guid userId, Guid goalId, CancellationToken ct = default)
    {
        var goal = await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

        if (goal == null)
            throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

        var today = DateTime.Today;
        decimal monthlyDepositRequired = goal.CalculateMonthlyDepositRequired(today);
        int monthsRemaining = Math.Max(1, ((goal.TargetDate.Year - today.Year) * 12) + goal.TargetDate.Month - today.Month);

        // Fetch user's active budget from Phase 5 (50/30/20 rule allocation)
        var activeBudget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Status == BudgetStatus.Active, ct);

        // Standard 50/30/20 benchmark: 20% of total budget is designated for savings/debt
        decimal monthlyAvailableSavings = (activeBudget != null && activeBudget.TotalAmount > 0)
            ? Math.Round(activeBudget.TotalAmount * 0.20m, 0)
            : 2_000_000m; // Fallback baseline 2M VND if no budget or budget amount is 0

        decimal feasibilityPercent = monthlyAvailableSavings > 0
            ? Math.Round((monthlyDepositRequired / monthlyAvailableSavings) * 100m, 1)
            : 0m;

        bool isFeasible = monthlyDepositRequired <= monthlyAvailableSavings;
        string adviceMessage;

        if (goal.IsCompleted)
        {
            adviceMessage = "🎉 Chúc mừng! Mục tiêu này đã hoàn thành 100%. Bạn có thể chuyển sang mục tiêu tài chính tiếp theo.";
        }
        else if (isFeasible)
        {
            adviceMessage = $"✅ Tính khả thi cao! Mức tích lũy {monthlyDepositRequired:N0} đ/tháng chiếm {feasibilityPercent:F1}% hạn mức tiết kiệm dự kiến ({monthlyAvailableSavings:N0} đ/tháng).";
        }
        else
        {
            int extraMonths = monthlyAvailableSavings > 0 
                ? (int)Math.Max(1, Math.Ceiling((monthlyDepositRequired - monthlyAvailableSavings) / monthlyAvailableSavings)) 
                : 3;
            adviceMessage = $"⚠️ Cảnh báo khả thi: Mức tích lũy cần thiết {monthlyDepositRequired:N0} đ/tháng vượt quá {feasibilityPercent:F1}% hạn mức tiết kiệm ({monthlyAvailableSavings:N0} đ/tháng). Bạn nên dời hạn chót thêm {extraMonths} tháng hoặc cắt giảm chi tiêu không thiết yếu.";
        }

        return new GoalFeasibilityDto(
            goal.Id,
            goal.Name,
            goal.TargetAmount,
            goal.CurrentAmount,
            goal.RemainingAmount,
            monthsRemaining,
            monthlyDepositRequired,
            monthlyAvailableSavings,
            feasibilityPercent,
            isFeasible,
            adviceMessage
        );
    }

    // Aggregate savings breakdown across Kakeibo/6 Jars categories (UC-06.05)
    public async Task<JarsSummaryDto> GetJarsSummaryAsync(Guid userId, CancellationToken ct = default)
    {
        var goals = await _context.SavingsGoals
            .Where(g => g.UserId == userId && g.Status != SavingsGoalStatus.Cancelled)
            .ToListAsync(ct);

        decimal totalSaved = goals.Sum(g => g.CurrentAmount);
        decimal totalTarget = goals.Sum(g => g.TargetAmount);
        decimal overallProgress = totalTarget > 0 
            ? Math.Clamp(Math.Round((totalSaved / totalTarget) * 100m, 2), 0m, 100m) 
            : 0m;

        int activeCount = goals.Count(g => g.Status == SavingsGoalStatus.Active);
        int completedCount = goals.Count(g => g.Status == SavingsGoalStatus.Completed);

        // Group by goal type
        var breakdown = Enum.GetValues<SavingsGoalType>()
            .Select(type =>
            {
                var typeGoals = goals.Where(g => g.Type == type).ToList();
                decimal typeCurrent = typeGoals.Sum(g => g.CurrentAmount);
                decimal typeTarget = typeGoals.Sum(g => g.TargetAmount);
                decimal shareOfTotal = totalSaved > 0 
                    ? Math.Round((typeCurrent / totalSaved) * 100m, 1) 
                    : 0m;

                return new JarCategorySummaryDto(
                    (int)type,
                    GetJarName(type),
                    GetJarIcon(type),
                    GetJarColor(type),
                    typeCurrent,
                    typeTarget,
                    shareOfTotal,
                    typeGoals.Count
                );
            })
            .Where(j => j.GoalsCount > 0)
            .OrderByDescending(j => j.TotalCurrentAmount)
            .ToList();

        return new JarsSummaryDto(
            totalSaved,
            totalTarget,
            overallProgress,
            activeCount,
            completedCount,
            breakdown
        );
    }

    // Configure Auto-deposit schedule (UC-06.04)
    public async Task<SavingsGoalDto> ConfigureAutoDepositAsync(Guid userId, Guid goalId, ConfigureAutoDepositDto request, CancellationToken ct = default)
    {
        var goal = await _context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId, ct);

        if (goal == null)
            throw new KeyNotFoundException("Không tìm thấy mục tiêu tích lũy.");

        if (request.IsEnabled && request.WalletId.HasValue)
        {
            var walletExists = await _context.Wallets
                .AnyAsync(w => w.Id == request.WalletId.Value && w.UserId == userId, ct);

            if (!walletExists)
                throw new KeyNotFoundException("Ví nguồn trích nạp tự động không tồn tại.");
        }

        goal.ConfigureAutoDeposit(request.IsEnabled, request.Amount, request.DayOfMonth, request.WalletId);
        await _context.SaveChangesAsync(ct);

        return MapToDto(goal);
    }

    // Mappers
    private static SavingsGoalDto MapToDto(SavingsGoal g) => new(
        g.Id,
        g.UserId,
        g.Name,
        g.Description,
        g.TargetAmount,
        g.CurrentAmount,
        g.StartDate,
        g.TargetDate,
        (int)g.Type,
        GetJarName(g.Type),
        (int)g.Status,
        g.Status switch
        {
            SavingsGoalStatus.Active => "Đang thực hiện",
            SavingsGoalStatus.Completed => "Đã hoàn thành",
            SavingsGoalStatus.Paused => "Tạm dừng",
            SavingsGoalStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        },
        (int)g.Priority,
        g.Priority switch
        {
            SavingsGoalPriority.High => "Ưu tiên cao",
            SavingsGoalPriority.Medium => "Ưu tiên trung bình",
            SavingsGoalPriority.Low => "Ưu tiên thấp",
            _ => "Trung bình"
        },
        g.IconKey,
        g.ColorHex,
        g.CompletedAt,
        g.ProgressPercent,
        g.RemainingAmount,
        g.IsCompleted,
        g.IsEmergencyFund,
        g.IsAutoDepositEnabled,
        g.AutoDepositAmount,
        g.AutoDepositDayOfMonth,
        g.AutoDepositWalletId
    );

    private static SavingsGoalContributionDto MapContributionToDto(SavingsGoalContribution c, string walletName) => new(
        c.Id,
        c.SavingsGoalId,
        c.WalletId,
        walletName,
        c.Amount,
        (int)c.Type,
        c.Type switch
        {
            ContributionType.Deposit => "Nạp tiền",
            ContributionType.Withdrawal => "Rút tiền",
            ContributionType.InterestOrBonus => "Tiền lãi/Thưởng",
            _ => "Khác"
        },
        c.ContributionDate,
        c.Note,
        c.TransactionId
    );

    private static string GetJarName(SavingsGoalType type) => type switch
    {
        SavingsGoalType.EmergencyFund => "Hũ Quỹ Khẩn Cấp (Emergency)",
        SavingsGoalType.MajorPurchase => "Hũ Mua Sắm Lớn (LTSS)",
        SavingsGoalType.Investment => "Hũ Tự Do Tài Chính (FFA)",
        SavingsGoalType.Education => "Hũ Học Vấn & Phát Triển (EDU)",
        SavingsGoalType.Travel => "Hũ Du Lịch & Trải Nghiệm (PLAY)",
        _ => "Hũ Mục Tiêu Khác"
    };

    private static string GetJarIcon(SavingsGoalType type) => type switch
    {
        SavingsGoalType.EmergencyFund => "shield-check",
        SavingsGoalType.MajorPurchase => "car",
        SavingsGoalType.Investment => "trending-up",
        SavingsGoalType.Education => "academic-cap",
        SavingsGoalType.Travel => "globe-alt",
        _ => "piggy-bank"
    };

    private static string GetJarColor(SavingsGoalType type) => type switch
    {
        SavingsGoalType.EmergencyFund => "#DC2626", // Red for Emergency Protection
        SavingsGoalType.MajorPurchase => "#2563EB", // Blue for Major Purchase
        SavingsGoalType.Investment => "#059669",    // Green for Financial Freedom
        SavingsGoalType.Education => "#7C3AED",     // Purple for Education
        SavingsGoalType.Travel => "#F59E0B",        // Amber for Travel
        _ => "#6B7280"
    };
}
