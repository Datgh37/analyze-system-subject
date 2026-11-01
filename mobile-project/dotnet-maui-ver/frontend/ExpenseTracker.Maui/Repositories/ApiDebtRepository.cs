using ExpenseTracker.Maui.Models.Debts;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Triển khai Repository kết nối trực tiếp ASP.NET Core WebApi qua Refit Client
public class ApiDebtRepository : IDebtRepository
{
    private readonly IDebtsApi _debtsApi;

    public ApiDebtRepository(IDebtsApi debtsApi)
    {
        _debtsApi = debtsApi;
    }

    public async Task<List<DebtModel>> GetDebtsAsync()
    {
        return await _debtsApi.GetDebtsAsync();
    }

    public async Task<DebtDetailModel?> GetDebtByIdAsync(Guid id)
    {
        return await _debtsApi.GetDebtByIdAsync(id);
    }

    public async Task<DebtSummaryModel> GetDebtSummaryAsync()
    {
        return await _debtsApi.GetDebtSummaryAsync();
    }

    public async Task<DebtModel> CreateDebtAsync(CreateDebtRequest request)
    {
        return await _debtsApi.CreateDebtAsync(request);
    }

    public async Task<DebtModel> UpdateDebtAsync(Guid id, UpdateDebtRequest request)
    {
        return await _debtsApi.UpdateDebtAsync(id, request);
    }

    public async Task<DebtTransactionModel> RecordPaymentAsync(Guid id, RecordDebtPaymentRequest request)
    {
        return await _debtsApi.RecordPaymentAsync(id, request);
    }

    public async Task<bool> ArchiveDebtAsync(Guid id)
    {
        await _debtsApi.ArchiveDebtAsync(id);
        return true;
    }

    public async Task<DebtEducationalGuideModel> GetEducationalGuideAsync()
    {
        return await _debtsApi.GetEducationalGuideAsync();
    }

    public async Task<SafeExtraBudgetRecommendationModel> GetSafeBudgetRecommendationAsync()
    {
        return await _debtsApi.GetSafeBudgetRecommendationAsync();
    }

    public async Task<DebtPayoffComparisonModel> CompareStrategiesAsync(decimal extraBudget, List<DebtSimulationItemModel>? customDebts = null)
    {
        return await _debtsApi.CompareStrategiesAsync(new DebtPayoffSimulationRequest
        {
            MonthlyExtraBudget = extraBudget,
            CustomDebts = customDebts
        });
    }

    public async Task<DebtStrategySimulationResultModel> SimulateStrategyAsync(string strategy, decimal extraBudget, List<DebtSimulationItemModel>? customDebts = null)
    {
        return await _debtsApi.SimulateStrategyAsync(strategy, new DebtPayoffSimulationRequest
        {
            MonthlyExtraBudget = extraBudget,
            CustomDebts = customDebts
        });
    }
}

