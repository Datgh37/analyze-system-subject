using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class AnalyticsReportServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AnalyticsReportService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _foodCategory;
    private readonly Category _rentCategory;
    private readonly Category _coffeeCategory;
    private readonly Category _bookCategory;

    public AnalyticsReportServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new AnalyticsReportService(_dbContext);

        _wallet = new Wallet(_userId, "Ví Chính", WalletType.Cash, 50_000_000m);
        _dbContext.Wallets.Add(_wallet);

        _foodCategory = new Category("Thực phẩm", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _rentCategory = new Category("Tiền thuê nhà", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _coffeeCategory = new Category("Cà phê giải trí", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Wants);
        _bookCategory = new Category("Sách & Học tập", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Wants);

        _dbContext.Categories.AddRange(_foodCategory, _rentCategory, _coffeeCategory, _bookCategory);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetIncomeExpense_WhenPreviousPeriodZero_ShouldReturnSafeGrowthRateWithoutDivisionByZero()
    {
        // Arrange: Transactions in current period only
        var fromDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc);

        var incomeTx = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 25_000_000m, TransactionType.Income, new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc));
        var expenseTx = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 10_000_000m, TransactionType.Expense, new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc));

        _dbContext.Transactions.AddRange(incomeTx, expenseTx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetIncomeExpenseComparisonAsync(_userId, fromDate, toDate, "month");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalIncome.Should().Be(25_000_000m);
        result.Value.TotalExpense.Should().Be(10_000_000m);
        result.Value.NetSurplus.Should().Be(15_000_000m);
        result.Value.GrowthMoM.Should().Be(100m); // Safe fallback when previous period is 0
        result.Value.GrowthYoY.Should().Be(100m);
    }

    [Fact]
    public async Task GetIncomeExpense_WithValidData_ShouldCalculateMoMAndYoYAccurately()
    {
        // Arrange
        // Current: 2026-10 (Expense = 15,000,000)
        // Previous period matching: 2026-09 (Expense = 10,000,000) -> Growth = +50%
        var fromDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc);

        var currentExpense = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 15_000_000m, TransactionType.Expense, new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc));
        var prevExpense = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 10_000_000m, TransactionType.Expense, new DateTime(2026, 9, 15, 0, 0, 0, DateTimeKind.Utc));

        _dbContext.Transactions.AddRange(currentExpense, prevExpense);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetIncomeExpenseComparisonAsync(_userId, fromDate, toDate, "month");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalExpense.Should().Be(15_000_000m);
        result.Value.GrowthMoM.Should().Be(50.0m);
    }

    [Fact]
    public async Task GetNetWorth_ShouldAggregateWalletsAndSavingsGoalsAccurately()
    {
        // Arrange
        // Cash wallet: 20M (Asset)
        var cashWallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 20_000_000m);
        // Bank account: 50M (Asset)
        var bankWallet = new Wallet(_userId, "Tài Khoản VCB", WalletType.BankAccount, 50_000_000m);
        // Credit card: -10M (Liability)
        var creditCard = new Wallet(_userId, "Thẻ Tín Dụng", WalletType.CreditCard, 0m, creditLimit: 30_000_000m);
        creditCard.Debit(10_000_000m); // Balance = -10,000,000

        // Active savings goal: 15M (Asset)
        var savingsGoal = new SavingsGoal(_userId, "Quỹ Mua Xe", 100_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        savingsGoal.Deposit(15_000_000m, cashWallet.Id);

        _dbContext.Wallets.AddRange(cashWallet, bankWallet, creditCard);
        _dbContext.SavingsGoals.Add(savingsGoal);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetNetWorthReportAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        // TotalAssets = initial _wallet (50M) + cashWallet (20M) + bankWallet (50M) + savingsGoal (15M) = 135M
        // TotalLiabilities = creditCard (10M)
        // NetWorth = 135M - 10M = 125M
        result.Value!.TotalAssets.Should().Be(135_000_000m);
        result.Value.TotalLiabilities.Should().Be(10_000_000m);
        result.Value.CurrentNetWorth.Should().Be(125_000_000m);
        result.Value.IsNegativeNetWorthWarning.Should().BeFalse();
    }

    [Fact]
    public async Task GetNetCashflow_WhenIncomeZero_ShouldHandleZeroDivisionGracefully()
    {
        // Arrange: Expense only, 0 income
        var fromDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc);

        var expenseTx = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 5_000_000m, TransactionType.Expense, new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc));
        _dbContext.Transactions.Add(expenseTx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetNetCashflowReportAsync(_userId, fromDate, toDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalIncome.Should().Be(0m);
        result.Value.TotalExpense.Should().Be(5_000_000m);
        result.Value.NetCashflow.Should().Be(-5_000_000m);
        result.Value.SavingsRate.Should().Be(-100m);
        result.Value.HealthStatus.Should().Be("Dangerous");
    }

    [Fact]
    public async Task GetNetCashflow_ShouldClassifyHealthStatusAccurately()
    {
        // Arrange: Income = 20M, Expense = 14M -> Net = 6M, SavingsRate = 30% -> "Excellent"
        var fromDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc);

        var incomeTx = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 20_000_000m, TransactionType.Income, new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc));
        var expenseTx = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 14_000_000m, TransactionType.Expense, new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc));

        _dbContext.Transactions.AddRange(incomeTx, expenseTx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetNetCashflowReportAsync(_userId, fromDate, toDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.SavingsRate.Should().Be(30.0m);
        result.Value.HealthStatus.Should().Be("Excellent");
        result.Value.WaterfallSteps.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetExpenseStructure_ShouldCategorize503020AndKakeiboCorrectly()
    {
        // Arrange
        var fromDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc);

        // Food (Needs / Survival): 5,000,000
        var t1 = new Transaction(_userId, _wallet.Id, _foodCategory.Id, 5_000_000m, TransactionType.Expense, new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc));
        // Coffee (Wants / Optional): 2,000,000
        var t2 = new Transaction(_userId, _wallet.Id, _coffeeCategory.Id, 2_000_000m, TransactionType.Expense, new DateTime(2026, 10, 6, 0, 0, 0, DateTimeKind.Utc));
        // Book (Wants / Culture): 3,000,000
        var t3 = new Transaction(_userId, _wallet.Id, _bookCategory.Id, 3_000_000m, TransactionType.Expense, new DateTime(2026, 10, 7, 0, 0, 0, DateTimeKind.Utc));

        _dbContext.Transactions.AddRange(t1, t2, t3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetExpenseStructureReportAsync(_userId, fromDate, toDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalExpense.Should().Be(10_000_000m);
        // 50/30/20: Needs = 5M (50%), Wants = 5M (50%)
        result.Value.By503020.NeedsAmount.Should().Be(5_000_000m);
        result.Value.By503020.NeedsPercentage.Should().Be(50.0m);
        result.Value.By503020.WantsAmount.Should().Be(5_000_000m);
        result.Value.By503020.WantsPercentage.Should().Be(50.0m);

        // Kakeibo: Survival = 5M, Optional = 2M, Culture = 3M
        result.Value.ByKakeibo.SurvivalAmount.Should().Be(5_000_000m);
        result.Value.ByKakeibo.OptionalAmount.Should().Be(2_000_000m);
        result.Value.ByKakeibo.CultureAmount.Should().Be(3_000_000m);
    }

    [Fact]
    public async Task DetectSpendingOutliers_WhenVarianceZero_ShouldReturnZeroZScoreWithoutException()
    {
        // Arrange: 5 transactions of the EXACT same amount (variance = 0, stdDev = 0)
        for (int i = 0; i < 5; i++)
        {
            _dbContext.Transactions.Add(new Transaction(
                _userId,
                _wallet.Id,
                _foodCategory.Id,
                100_000m,
                TransactionType.Expense,
                DateTime.UtcNow.AddDays(-i)));
        }
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.DetectSpendingOutliersAsync(_userId, days: 30, threshold: 2.0);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.OutlierCount.Should().Be(0);
        result.Value.Outliers.Should().BeEmpty();
    }

    [Fact]
    public async Task DetectSpendingOutliers_WithSpikeTransaction_ShouldFlagOutlierWhenZScoreExceedsThreshold()
    {
        // Arrange: 10 transactions around 100,000 + 1 massive spike transaction of 5,000,000
        for (int i = 0; i < 10; i++)
        {
            _dbContext.Transactions.Add(new Transaction(
                _userId,
                _wallet.Id,
                _coffeeCategory.Id,
                100_000m + (i * 2_000m),
                TransactionType.Expense,
                DateTime.UtcNow.AddDays(-i)));
        }

        var spikeTx = new Transaction(
            _userId,
            _wallet.Id,
            _coffeeCategory.Id,
            5_000_000m,
            TransactionType.Expense,
            DateTime.UtcNow.AddDays(-1),
            note: "Tiệc sinh nhật công ty");

        _dbContext.Transactions.Add(spikeTx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.DetectSpendingOutliersAsync(_userId, days: 30, threshold: 2.0);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.OutlierCount.Should().BeGreaterThanOrEqualTo(1);
        var flagged = result.Value.Outliers.First();
        flagged.Amount.Should().Be(5_000_000m);
        flagged.ZScore.Should().BeGreaterThan(2.0);
    }
}
