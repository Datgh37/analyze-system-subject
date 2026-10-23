using ExpenseTracker.Maui.Models.Budgets;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for budget and Adaptive DSA operations (Clean MVVM Architecture)
public interface IBudgetRepository
{
    Task<BudgetModel?> GetActiveBudgetAsync();
    Task<List<BudgetSummaryModel>> GetBudgetsAsync();
    Task<BudgetModel> GetBudgetByIdAsync(Guid id);
    Task<BudgetModel> CreateBudgetAsync(CreateBudgetModel request);
    Task CloseBudgetAsync(Guid id);
    Task<DsaStatusModel> GetCurrentDsaStatusAsync();
    Task<VelocityStatusModel> GetSpendingVelocityAsync();
    Task<BudgetProgressModel> GetBudgetProgressOverviewAsync();
}
