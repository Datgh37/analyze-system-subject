using ExpenseTracker.Application.DTOs.AutoRules;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service interface managing automatic categorization rules and keyword pattern matching (UC-04.07)
public interface IAutoCategorizationService
{
    Task<Result<List<AutoRuleDto>>> GetRulesAsync(Guid userId, CancellationToken ct = default);
    Task<Result<AutoRuleDto>> GetRuleByIdAsync(Guid userId, Guid ruleId, CancellationToken ct = default);
    Task<Result<AutoRuleDto>> CreateRuleAsync(Guid userId, CreateAutoRuleRequest request, CancellationToken ct = default);
    Task<Result<AutoRuleDto>> UpdateRuleAsync(Guid userId, Guid ruleId, UpdateAutoRuleRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteRuleAsync(Guid userId, Guid ruleId, CancellationToken ct = default);
    Task<Result<MatchCategoryResponse>> MatchCategoryAsync(Guid userId, string text, CancellationToken ct = default);
}
