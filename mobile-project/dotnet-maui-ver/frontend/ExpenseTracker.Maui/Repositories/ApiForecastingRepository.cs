using ExpenseTracker.Maui.Models.Forecasting;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Real WebApi implementation for Financial Forecasting repository
public class ApiForecastingRepository : IForecastingRepository
{
    private readonly IForecastingApi _api;

    public ApiForecastingRepository(IForecastingApi api)
    {
        _api = api;
    }

    public async Task<ExpenseForecastModel> GetExpenditureForecastAsync()
    {
        return await _api.GetExpenditureForecastAsync();
    }

    public async Task<LiquidityDepletionModel> GetLiquidityDepletionAsync()
    {
        return await _api.GetLiquidityDepletionAsync();
    }

    public async Task<List<CategoryDeficitRiskModel>> GetCategoryRisksAsync()
    {
        return await _api.GetCategoryRisksAsync();
    }

    public async Task<WhatIfSimulationResultModel> SimulateWhatIfAsync(WhatIfSimulationRequestModel request)
    {
        return await _api.SimulateWhatIfAsync(request);
    }

    public async Task<ConvertToSavingsGoalResultModel> ConvertWhatIfToGoalAsync(ConvertToSavingsGoalRequestModel request)
    {
        return await _api.ConvertWhatIfToGoalAsync(request);
    }
}
