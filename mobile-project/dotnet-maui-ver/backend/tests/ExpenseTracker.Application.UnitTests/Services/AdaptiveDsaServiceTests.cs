using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating Adaptive DSA formula, elasticity, and spending velocity tracker (UC-05.05/05.06)
public class AdaptiveDsaServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AdaptiveDsaService _dsaService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Category _wantsCat;
    private readonly Wallet _wallet;

    public AdaptiveDsaServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _dsaService = new AdaptiveDsaService(_dbContext);

        _wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 10_000_000m);
        _dbContext.Wallets.Add(_wallet);

        _wantsCat = new Category("Cà phê giải trí", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Wants);
        _dbContext.Categories.Add(_wantsCat);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetCurrentDsaStatusAsync_CalculatesDailyAllowanceAccurately()
    {
        // Arrange: 30-day month, flexible budget = 3,000,000 VND
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(29); // 30 days total
        var budget = new Budget(_userId, "Tháng này", 10_000_000m, startDate, endDate);
        budget.AddAllocation(_wantsCat.Id, 3_000_000m);
        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _dsaService.GetCurrentDsaStatusAsync(_userId);

        // Assert (UC-05.05 Scenario 1: DSA = 3,000,000 / 30 = 100,000 VND)
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.DailyAllowanceAmount.Should().Be(100_000m);
        result.Value.RemainingAllowanceToday.Should().Be(100_000m);
        result.Value.IsExceeded.Should().BeFalse();
    }

    [Fact]
    public async Task GetCurrentDsaStatusAsync_WhenSpentTodayExceedsDSA_DetectsExceeded()
    {
        // Arrange: 30-day month, daily allowance = 100,000 VND
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(29);
        var budget = new Budget(_userId, "Tháng này", 10_000_000m, startDate, endDate);
        budget.AddAllocation(_wantsCat.Id, 3_000_000m);
        _dbContext.Budgets.Add(budget);

        // Record a transaction of 150,000 VND today
        var tx = new Transaction(_userId, _wallet.Id, _wantsCat.Id, 150_000m, TransactionType.Expense, startDate, "Uống cà phê");
        _dbContext.Transactions.Add(tx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _dsaService.GetCurrentDsaStatusAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ActualSpentToday.Should().Be(150_000m);
        result.Value.RemainingAllowanceToday.Should().Be(-50_000m);
        result.Value.IsExceeded.Should().BeTrue();
        result.Value.AdviceMessage.Should().Contain("siết giảm DSA");
    }

    [Fact]
    public async Task GetSpendingVelocityAsync_WhenVelocityHigh_ForecastsOutOfMoneyDate()
    {
        // Arrange: Budget 30 days = 9,000,000 VND (target = 300,000/day).
        // 10 days passed, user spent 4,500,000 VND (actual = 450,000/day => velocity = 1.5 > 1.2 Danger).
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-9); // Day 10
        var endDate = startDate.AddDays(29); // 30 days total
        var budget = new Budget(_userId, "Tháng này", 9_000_000m, startDate, endDate);
        budget.AddAllocation(_wantsCat.Id, 9_000_000m);
        budget.RecordExpense(_wantsCat.Id, 4_500_000m);
        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _dsaService.GetSpendingVelocityAsync(_userId);

        // Assert (UC-05.06 Scenario 2)
        result.IsSuccess.Should().BeTrue();
        result.Value!.VelocityIndex.Should().Be(1.5m);
        result.Value.LevelName.Should().Be("Nguy hiểm");
        result.Value.LevelColorHex.Should().Be("#EF4444");
        result.Value.DaysUntilDepletion.Should().Be(10); // 4,500,000 remaining / 450,000 = 10 days
        result.Value.OutOfMoneyForecastDate.Should().Be(today.AddDays(10));
        result.Value.ForecastMessage.Should().Contain("CẠN SẠCH TIỀN");
    }
}
