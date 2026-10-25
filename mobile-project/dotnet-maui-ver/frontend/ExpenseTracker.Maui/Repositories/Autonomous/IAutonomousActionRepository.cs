using ExpenseTracker.Maui.Models.Autonomous;

namespace ExpenseTracker.Maui.Repositories.Autonomous;

// Repository interface for Server-Driven Action Protocol (Chặng 10.5 / PLAN-47)
public interface IAutonomousActionRepository
{
    Task<AutonomousActionInboxResponseModel> GetInboxAsync();
    Task<AutonomousActionModel> ProcessNotificationAsync(RawBankNotificationRequestModel request);
    Task<AutonomousActionModel> QuickCommitAsync(QuickCommitActionRequestModel request);
    Task<bool> DismissActionAsync(string actionId);
}
