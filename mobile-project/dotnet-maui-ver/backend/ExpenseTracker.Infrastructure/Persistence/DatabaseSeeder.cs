using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Persistence;

// Seeds default Vietnamese financial categories, admin user, and provides database reset utility
public static class DatabaseSeeder
{
    public const string DefaultAdminEmail = "admin@expensetracker.com";
    public const string DefaultAdminPassword = "Admin@123";

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

    public static async Task<User> SeedAdminUserAsync(
        ApplicationDbContext context, 
        IPasswordHasher passwordHasher, 
        ILogger logger, 
        CancellationToken cancellationToken = default)
    {
        var existingAdmin = await context.Users
            .Include(u => u.Wallets)
            .FirstOrDefaultAsync(u => u.Email == DefaultAdminEmail, cancellationToken);

        if (existingAdmin != null)
        {
            return existingAdmin;
        }

        logger.LogInformation("Seeding default Administrator user ({Email})...", DefaultAdminEmail);

        var passwordHash = passwordHasher.HashPassword(DefaultAdminPassword);
        var adminUser = new User(
            fullName: "Quản Trị Viên (Admin)",
            email: DefaultAdminEmail,
            passwordHash: passwordHash,
            baseCurrency: "VND"
        );

        await context.Users.AddAsync(adminUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Seed 2 default wallets for Admin
        var cashWallet = new Wallet(
            adminUser.Id,
            name: "Ví Tiền Mặt",
            walletType: WalletType.Cash,
            initialBalance: 5_000_000m,
            currency: "VND",
            colorHex: "#10B981",
            iconKey: "cash"
        );

        var bankWallet = new Wallet(
            adminUser.Id,
            name: "Tài Khoản Vietcombank",
            walletType: WalletType.BankAccount,
            initialBalance: 25_000_000m,
            currency: "VND",
            accountNumber: "9988776655",
            bankName: "Vietcombank",
            colorHex: "#0066FF",
            iconKey: "building-columns"
        );

        await context.Wallets.AddRangeAsync(new[] { cashWallet, bankWallet }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Seed sample financial transactions for immediate dashboard demo
        var foodCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Ăn uống & Thực phẩm" && c.IsSystem, cancellationToken);
        var transportCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Đi lại & Xăng xe" && c.IsSystem, cancellationToken);
        var salaryCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Tiền lương" && c.IsSystem, cancellationToken);

        if (foodCategory != null && transportCategory != null && salaryCategory != null)
        {
            var now = DateTime.UtcNow;

            var salaryTx = new Transaction(
                adminUser.Id,
                bankWallet.Id,
                salaryCategory.Id,
                amount: 30_000_000m,
                transactionType: TransactionType.Income,
                transactionDate: now.AddDays(-5),
                note: "Lương tháng chuyển khoản"
            );

            var foodTx = new Transaction(
                adminUser.Id,
                cashWallet.Id,
                foodCategory.Id,
                amount: 75_000m,
                transactionType: TransactionType.Expense,
                transactionDate: now.AddDays(-1),
                note: "Ăn trưa cơm văn phòng",
                isEssential: true
            );

            var transportTx = new Transaction(
                adminUser.Id,
                cashWallet.Id,
                transportCategory.Id,
                amount: 90_000m,
                transactionType: TransactionType.Expense,
                transactionDate: now,
                note: "Đổ xăng xe máy",
                isEssential: true
            );

            await context.Transactions.AddRangeAsync(new[] { salaryTx, foodTx, transportTx }, cancellationToken);

            // Seed sample Monthly Budget (50/30/20) for current month
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var sampleBudget = new Budget(
                adminUser.Id,
                name: $"Ngân sách tháng {now.Month}/{now.Year}",
                totalAmount: 15_000_000m,
                startDate: startOfMonth,
                endDate: endOfMonth,
                periodType: BudgetPeriodType.Monthly,
                method: BudgetMethod.Rule503020
            );

            sampleBudget.AddAllocation(foodCategory.Id, 5_000_000m, 80m);
            sampleBudget.AddAllocation(transportCategory.Id, 2_000_000m, 80m);

            await context.Budgets.AddAsync(sampleBudget, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("Successfully seeded Admin user, default wallets, and sample financial ledger records.");
        return adminUser;
    }

    public static async Task ResetDatabaseAsync(
        ApplicationDbContext context, 
        IPasswordHasher passwordHasher, 
        ILogger logger, 
        CancellationToken cancellationToken = default)
    {
        logger.LogWarning("Initiating Database Reset to fresh seed state...");

        // Ensure database schema migrations are applied
        await context.Database.MigrateAsync(cancellationToken);

        // Truncate all application data tables using PostgreSQL CASCADE
        var truncateSql = @"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (
                    SELECT tablename 
                    FROM pg_tables 
                    WHERE schemaname = current_schema() AND tablename <> '__EFMigrationsHistory'
                ) LOOP
                    EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE;';
                END LOOP;
            END $$;";

        await context.Database.ExecuteSqlRawAsync(truncateSql, cancellationToken);
        logger.LogInformation("All application tables truncated successfully.");

        // Re-seed system categories and admin user
        await SeedDefaultCategoriesAsync(context, logger, cancellationToken);
        await SeedAdminUserAsync(context, passwordHasher, logger, cancellationToken);

        logger.LogInformation("Database reset and re-seeding completed successfully!");
    }
}
