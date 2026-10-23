namespace ExpenseTracker.Maui.Models.AutoRules;

public record AutoRuleModel(
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

public record CreateAutoRuleModel(
    string KeywordPattern,
    Guid TargetCategoryId,
    int PriorityOrder = 0,
    bool IsRegex = false,
    bool IsCaseSensitive = false
);

public record UpdateAutoRuleModel(
    string KeywordPattern,
    Guid TargetCategoryId,
    int PriorityOrder,
    bool IsRegex,
    bool IsCaseSensitive
);

public record MatchCategoryModel(
    Guid? CategoryId,
    string? CategoryName,
    string? MatchedPattern
);
