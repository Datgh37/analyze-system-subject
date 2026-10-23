using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service contract for Adaptive Daily Spending Allowance (DSA) and Spending Velocity Tracker (UC-05.05/05.06)
public interface IAdaptiveDsaService
{
    Task<Result<DsaStatusDto>> GetCurrentDsaStatusAsync(Guid userId, CancellationToken ct = default);
    Task<Result<VelocityStatusDto>> GetSpendingVelocityAsync(Guid userId, CancellationToken ct = default);
    Task<Result<BudgetProgressDto>> GetBudgetProgressOverviewAsync(Guid userId, CancellationToken ct = default);
    Task RecalculateDsaForTodayAsync(Guid budgetId, CancellationToken ct = default);
}
