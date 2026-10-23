using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of Adaptive Daily Spending Allowance (DSA) and Spending Velocity Tracker (UC-05.05/05.06)
public class AdaptiveDsaService : IAdaptiveDsaService
{
    private readonly IApplicationDbContext _context;

    public AdaptiveDsaService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DsaStatusDto>> GetCurrentDsaStatusAsync(Guid userId, CancellationToken ct = default)
    {
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (activeBudget == null)
            return Result<DsaStatusDto>.Failure("Không tìm thấy kế hoạch ngân sách đang hoạt động.");

        var today = DateTime.UtcNow.Date;
        var totalDays = Math.Max(1, (activeBudget.EndDate.Date - activeBudget.StartDate.Date).Days + 1);
        var currentDayIndex = Math.Max(1, (today - activeBudget.StartDate.Date).Days + 1);
        var remainingDays = Math.Max(1, totalDays - currentDayIndex + 1);

        // Calculate flexible budget (Wants / Discretionary categories)
        var flexibleAllocations = activeBudget.Allocations
            .Where(a => a.Category?.BudgetRuleMapping == BudgetRuleMapping.Wants)
            .ToList();

        // If no Wants explicitly defined, use total remaining budget
        decimal totalFlexible = flexibleAllocations.Count > 0 
            ? flexibleAllocations.Sum(a => a.AllocatedAmount) 
            : activeBudget.TotalAmount;

        decimal totalSpentFlexible = flexibleAllocations.Count > 0
            ? flexibleAllocations.Sum(a => a.CurrentSpent)
            : activeBudget.TotalSpent;

        // Query today's spending in transactions
        var spentToday = await _context.Transactions
            .Where(t => t.UserId == userId && t.TransactionType == TransactionType.Expense && t.TransactionDate.Date == today && !t.IsDeleted)
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

        // Spent until yesterday = totalSpent - spentToday
        var spentUntilYesterday = Math.Max(0m, totalSpentFlexible - spentToday);
        var remainingFlexible = Math.Max(0m, totalFlexible - spentUntilYesterday);

        // Adaptive DSA formula: RemainingFlexible / RemainingDays
        decimal dailyAllowance = remainingDays > 0 && remainingFlexible > 0
            ? Math.Round(remainingFlexible / remainingDays, 0)
            : 0m;

        decimal remainingToday = dailyAllowance - spentToday;
        bool isExceeded = remainingToday < 0;

        string adviceMessage = isExceeded
            ? $"⚠️ Bạn đã tiêu vượt {Math.Abs(remainingToday):N0} đ so với hạn mức an toàn hôm nay. Ngày mai hệ thống sẽ tự động siết giảm DSA để bảo toàn tài chính."
            : $"✅ Hạn mức an toàn hôm nay: {dailyAllowance:N0} đ. Bạn còn {remainingToday:N0} đ khả dụng trong ngày.";

        var dto = new DsaStatusDto(
            today,
            dailyAllowance,
            spentToday,
            remainingToday,
            remainingFlexible,
            remainingDays,
            isExceeded,
            adviceMessage
        );

        return Result<DsaStatusDto>.Success(dto);
    }

    public async Task<Result<VelocityStatusDto>> GetSpendingVelocityAsync(Guid userId, CancellationToken ct = default)
    {
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (activeBudget == null)
            return Result<VelocityStatusDto>.Failure("Không tìm thấy kế hoạch ngân sách đang hoạt động.");

        var today = DateTime.UtcNow.Date;
        var totalDays = Math.Max(1, (activeBudget.EndDate.Date - activeBudget.StartDate.Date).Days + 1);
        var daysPassed = Math.Max(1, (today - activeBudget.StartDate.Date).Days + 1);

        decimal targetVelocity = Math.Round(activeBudget.TotalAmount / totalDays, 0);
        decimal actualVelocity = Math.Round(activeBudget.TotalSpent / daysPassed, 0);

        decimal velocityIndex = targetVelocity > 0
            ? Math.Round(actualVelocity / targetVelocity, 2)
            : 1.0m;

        string levelName;
        string colorHex;
        DateTime? forecastDate = null;
        int? daysUntilDepletion = null;
        string forecastMessage;

        if (daysPassed < 3)
        {
            levelName = "Đang tích lũy";
            colorHex = "#3B82F6"; // Blue
            forecastMessage = "Hệ thống đang tích lũy dữ liệu chi tiêu ban đầu (cần tối thiểu 3 ngày để tính toán vận tốc chuẩn xác).";
        }
        else if (velocityIndex <= 1.0m)
        {
            levelName = "An toàn";
            colorHex = "#10B981"; // Green
            forecastMessage = $"Vận tốc tiêu tiền an toàn ({velocityIndex:F2}x). Bạn đang giữ đúng tiến độ kế hoạch tài chính đề ra.";
        }
        else if (velocityIndex <= 1.2m)
        {
            levelName = "Cảnh giác";
            colorHex = "#F59E0B"; // Amber
            forecastMessage = $"Tốc độ tiêu tiền hơi nhanh hơn kế hoạch ({velocityIndex:F2}x). Hãy lưu ý cắt giảm các chi phí tùy chọn.";
        }
        else
        {
            levelName = "Nguy hiểm";
            colorHex = "#EF4444"; // Red
            
            // Forecast out-of-money date (UC-05.06)
            if (actualVelocity > 0 && activeBudget.TotalRemaining > 0)
            {
                daysUntilDepletion = (int)Math.Floor(activeBudget.TotalRemaining / actualVelocity);
                forecastDate = today.AddDays(daysUntilDepletion.Value);
                forecastMessage = $"🚨 BÁO ĐỘNG: Tốc độ tiêu tiền quá cao ({velocityIndex:F2}x). Với tốc độ này ({actualVelocity:N0} đ/ngày), bạn dự kiến sẽ CẠN SẠCH TIỀN vào ngày {forecastDate.Value:dd/MM/yyyy} (sớm hơn {Math.Max(0, (activeBudget.EndDate.Date - forecastDate.Value).Days)} ngày so với cuối kỳ)!";
            }
            else
            {
                forecastMessage = "🚨 BÁO ĐỘNG: Bạn đã cạn sạch ngân sách của toàn bộ chu kỳ hiện tại!";
            }
        }

        var dto = new VelocityStatusDto(
            velocityIndex,
            levelName,
            colorHex,
            actualVelocity,
            targetVelocity,
            forecastDate,
            daysUntilDepletion,
            forecastMessage
        );

        return Result<VelocityStatusDto>.Success(dto);
    }

    public async Task<Result<BudgetProgressDto>> GetBudgetProgressOverviewAsync(Guid userId, CancellationToken ct = default)
    {
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (activeBudget == null)
            return Result<BudgetProgressDto>.Failure("Không tìm thấy kế hoạch ngân sách đang hoạt động.");

        var dsaResult = await GetCurrentDsaStatusAsync(userId, ct);
        var velocityResult = await GetSpendingVelocityAsync(userId, ct);

        var today = DateTime.UtcNow.Date;
        var totalDays = Math.Max(1, (activeBudget.EndDate.Date - activeBudget.StartDate.Date).Days + 1);
        var daysPassed = Math.Max(1, (today - activeBudget.StartDate.Date).Days + 1);

        var allocations = activeBudget.Allocations.Select(a => new BudgetAllocationDto(
            a.Id,
            a.CategoryId,
            a.Category?.Name ?? "Chưa rõ",
            a.Category?.IconKey,
            a.Category?.ColorHex,
            a.Category?.BudgetRuleMapping.HasValue == true ? (int)a.Category.BudgetRuleMapping.Value : null,
            a.AllocatedAmount,
            a.CurrentSpent,
            a.RemainingAmount,
            a.SpentPercent,
            a.AlertThresholdPercent,
            a.IsAlertTriggered,
            a.IsExceeded
        )).ToList();

        var dto = new BudgetProgressDto(
            activeBudget.Id,
            activeBudget.Name,
            activeBudget.TotalAmount,
            activeBudget.TotalSpent,
            activeBudget.TotalRemaining,
            activeBudget.OverallSpentPercent,
            daysPassed,
            totalDays,
            dsaResult.Value!,
            velocityResult.Value!,
            allocations
        );

        return Result<BudgetProgressDto>.Success(dto);
    }

    public async Task RecalculateDsaForTodayAsync(Guid budgetId, CancellationToken ct = default)
    {
        var budget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .FirstOrDefaultAsync(b => b.Id == budgetId, ct);

        if (budget == null || budget.Status != BudgetStatus.Active) return;

        var today = DateTime.UtcNow.Date;
        var totalDays = Math.Max(1, (budget.EndDate.Date - budget.StartDate.Date).Days + 1);
        var currentDayIndex = Math.Max(1, (today - budget.StartDate.Date).Days + 1);
        var remainingDays = Math.Max(1, totalDays - currentDayIndex + 1);

        var flexibleAllocations = budget.Allocations
            .Where(a => a.Category?.BudgetRuleMapping == BudgetRuleMapping.Wants)
            .ToList();

        decimal totalFlexible = flexibleAllocations.Count > 0 
            ? flexibleAllocations.Sum(a => a.AllocatedAmount) 
            : budget.TotalAmount;

        decimal totalSpentFlexible = flexibleAllocations.Count > 0
            ? flexibleAllocations.Sum(a => a.CurrentSpent)
            : budget.TotalSpent;

        var spentToday = await _context.Transactions
            .Where(t => t.UserId == budget.UserId && t.TransactionType == TransactionType.Expense && t.TransactionDate.Date == today && !t.IsDeleted)
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

        var spentUntilYesterday = Math.Max(0m, totalSpentFlexible - spentToday);
        var remainingFlexible = Math.Max(0m, totalFlexible - spentUntilYesterday);

        decimal dailyAllowance = remainingDays > 0 && remainingFlexible > 0
            ? Math.Round(remainingFlexible / remainingDays, 0)
            : 0m;

        // Calculate velocity
        decimal targetVelocity = Math.Round(budget.TotalAmount / totalDays, 0);
        decimal actualVelocity = Math.Round(budget.TotalSpent / currentDayIndex, 0);
        decimal velocityIndex = targetVelocity > 0 ? Math.Round(actualVelocity / targetVelocity, 2) : 1.0m;

        DateTime? outOfMoneyForecast = null;
        if (actualVelocity > 0 && budget.TotalRemaining > 0 && velocityIndex > 1.2m)
        {
            var daysUntilDepletion = (int)Math.Floor(budget.TotalRemaining / actualVelocity);
            outOfMoneyForecast = today.AddDays(daysUntilDepletion);
        }

        var existingDsa = await _context.DailySpendingAllowances
            .FirstOrDefaultAsync(d => d.BudgetId == budgetId && d.Date == today, ct);

        if (existingDsa != null)
        {
            existingDsa.UpdateAllowance(dailyAllowance, spentToday, velocityIndex, outOfMoneyForecast);
        }
        else
        {
            var newDsa = new DailySpendingAllowance(
                budgetId,
                today,
                dailyAllowance,
                spentToday,
                velocityIndex,
                outOfMoneyForecast
            );
            _context.DailySpendingAllowances.Add(newDsa);
        }

        await _context.SaveChangesAsync(ct);
    }
}
