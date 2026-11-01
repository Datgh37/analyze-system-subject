using ExpenseTracker.Maui.Models.SavingsGoals;

namespace ExpenseTracker.Maui.Repositories;

public interface IEmergencyFundRepository
{
    Task<EmergencyFundRecommendationModel> GetRecommendationAsync();
    Task<EmergencyFundStatusModel> GetStatusAsync();
    Task<EmergencyFundStatusModel> SetupFundAsync(SetupEmergencyFundRequest request);
    Task<EmergencyFundStatusModel> DepositAsync(DepositEmergencyFundRequest request);
    Task<EmergencyFundStatusModel> WithdrawAsync(WithdrawEmergencyFundRequest request);
}

