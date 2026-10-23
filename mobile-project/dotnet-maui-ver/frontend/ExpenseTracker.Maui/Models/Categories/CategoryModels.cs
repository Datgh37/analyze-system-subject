namespace ExpenseTracker.Maui.Models.Categories;

// Category presentation model matching backend DTO
public record CategoryModel(
    Guid Id,
    Guid? UserId,
    string Name,
    int TransactionType,
    Guid? ParentId,
    string? ParentName,
    string IconKey,
    string ColorHex,
    int DisplayOrder,
    bool IsSystem,
    int? IncomeGroup,
    int? BudgetRuleMapping,
    List<CategoryModel>? SubCategories = null
);

public record CreateCategoryModel(
    string Name,
    int TransactionType,
    Guid? ParentId = null,
    string IconKey = "folder",
    string ColorHex = "#10B981",
    int DisplayOrder = 0,
    int? IncomeGroup = null,
    int? BudgetRuleMapping = null
);

public record UpdateCategoryModel(
    string Name,
    Guid? ParentId = null,
    string IconKey = "folder",
    string ColorHex = "#10B981",
    int DisplayOrder = 0,
    int? IncomeGroup = null,
    int? BudgetRuleMapping = null
);
