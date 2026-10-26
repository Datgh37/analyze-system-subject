using ExpenseTracker.Maui.Models.AutoRules;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for automatic classification rules (Clean MVVM Architecture)
public interface IAutoRuleRepository
{
    Task<List<AutoRuleModel>> GetRulesAsync();
    Task<AutoRuleModel> GetRuleByIdAsync(Guid id);
    Task<AutoRuleModel> CreateRuleAsync(CreateAutoRuleModel request);
    Task<AutoRuleModel> UpdateRuleAsync(Guid id, UpdateAutoRuleModel request);
    Task DeleteRuleAsync(Guid id);
    Task<MatchCategoryModel> MatchCategoryAsync(string text);
}
