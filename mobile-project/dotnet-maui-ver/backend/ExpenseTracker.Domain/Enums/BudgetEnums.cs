namespace ExpenseTracker.Domain.Enums;

// Period cycle for financial budgeting (UC-05.01)
public enum BudgetPeriodType
{
    Monthly = 1,
    Weekly = 2
}

// Behavioral budgeting methodologies (UC-05.01)
public enum BudgetMethod
{
    Rule503020 = 1, // 50% Needs, 30% Wants, 20% Savings
    ZeroBased = 2,  // Zero-Based Budgeting (ZBB: Income - Expenses = 0)
    Kakeibo = 3     // Kakeibo Japanese 4 Pillars (Survival, Optional, Culture, Extra)
}

// Lifecycle status of a budget plan
public enum BudgetStatus
{
    Draft = 1,
    Active = 2,
    Closed = 3
}
