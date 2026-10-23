namespace ExpenseTracker.Application.DTOs.AutoRules;

// DTO representing an auto-categorization rule (UC-04.07)
public record AutoRuleDto(
    Guid Id,
    Guid UserId,
    string KeywordPattern,
    Guid TargetCategoryId,
    string TargetCategoryName,
    int PriorityOrder,
    bool IsRegex,
    bool IsCaseSensitive,
    DateTime CreatedAt
);

// Request to create a new auto-categorization rule
public record CreateAutoRuleRequest(
    string KeywordPattern,
    Guid TargetCategoryId,
    int PriorityOrder = 0,
    bool IsRegex = false,
    bool IsCaseSensitive = false
);

// Request to update an existing auto-categorization rule
public record UpdateAutoRuleRequest(
    string KeywordPattern,
    Guid TargetCategoryId,
    int PriorityOrder,
    bool IsRegex,
    bool IsCaseSensitive
);

// Result of evaluating a transaction note or line item against user rules
public record MatchCategoryResponse(
    Guid? CategoryId,
    string? CategoryName,
    string? MatchedPattern
);
