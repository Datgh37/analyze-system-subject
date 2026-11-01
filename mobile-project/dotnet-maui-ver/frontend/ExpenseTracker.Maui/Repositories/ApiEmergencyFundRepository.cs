using ExpenseTracker.Maui.Models.SavingsGoals;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

public class ApiEmergencyFundRepository : IEmergencyFundRepository
{
    private readonly IEmergencyFundApi _api;

    public ApiEmergencyFundRepository(IEmergencyFundApi api)
    {
        _api = api;
    }

    public Task<EmergencyFundRecommendationModel> GetRecommendationAsync()
        => _api.GetRecommendationAsync();

    public Task<EmergencyFundStatusModel> GetStatusAsync()
        => _api.GetStatusAsync();

    public Task<EmergencyFundStatusModel> SetupFundAsync(SetupEmergencyFundRequest request)
        => _api.SetupFundAsync(request);

    public Task<EmergencyFundStatusModel> DepositAsync(DepositEmergencyFundRequest request)
        => _api.DepositAsync(request);

    public Task<EmergencyFundStatusModel> WithdrawAsync(WithdrawEmergencyFundRequest request)
        => _api.WithdrawAsync(request);
}

