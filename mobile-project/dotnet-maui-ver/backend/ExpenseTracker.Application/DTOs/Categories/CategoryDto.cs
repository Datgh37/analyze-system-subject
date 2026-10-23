using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Categories;

// DTO representing category details and recursive hierarchical child branches
public record CategoryDto(
    Guid Id,
    Guid? UserId,
    string Name,
    TransactionType TransactionType,
    Guid? ParentId,
    string? ParentName,
    string IconKey,
    string ColorHex,
    int DisplayOrder,
    bool IsSystem,
    IncomeCategoryGroup? IncomeGroup,
    BudgetRuleMapping? BudgetRuleMapping,
    List<CategoryDto>? SubCategories = null
);
