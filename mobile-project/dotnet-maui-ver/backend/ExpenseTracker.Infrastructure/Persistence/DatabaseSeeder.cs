using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Persistence;

// Seeds default Vietnamese financial categories and system taxonomies
public static class DatabaseSeeder
{
    public static async Task SeedDefaultCategoriesAsync(ApplicationDbContext context, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (await context.Categories.AnyAsync(c => c.IsSystem, cancellationToken))
        {
            return;
        }

        logger.LogInformation("Seeding default system financial categories for Vietnamese personal finance...");

        var defaultCategories = new List<Category>
        {
            // Income Categories
            new Category("Tiền lương", TransactionType.Income, isSystem: true, iconKey: "briefcase", colorHex: "#10B981", displayOrder: 1, incomeGroup: IncomeCategoryGroup.Active, budgetRuleMapping: BudgetRuleMapping.IncomeSource),
            new Category("Tiền thưởng", TransactionType.Income, isSystem: true, iconKey: "gift", colorHex: "#059669", displayOrder: 2, incomeGroup: IncomeCategoryGroup.Active, budgetRuleMapping: BudgetRuleMapping.IncomeSource),
            new Category("Làm thêm & Freelance", TransactionType.Income, isSystem: true, iconKey: "laptop", colorHex: "#34D399", displayOrder: 3, incomeGroup: IncomeCategoryGroup.Active, budgetRuleMapping: BudgetRuleMapping.IncomeSource),
            new Category("Lãi đầu tư & Cổ tức", TransactionType.Income, isSystem: true, iconKey: "trending-up", colorHex: "#047857", displayOrder: 4, incomeGroup: IncomeCategoryGroup.Passive, budgetRuleMapping: BudgetRuleMapping.IncomeSource),
            new Category("Lãi tiết kiệm", TransactionType.Income, isSystem: true, iconKey: "piggy-bank", colorHex: "#065F46", displayOrder: 5, incomeGroup: IncomeCategoryGroup.Passive, budgetRuleMapping: BudgetRuleMapping.IncomeSource),
            new Category("Thu nhập khác", TransactionType.Income, isSystem: true, iconKey: "plus-circle", colorHex: "#6EE7B7", displayOrder: 6, incomeGroup: IncomeCategoryGroup.Other, budgetRuleMapping: BudgetRuleMapping.IncomeSource),

            // Core Expense Categories (Needs, Wants, Savings)
            new Category("Ăn uống & Thực phẩm", TransactionType.Expense, isSystem: true, iconKey: "utensils", colorHex: "#EF4444", displayOrder: 1, budgetRuleMapping: BudgetRuleMapping.Needs),
            new Category("Nhà ở & Tiện ích", TransactionType.Expense, isSystem: true, iconKey: "home", colorHex: "#DC2626", displayOrder: 2, budgetRuleMapping: BudgetRuleMapping.Needs),
            new Category("Đi lại & Xăng xe", TransactionType.Expense, isSystem: true, iconKey: "car", colorHex: "#B91C1C", displayOrder: 3, budgetRuleMapping: BudgetRuleMapping.Needs),
            new Category("Y tế & Sức khỏe", TransactionType.Expense, isSystem: true, iconKey: "heart-pulse", colorHex: "#991B1B", displayOrder: 4, budgetRuleMapping: BudgetRuleMapping.Needs),
            new Category("Mua sắm cá nhân", TransactionType.Expense, isSystem: true, iconKey: "shopping-bag", colorHex: "#F59E0B", displayOrder: 5, budgetRuleMapping: BudgetRuleMapping.Wants),
            new Category("Giải trí & Du lịch", TransactionType.Expense, isSystem: true, iconKey: "film", colorHex: "#D97706", displayOrder: 6, budgetRuleMapping: BudgetRuleMapping.Wants),
            new Category("Giáo dục & Học tập", TransactionType.Expense, isSystem: true, iconKey: "book-open", colorHex: "#3B82F6", displayOrder: 7, budgetRuleMapping: BudgetRuleMapping.SavingsInvestments),
            new Category("Tiết kiệm & Đầu tư", TransactionType.Expense, isSystem: true, iconKey: "wallet", colorHex: "#2563EB", displayOrder: 8, budgetRuleMapping: BudgetRuleMapping.SavingsInvestments)
        };

        await context.Categories.AddRangeAsync(defaultCategories, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully seeded {Count} default system categories.", defaultCategories.Count);
    }
}
