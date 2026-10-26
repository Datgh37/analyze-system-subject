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

    public const string DefaultDemoEmail = "demo@expensetracker.vn";
    public const string DefaultDemoPassword = "Demo@123";

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

    public static async Task<User> SeedDemoStoryAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Seeding master demo financial storyline for user ({Email})...", DefaultDemoEmail);

        // Ensure default system categories exist
        await SeedDefaultCategoriesAsync(context, logger, cancellationToken);

        // Check if demo user already exists
        var existingDemoUser = await context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Wallets)
            .FirstOrDefaultAsync(u => u.Email == DefaultDemoEmail, cancellationToken);

        User demoUser;
        if (existingDemoUser == null)
        {
            var passwordHash = passwordHasher.HashPassword(DefaultDemoPassword);
            demoUser = new User(
                fullName: "Trịnh Thành Đạt",
                email: DefaultDemoEmail,
                passwordHash: passwordHash,
                baseCurrency: "VND"
            );
            await context.Users.AddAsync(demoUser, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            demoUser = existingDemoUser;
            demoUser.Restore();
            demoUser.UpdateProfile("Trịnh Thành Đạt", "VND");

            var userId = demoUser.Id;

            // Clean child entities for clean idempotent re-seed
            var receipts = await context.ReceiptItems.IgnoreQueryFilters().Where(r => r.Transaction!.UserId == userId).ToListAsync(cancellationToken);
            context.ReceiptItems.RemoveRange(receipts);

            var salaryDetails = await context.SalarySlipDetails.IgnoreQueryFilters().Where(s => s.UserId == userId).ToListAsync(cancellationToken);
            context.SalarySlipDetails.RemoveRange(salaryDetails);

            var transactions = await context.Transactions.IgnoreQueryFilters().Where(t => t.UserId == userId).ToListAsync(cancellationToken);
            context.Transactions.RemoveRange(transactions);

            var transfers = await context.WalletTransfers.IgnoreQueryFilters().Where(w => w.UserId == userId).ToListAsync(cancellationToken);
            context.WalletTransfers.RemoveRange(transfers);

            var coolingOff = await context.CoolingOffTransactions.IgnoreQueryFilters().Where(c => c.UserId == userId).ToListAsync(cancellationToken);
            context.CoolingOffTransactions.RemoveRange(coolingOff);

            var nudges = await context.FinancialNudges.IgnoreQueryFilters().Where(n => n.UserId == userId).ToListAsync(cancellationToken);
            context.FinancialNudges.RemoveRange(nudges);

            var notifications = await context.InAppNotifications.IgnoreQueryFilters().Where(n => n.UserId == userId).ToListAsync(cancellationToken);
            context.InAppNotifications.RemoveRange(notifications);

            var tokens = await context.DevicePushTokens.IgnoreQueryFilters().Where(d => d.UserId == userId).ToListAsync(cancellationToken);
            context.DevicePushTokens.RemoveRange(tokens);

            var refreshTokens = await context.RefreshTokens.IgnoreQueryFilters().Where(r => r.UserId == userId).ToListAsync(cancellationToken);
            context.RefreshTokens.RemoveRange(refreshTokens);

            var contributions = await context.SavingsGoalContributions.IgnoreQueryFilters().Where(c => c.SavingsGoal!.UserId == userId).ToListAsync(cancellationToken);
            context.SavingsGoalContributions.RemoveRange(contributions);

            var goals = await context.SavingsGoals.IgnoreQueryFilters().Where(g => g.UserId == userId).ToListAsync(cancellationToken);
            context.SavingsGoals.RemoveRange(goals);

            var dsas = await context.DailySpendingAllowances.IgnoreQueryFilters().Where(d => d.Budget!.UserId == userId).ToListAsync(cancellationToken);
            context.DailySpendingAllowances.RemoveRange(dsas);

            var allocations = await context.BudgetAllocations.IgnoreQueryFilters().Where(b => b.Budget!.UserId == userId).ToListAsync(cancellationToken);
            context.BudgetAllocations.RemoveRange(allocations);

            var budgets = await context.Budgets.IgnoreQueryFilters().Where(b => b.UserId == userId).ToListAsync(cancellationToken);
            context.Budgets.RemoveRange(budgets);

            var adjustments = await context.BalanceAdjustments.IgnoreQueryFilters().Where(b => b.Wallet!.UserId == userId).ToListAsync(cancellationToken);
            context.BalanceAdjustments.RemoveRange(adjustments);

            var wallets = await context.Wallets.IgnoreQueryFilters().Where(w => w.UserId == userId).ToListAsync(cancellationToken);
            context.Wallets.RemoveRange(wallets);

            var rules = await context.AutoCategorizationRules.IgnoreQueryFilters().Where(a => a.UserId == userId).ToListAsync(cancellationToken);
            context.AutoCategorizationRules.RemoveRange(rules);

            var recurring = await context.RecurringIncomes.IgnoreQueryFilters().Where(r => r.UserId == userId).ToListAsync(cancellationToken);
            context.RecurringIncomes.RemoveRange(recurring);

            var snapshots = await context.NetWorthSnapshots.IgnoreQueryFilters().Where(n => n.UserId == userId).ToListAsync(cancellationToken);
            context.NetWorthSnapshots.RemoveRange(snapshots);

            await context.SaveChangesAsync(cancellationToken);
        }

        var now = DateTime.UtcNow;

        // 1. Wallets
        var bankWallet = new Wallet(
            demoUser.Id,
            name: "Tài Khoản Vietcombank",
            walletType: WalletType.BankAccount,
            initialBalance: 15_500_000m,
            currency: "VND",
            accountNumber: "007100889988",
            bankName: "Vietcombank",
            colorHex: "#0066FF",
            iconKey: "building-columns"
        );

        var cashWallet = new Wallet(
            demoUser.Id,
            name: "Ví Tiền Mặt",
            walletType: WalletType.Cash,
            initialBalance: 1_200_000m,
            currency: "VND",
            colorHex: "#10B981",
            iconKey: "cash"
        );

        var savingsWallet = new Wallet(
            demoUser.Id,
            name: "Quỹ Khẩn Cấp (Techcombank)",
            walletType: WalletType.BankAccount,
            initialBalance: 10_000_000m,
            currency: "VND",
            accountNumber: "19033445566",
            bankName: "Techcombank",
            colorHex: "#8B5CF6",
            iconKey: "shield"
        );

        await context.Wallets.AddRangeAsync(new[] { bankWallet, cashWallet, savingsWallet }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Fetch Categories
        var foodCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Ăn uống & Thực phẩm" && c.IsSystem, cancellationToken)
            ?? await context.Categories.FirstAsync(c => c.TransactionType == TransactionType.Expense, cancellationToken);
        var transportCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Đi lại & Xăng xe" && c.IsSystem, cancellationToken)
            ?? foodCategory;
        var housingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Nhà ở & Tiện ích" && c.IsSystem, cancellationToken)
            ?? foodCategory;
        var shoppingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Mua sắm cá nhân" && c.IsSystem, cancellationToken)
            ?? foodCategory;
        var salaryCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Tiền lương" && c.IsSystem, cancellationToken)
            ?? await context.Categories.FirstAsync(c => c.TransactionType == TransactionType.Income, cancellationToken);
        var freelanceCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Làm thêm & Freelance" && c.IsSystem, cancellationToken)
            ?? salaryCategory;

        // 2. Budget and Allocations
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var demoBudget = new Budget(
            demoUser.Id,
            name: $"Ngân sách Tháng {now.Month}/{now.Year}",
            totalAmount: 12_000_000m,
            startDate: startOfMonth,
            endDate: endOfMonth,
            periodType: BudgetPeriodType.Monthly,
            method: BudgetMethod.Rule503020
        );

        // Allocation: Food 3M (spent 2.55M = 85.0%), Transport 1.5M (spent 650K = 43.3%), Housing 4M (spent 3.5M = 87.5%), Shopping 2M (spent 1.2M = 60.0%)
        demoBudget.AddAllocation(foodCategory.Id, 3_000_000m, 80m);
        demoBudget.AddAllocation(transportCategory.Id, 1_500_000m, 80m);
        demoBudget.AddAllocation(housingCategory.Id, 4_000_000m, 85m);
        demoBudget.AddAllocation(shoppingCategory.Id, 2_000_000m, 80m);

        // Update allocation current spent figures to match the seeded transactions
        demoBudget.RecordExpense(foodCategory.Id, 2_550_000m);
        demoBudget.RecordExpense(transportCategory.Id, 650_000m);
        demoBudget.RecordExpense(housingCategory.Id, 3_500_000m);
        demoBudget.RecordExpense(shoppingCategory.Id, 1_200_000m);

        await context.Budgets.AddAsync(demoBudget, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Daily Spending Allowance
        var dsa = new DailySpendingAllowance(
            demoBudget.Id,
            now.Date,
            dailyAllowanceAmount: 350_000m,
            actualSpentToday: 85_000m,
            velocityIndex: 1.05m
        );
        await context.DailySpendingAllowances.AddAsync(dsa, cancellationToken);

        // 3. Transactions
        var salaryTx = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            salaryCategory.Id,
            amount: 22_690_000m,
            transactionType: TransactionType.Income,
            transactionDate: now.AddDays(-15),
            note: "Lương tháng chuyển khoản FPT"
        );

        var freelanceTx = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            freelanceCategory.Id,
            amount: 3_500_000m,
            transactionType: TransactionType.Income,
            transactionDate: now.AddDays(-6),
            note: "Thu nhập dự án ngoài giờ"
        );

        var winmartTx = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            foodCategory.Id,
            amount: 385_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-3),
            note: "Đi siêu thị WinMart",
            isEssential: true
        );

        var foodTx1 = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            foodCategory.Id,
            amount: 1_200_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-10),
            note: "Ăn uống liên hoan nhóm bạn",
            isEssential: false
        );

        var foodTx2 = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            foodCategory.Id,
            amount: 650_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-5),
            note: "Đặt cơm trưa văn phòng tuần 2",
            isEssential: true
        );

        var foodTx3 = new Transaction(
            demoUser.Id,
            cashWallet.Id,
            foodCategory.Id,
            amount: 315_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-2),
            note: "Ăn sáng và cà phê sáng",
            isEssential: true
        );

        var transportTx1 = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            transportCategory.Id,
            amount: 500_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-7),
            note: "Đổ xăng xe máy",
            isEssential: true
        );

        var transportTx2 = new Transaction(
            demoUser.Id,
            cashWallet.Id,
            transportCategory.Id,
            amount: 150_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-1),
            note: "Gửi xe tháng và rửa xe",
            isEssential: true
        );

        var housingTx = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            housingCategory.Id,
            amount: 3_500_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-12),
            note: "Tiền thuê nhà và điện nước",
            isEssential: true
        );

        var shoppingTx = new Transaction(
            demoUser.Id,
            bankWallet.Id,
            shoppingCategory.Id,
            amount: 1_200_000m,
            transactionType: TransactionType.Expense,
            transactionDate: now.AddDays(-8),
            note: "Mua quần áo công sở Uniqlo",
            isEssential: false
        );

        await context.Transactions.AddRangeAsync(new[]
        {
            salaryTx, freelanceTx,
            winmartTx, foodTx1, foodTx2, foodTx3,
            transportTx1, transportTx2,
            housingTx, shoppingTx
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // 4. Receipt Items for WinMart (Sum = 70k + 35k + 180k + 100k = 385,000 VND)
        var receipt1 = new ReceiptItem(winmartTx.Id, "Sữa tươi tiệt trùng Vinamilk 1L", quantity: 2, unitPrice: 35_000m, totalPrice: 70_000m, categoryId: foodCategory.Id);
        var receipt2 = new ReceiptItem(winmartTx.Id, "Trứng gà tươi Ba Huân (hộp 10 quả)", quantity: 1, unitPrice: 35_000m, totalPrice: 35_000m, categoryId: foodCategory.Id);
        var receipt3 = new ReceiptItem(winmartTx.Id, "Gạo lúa tôm ST25 Ông Cua túi 5kg", quantity: 1, unitPrice: 180_000m, totalPrice: 180_000m, categoryId: foodCategory.Id);
        var receipt4 = new ReceiptItem(winmartTx.Id, "Nước mắm Nam Ngư Đệ Nhị 750ml", quantity: 1, unitPrice: 100_000m, totalPrice: 100_000m, categoryId: foodCategory.Id);

        await context.ReceiptItems.AddRangeAsync(new[] { receipt1, receipt2, receipt3, receipt4 }, cancellationToken);

        // 5. Salary Slip Detail for Salary Tx (Net = 22,000,000 - 2,310,000 + 3,000,000 = 22,690,000 VND)
        var salarySlip = new SalarySlipDetail(
            demoUser.Id,
            salaryTx.Id,
            employerName: "Công ty Cổ phần Công nghệ FPT",
            payPeriod: $"{now.Month:D2}/{now.Year}",
            grossSalary: 22_000_000m,
            netSalary: 22_690_000m,
            socialInsurance: 1_760_000m,
            healthInsurance: 330_000m,
            unemploymentInsurance: 220_000m,
            personalIncomeTax: 0m,
            allowances: 0m,
            bonus: 3_000_000m,
            otherDeductions: 0m,
            rawOcrJson: "{\"employer\":\"FPT\",\"gross\":22000000,\"net\":22690000,\"bonus\":3000000}"
        );
        await context.SalarySlipDetails.AddAsync(salarySlip, cancellationToken);

        // 6. Savings Goals
        var laptopGoal = new SavingsGoal(
            demoUser.Id,
            name: "Mua Laptop Mới",
            targetAmount: 25_000_000m,
            startDate: now.AddMonths(-2),
            targetDate: now.AddMonths(4),
            type: SavingsGoalType.Other,
            priority: SavingsGoalPriority.High,
            initialAmount: 0m,
            description: "Tích lũy mua laptop mới phục vụ công việc và học tập",
            iconKey: "laptop",
            colorHex: "#3B82F6"
        );
        laptopGoal.Deposit(5_000_000m, bankWallet.Id, "Trích thưởng tháng trước");
        laptopGoal.Deposit(7_000_000m, bankWallet.Id, "Trích tiết kiệm tháng này");

        var emergencyGoal = new SavingsGoal(
            demoUser.Id,
            name: "Quỹ Khẩn Cấp 3 Tháng",
            targetAmount: 20_000_000m,
            startDate: now.AddMonths(-3),
            targetDate: now.AddMonths(9),
            type: SavingsGoalType.EmergencyFund,
            priority: SavingsGoalPriority.High,
            initialAmount: 10_000_000m,
            description: "Quỹ dự phòng tài chính bất khả xâm phạm",
            iconKey: "shield-check",
            colorHex: "#DC2626"
        );

        await context.SavingsGoals.AddRangeAsync(new[] { laptopGoal, emergencyGoal }, cancellationToken);

        // 7. Thaler 24h Cooling-Off Transaction
        var coolingOffTx = new CoolingOffTransaction(
            demoUser.Id,
            bankWallet.Id,
            shoppingCategory.Id,
            amount: 4_500_000m,
            description: "Tai nghe chống ồn Sony WH-1000XM5",
            coolingOffUntil: now.AddHours(18),
            equivalentWorkHours: 32.5m,
            projectedDsaReduction: 150_000m
        );
        coolingOffTx.CreatedAt = now.AddHours(-6);
        await context.CoolingOffTransactions.AddAsync(coolingOffTx, cancellationToken);

        // 8. Financial Nudges
        var nudge1 = new FinancialNudge(
            demoUser.Id,
            NudgeType.DsaThreshold,
            title: "Cảnh báo hạn mức Ăn uống",
            message: "Danh mục Ăn uống & Thực phẩm đã đạt 85% hạn mức tháng. Hãy cân nhắc nấu ăn tại nhà trong tuần này.",
            triggerEvent: "BudgetAlert_85Percent"
        );

        var nudge2 = new FinancialNudge(
            demoUser.Id,
            NudgeType.PayYourselfFirst,
            title: "Cú hích Pay Yourself First",
            message: "Bạn vừa nhận lương! Hãy trích ngay 10% (2.200.000đ) vào mục tiêu Mua Laptop Mới.",
            triggerEvent: "IncomeReceived_Salary"
        );
        await context.FinancialNudges.AddRangeAsync(new[] { nudge1, nudge2 }, cancellationToken);

        // 9. InApp Notifications
        var notif1 = new InAppNotification(
            demoUser.Id,
            title: "Cảnh báo ngân sách",
            message: "Ngân sách Ăn uống tháng này đã đạt mức 85%.",
            type: "BudgetAlert"
        );

        var notif2 = new InAppNotification(
            demoUser.Id,
            title: "Quét hóa đơn thành công",
            message: "Đã quét thành công hóa đơn WinMart: 4 món hàng (385.000 đ).",
            type: "AiReceiptProcessed"
        );

        var notif3 = new InAppNotification(
            demoUser.Id,
            title: "Chào mừng bạn",
            message: "Chào mừng bạn đến với Hệ thống Quản lý Chi tiêu Thông minh!",
            type: "System"
        );
        await context.InAppNotifications.AddRangeAsync(new[] { notif1, notif2, notif3 }, cancellationToken);

        // 10. Net Worth Historical Snapshots
        var nw1 = new NetWorthSnapshot(demoUser.Id, now.AddMonths(-2), totalAssets: 23_500_000m, totalLiabilities: 0m, notes: "Tài sản ròng 2 tháng trước");
        var nw2 = new NetWorthSnapshot(demoUser.Id, now.AddMonths(-1), totalAssets: 25_200_000m, totalLiabilities: 0m, notes: "Tài sản ròng tháng trước");
        var nw3 = new NetWorthSnapshot(demoUser.Id, now, totalAssets: 26_700_000m, totalLiabilities: 0m, notes: "Tài sản ròng hiện tại");
        await context.NetWorthSnapshots.AddRangeAsync(new[] { nw1, nw2, nw3 }, cancellationToken);

        // 11. Auto Categorization Rules
        var rule1 = new AutoCategorizationRule(demoUser.Id, "WinMart", foodCategory.Id, priorityOrder: 1);
        var rule2 = new AutoCategorizationRule(demoUser.Id, "Petrolimex", transportCategory.Id, priorityOrder: 2);
        await context.AutoCategorizationRules.AddRangeAsync(new[] { rule1, rule2 }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Master demo financial storyline seeded successfully for {Email}!", DefaultDemoEmail);

        return demoUser;
    }
}
