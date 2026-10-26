using ExpenseTracker.Maui.Models.SmartUtilities;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// API implementation of smart financial utilities repository delegating to Refit ISmartUtilityApi
public class ApiSmartUtilityRepository : ISmartUtilityRepository
{
    private readonly ISmartUtilityApi _api;

    public ApiSmartUtilityRepository(ISmartUtilityApi api)
    {
        _api = api;
    }

    public Task<SubscriptionAnalysisResultModel> DetectSubscriptionsAsync() => _api.DetectSubscriptionsAsync();
    public Task<VietQrResultModel> GenerateVietQrAsync(GenerateVietQrModel request) => _api.GenerateVietQrAsync(request);
    public Task<ParsedVoiceExpenseModel> ParseVoiceExpenseAsync(ParseVoiceExpenseModel request) => _api.ParseVoiceExpenseAsync(request);
}
