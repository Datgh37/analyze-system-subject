using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service contract for budget creation, allocation, rebalancing, and ledger synchronization (UC-05.01 - 05.04)
public interface IBudgetService
{
    Task<Result<BudgetDto>> CreateBudgetAsync(Guid userId, CreateBudgetRequest request, CancellationToken ct = default);
    Task<Result<BudgetDto>> GetActiveBudgetAsync(Guid userId, CancellationToken ct = default);
    Task<Result<List<BudgetSummaryDto>>> GetBudgetsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<BudgetDto>> GetBudgetByIdAsync(Guid userId, Guid budgetId, CancellationToken ct = default);
    Task<Result<BudgetDto>> UpdateBudgetAsync(Guid userId, Guid budgetId, UpdateBudgetRequest request, CancellationToken ct = default);
    Task<Result<BudgetDto>> RebalanceBudgetAsync(Guid userId, Guid budgetId, RebalanceBudgetRequest request, CancellationToken ct = default);
    Task<Result<bool>> CloseBudgetAsync(Guid userId, Guid budgetId, CancellationToken ct = default);
    
    // Cross-module ledger synchronization called when transactions are committed
    Task RecordExpenseAgainstBudgetAsync(Guid userId, Guid categoryId, decimal amount, DateTime transactionDate, CancellationToken ct = default);
    Task RevertExpenseAgainstBudgetAsync(Guid userId, Guid categoryId, decimal amount, DateTime transactionDate, CancellationToken ct = default);
}
