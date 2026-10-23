namespace ExpenseTracker.Domain.Enums;

// Maps categories to behavioral budgeting methodologies (50/30/20 & Kakeibo)
public enum BudgetRuleMapping
{
    Needs = 1,               // Nhu cầu thiết yếu (50%)
    Wants = 2,               // Mong muốn & Hưởng thụ (30%)
    SavingsInvestments = 3,  // Tiết kiệm & Tích lũy (20%)
    IncomeSource = 4         // Nguồn thu nhập (Dùng cho Category loại Income)
}
