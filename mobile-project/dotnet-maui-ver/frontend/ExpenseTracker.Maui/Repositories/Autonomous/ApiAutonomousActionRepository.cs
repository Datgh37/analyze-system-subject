using ExpenseTracker.Maui.Models.Autonomous;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories.Autonomous;

// Production Refit implementation of IAutonomousActionRepository
public class ApiAutonomousActionRepository : IAutonomousActionRepository
{
    private readonly IAutonomousActionsApi _api;

    public ApiAutonomousActionRepository(IAutonomousActionsApi api)
    {
        _api = api;
    }

    public Task<AutonomousActionInboxResponseModel> GetInboxAsync()
    {
        return _api.GetInboxAsync();
    }

    public Task<AutonomousActionModel> ProcessNotificationAsync(RawBankNotificationRequestModel request)
    {
        return _api.ProcessNotificationAsync(request);
    }

    public Task<AutonomousActionModel> QuickCommitAsync(QuickCommitActionRequestModel request)
    {
        return _api.QuickCommitAsync(request);
    }

    public Task<bool> DismissActionAsync(string actionId)
    {
        return _api.DismissActionAsync(actionId);
    }
}
