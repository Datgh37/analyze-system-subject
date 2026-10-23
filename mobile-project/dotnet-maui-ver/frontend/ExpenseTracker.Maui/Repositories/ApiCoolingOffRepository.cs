using ExpenseTracker.Maui.Models.Behavioral;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

public class ApiCoolingOffRepository : ICoolingOffRepository
{
    private readonly ICoolingOffApi _api;

    public ApiCoolingOffRepository(ICoolingOffApi api)
    {
        _api = api;
    }

    public async Task<CoolingOffEvaluationResultModel> EvaluateExpenseAsync(CoolingOffEvaluationRequestModel request)
    {
        return await _api.EvaluateExpenseAsync(request);
    }

    public async Task<CoolingOffTransactionModel> CreateCoolingOffAsync(CreateCoolingOffRequestModel request)
    {
        return await _api.CreateCoolingOffAsync(request);
    }

    public async Task<List<CoolingOffTransactionModel>> GetPendingCoolingOffAsync()
    {
        return await _api.GetPendingCoolingOffAsync();
    }

    public async Task<bool> ConfirmPurchaseAsync(Guid id)
    {
        return await _api.ConfirmPurchaseAsync(id);
    }

    public async Task<bool> CancelPurchaseAsync(Guid id)
    {
        return await _api.CancelPurchaseAsync(id);
    }

    public async Task<bool> BypassCoolingOffAsync(Guid id, BypassCoolingOffRequestModel request)
    {
        return await _api.BypassCoolingOffAsync(id, request);
    }
}
