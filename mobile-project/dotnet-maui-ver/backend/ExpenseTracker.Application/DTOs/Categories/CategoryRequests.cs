using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Categories;

// Request to create a new user-defined category
public record CreateCategoryRequest(
    string Name,
    TransactionType TransactionType,
    Guid? ParentId = null,
    string IconKey = "folder",
    string ColorHex = "#10B981",
    int DisplayOrder = 0,
    IncomeCategoryGroup? IncomeGroup = null,
    BudgetRuleMapping? BudgetRuleMapping = null
);

// Request to update an existing category
public record UpdateCategoryRequest(
    string Name,
    Guid? ParentId = null,
    string IconKey = "folder",
    string ColorHex = "#10B981",
    int DisplayOrder = 0,
    IncomeCategoryGroup? IncomeGroup = null,
    BudgetRuleMapping? BudgetRuleMapping = null
);
