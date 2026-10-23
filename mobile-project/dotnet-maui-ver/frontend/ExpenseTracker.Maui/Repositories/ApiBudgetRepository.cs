using ExpenseTracker.Maui.Models.Budgets;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Live API implementation of IBudgetRepository delegating to Refit IBudgetApi
public class ApiBudgetRepository : IBudgetRepository
{
    private readonly IBudgetApi _budgetApi;

    public ApiBudgetRepository(IBudgetApi budgetApi)
    {
        _budgetApi = budgetApi;
    }

    public Task<BudgetModel?> GetActiveBudgetAsync() => _budgetApi.GetActiveBudgetAsync()!;
    public Task<List<BudgetSummaryModel>> GetBudgetsAsync() => _budgetApi.GetBudgetsAsync();
    public Task<BudgetModel> GetBudgetByIdAsync(Guid id) => _budgetApi.GetBudgetByIdAsync(id);
    public Task<BudgetModel> CreateBudgetAsync(CreateBudgetModel request) => _budgetApi.CreateBudgetAsync(request);
    public Task CloseBudgetAsync(Guid id) => _budgetApi.CloseBudgetAsync(id);
    public Task<DsaStatusModel> GetCurrentDsaStatusAsync() => _budgetApi.GetCurrentDsaStatusAsync();
    public Task<VelocityStatusModel> GetSpendingVelocityAsync() => _budgetApi.GetSpendingVelocityAsync();
    public Task<BudgetProgressModel> GetBudgetProgressOverviewAsync() => _budgetApi.GetBudgetProgressOverviewAsync();
}
