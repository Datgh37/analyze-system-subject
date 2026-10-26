using ExpenseTracker.Maui.Models.AutoRules;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Production API repository connecting to backend ASP.NET Core WebApi
public class ApiAutoRuleRepository : IAutoRuleRepository
{
    private readonly IAutoRuleApi _api;

    public ApiAutoRuleRepository(IAutoRuleApi api)
    {
        _api = api;
    }

    public Task<List<AutoRuleModel>> GetRulesAsync()
        => _api.GetRulesAsync();

    public Task<AutoRuleModel> GetRuleByIdAsync(Guid id)
        => _api.GetRuleByIdAsync(id);

    public Task<AutoRuleModel> CreateRuleAsync(CreateAutoRuleModel request)
        => _api.CreateRuleAsync(request);

    public Task<AutoRuleModel> UpdateRuleAsync(Guid id, UpdateAutoRuleModel request)
        => _api.UpdateRuleAsync(id, request);

    public Task DeleteRuleAsync(Guid id)
        => _api.DeleteRuleAsync(id);

    public Task<MatchCategoryModel> MatchCategoryAsync(string text)
        => _api.MatchCategoryAsync(text);
}
