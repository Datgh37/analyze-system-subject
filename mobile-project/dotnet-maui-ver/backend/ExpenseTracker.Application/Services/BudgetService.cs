using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of budget cycle planning, allocation, rebalancing, and ledger synchronization (UC-05.01 - 05.04)
public class BudgetService : IBudgetService
{
    private readonly IApplicationDbContext _context;

    public BudgetService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BudgetDto>> CreateBudgetAsync(Guid userId, CreateBudgetRequest request, CancellationToken ct = default)
    {
        if (request.TotalAmount <= 0)
            return Result<BudgetDto>.Failure("Tổng số tiền ngân sách phải lớn hơn 0 đ.");

        if (request.EndDate.Date < request.StartDate.Date)
            return Result<BudgetDto>.Failure("Ngày kết thúc ngân sách phải sau hoặc bằng ngày bắt đầu.");

        // Check method validation
        var method = (BudgetMethod)request.Method;
        var periodType = (BudgetPeriodType)request.PeriodType;

        var budget = new Budget(
            userId,
            request.Name,
            request.TotalAmount,
            request.StartDate,
            request.EndDate,
            periodType,
            method
        );

        // Process allocations
        if (request.Allocations != null && request.Allocations.Count > 0)
        {
            decimal sumAllocated = request.Allocations.Sum(a => a.AllocatedAmount);

            // UC-05.01 E1: Zero-Based Budgeting invariant: Income - Allocations = 0
            if (method == BudgetMethod.ZeroBased && sumAllocated != request.TotalAmount)
            {
                return Result<BudgetDto>.Failure($"Phương pháp Ngân sách số không (ZBB) yêu cầu tổng phân bổ ({sumAllocated:N0} đ) phải bằng chính xác tổng ngân sách ({request.TotalAmount:N0} đ).");
            }

            foreach (var alloc in request.Allocations)
            {
                budget.AddAllocation(alloc.CategoryId, alloc.AllocatedAmount, alloc.AlertThresholdPercent);
            }
        }
        else if (method == BudgetMethod.Rule503020)
        {
            // Auto-generate high-level allocations if user selected 50/30/20 with existing categories
            var expenseCategories = await _context.Categories
                .Where(c => (c.UserId == userId || c.IsSystem) && c.TransactionType == TransactionType.Expense && !c.IsDeleted)
                .ToListAsync(ct);

            var needsAmount = request.TotalAmount * 0.50m;
            var wantsAmount = request.TotalAmount * 0.30m;

            var needsCats = expenseCategories.Where(c => c.BudgetRuleMapping == BudgetRuleMapping.Needs).ToList();
            var wantsCats = expenseCategories.Where(c => c.BudgetRuleMapping == BudgetRuleMapping.Wants).ToList();

            if (needsCats.Count > 0)
            {
                decimal perCat = Math.Round(needsAmount / needsCats.Count, 0);
                foreach (var c in needsCats) budget.AddAllocation(c.Id, perCat, 80m);
            }

            if (wantsCats.Count > 0)
            {
                decimal perCat = Math.Round(wantsAmount / wantsCats.Count, 0);
                foreach (var c in wantsCats) budget.AddAllocation(c.Id, perCat, 80m);
            }
        }

        // Close any currently active budget of the user that overlaps
        var activeBudgets = await _context.Budgets
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .ToListAsync(ct);

        foreach (var ab in activeBudgets)
        {
            ab.Close();
        }

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync(ct);

        return await GetBudgetByIdAsync(userId, budget.Id, ct);
    }

    public async Task<Result<BudgetDto>> GetActiveBudgetAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow.Date;

        var budget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        // Fallback: If no Active budget, look for the most recent budget
        budget ??= await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (budget == null)
            return Result<BudgetDto>.Failure("Hiện chưa có kế hoạch ngân sách nào.", 404);

        return Result<BudgetDto>.Success(MapToDto(budget));
    }

    public async Task<Result<List<BudgetSummaryDto>>> GetBudgetsAsync(Guid userId, CancellationToken ct = default)
    {
        var budgets = await _context.Budgets
            .Include(b => b.Allocations)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync(ct);

        var summaries = budgets.Select(b => new BudgetSummaryDto(
            b.Id,
            b.Name,
            b.TotalAmount,
            b.TotalSpent,
            b.TotalRemaining,
            b.OverallSpentPercent,
            b.Status.ToString()
        )).ToList();

        return Result<List<BudgetSummaryDto>>.Success(summaries);
    }

    public async Task<Result<BudgetDto>> GetBudgetByIdAsync(Guid userId, Guid budgetId, CancellationToken ct = default)
    {
        var budget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId, ct);

        if (budget == null)
            return Result<BudgetDto>.Failure("Không tìm thấy kế hoạch ngân sách yêu cầu.");

        return Result<BudgetDto>.Success(MapToDto(budget));
    }

    public async Task<Result<BudgetDto>> UpdateBudgetAsync(Guid userId, Guid budgetId, UpdateBudgetRequest request, CancellationToken ct = default)
    {
        var budget = await _context.Budgets
            .Include(b => b.Allocations)
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId, ct);

        if (budget == null)
            return Result<BudgetDto>.Failure("Không tìm thấy kế hoạch ngân sách.");

        budget.AdjustTotalBudget(request.TotalAmount);
        await _context.SaveChangesAsync(ct);

        return await GetBudgetByIdAsync(userId, budgetId, ct);
    }

    public async Task<Result<BudgetDto>> RebalanceBudgetAsync(Guid userId, Guid budgetId, RebalanceBudgetRequest request, CancellationToken ct = default)
    {
        var budget = await _context.Budgets
            .Include(b => b.Allocations)
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId, ct);

        if (budget == null)
            return Result<BudgetDto>.Failure("Không tìm thấy kế hoạch ngân sách.");

        foreach (var item in request.Rebalances)
        {
            var allocation = budget.Allocations.FirstOrDefault(a => a.CategoryId == item.CategoryId);
            if (allocation != null)
            {
                // Warning if new limit < current spent
                allocation.Rebalance(item.NewAllocatedAmount);
            }
        }

        await _context.SaveChangesAsync(ct);
        return await GetBudgetByIdAsync(userId, budgetId, ct);
    }

    public async Task<Result<bool>> CloseBudgetAsync(Guid userId, Guid budgetId, CancellationToken ct = default)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId, ct);

        if (budget == null)
            return Result<bool>.Failure("Không tìm thấy kế hoạch ngân sách.");

        budget.Close();
        await _context.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    public async Task RecordExpenseAgainstBudgetAsync(Guid userId, Guid categoryId, decimal amount, DateTime transactionDate, CancellationToken ct = default)
    {
        var date = transactionDate.Date;
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active && b.StartDate <= date && b.EndDate >= date)
            .FirstOrDefaultAsync(ct);

        if (activeBudget != null)
        {
            activeBudget.RecordExpense(categoryId, amount);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task RevertExpenseAgainstBudgetAsync(Guid userId, Guid categoryId, decimal amount, DateTime transactionDate, CancellationToken ct = default)
    {
        var date = transactionDate.Date;
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active && b.StartDate <= date && b.EndDate >= date)
            .FirstOrDefaultAsync(ct);

        if (activeBudget != null)
        {
            activeBudget.RevertExpense(categoryId, amount);
            await _context.SaveChangesAsync(ct);
        }
    }

    private static BudgetDto MapToDto(Budget budget)
    {
        var allocationDtos = budget.Allocations.Select(a => new BudgetAllocationDto(
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

        var periodName = budget.PeriodType switch
        {
            BudgetPeriodType.Monthly => "Hàng tháng",
            BudgetPeriodType.Weekly => "Hàng tuần",
            _ => "Chu kỳ tùy chỉnh"
        };

        var methodName = budget.Method switch
        {
            BudgetMethod.Rule503020 => "Quy tắc 50/30/20",
            BudgetMethod.ZeroBased => "Ngân sách số không (ZBB)",
            BudgetMethod.Kakeibo => "Sổ Kakeibo 4 Cột trụ",
            _ => "Tiêu chuẩn"
        };

        var statusName = budget.Status switch
        {
            BudgetStatus.Active => "Đang hoạt động",
            BudgetStatus.Draft => "Bản nháp",
            BudgetStatus.Closed => "Đã đóng",
            _ => "Không xác định"
        };

        return new BudgetDto(
            budget.Id,
            budget.UserId,
            budget.Name,
            (int)budget.PeriodType,
            periodName,
            budget.StartDate,
            budget.EndDate,
            budget.TotalAmount,
            (int)budget.Method,
            methodName,
            (int)budget.Status,
            statusName,
            budget.TotalAllocated,
            budget.TotalSpent,
            budget.TotalRemaining,
            budget.OverallSpentPercent,
            budget.TotalDays,
            allocationDtos
        );
    }
}
