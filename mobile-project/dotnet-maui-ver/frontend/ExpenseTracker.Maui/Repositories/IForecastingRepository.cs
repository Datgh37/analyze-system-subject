using ExpenseTracker.Maui.Models.Forecasting;

namespace ExpenseTracker.Maui.Repositories;

// Clean MVVM Repository Contract for Financial Forecasting & Deficit Radar (UC-08.01 to UC-08.04)
public interface IForecastingRepository
{
    Task<ExpenseForecastModel> GetExpenditureForecastAsync();
    Task<LiquidityDepletionModel> GetLiquidityDepletionAsync();
    Task<List<CategoryDeficitRiskModel>> GetCategoryRisksAsync();
    Task<WhatIfSimulationResultModel> SimulateWhatIfAsync(WhatIfSimulationRequestModel request);
    Task<ConvertToSavingsGoalResultModel> ConvertWhatIfToGoalAsync(ConvertToSavingsGoalRequestModel request);
}
