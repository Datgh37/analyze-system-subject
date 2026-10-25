using ExpenseTracker.Application.DTOs.SavingsGoals;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class SavingsGoalServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SavingsGoalService _goalService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;

    public SavingsGoalServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _goalService = new SavingsGoalService(_dbContext);

        // Seed wallet with 20,000,000 VND
        _wallet = new Wallet(_userId, "Tài khoản Vietcombank", WalletType.BankAccount, 20_000_000m, "VND");
        _dbContext.Wallets.Add(_wallet);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task CreateGoalAsync_WithValidData_PersistsGoalAndReturnsDto()
    {
        // Arrange
        var request = new CreateSavingsGoalDto(
            Name: "Mua Xe Máy Mới",
            TargetAmount: 30_000_000m,
            StartDate: DateTime.Today,
            TargetDate: DateTime.Today.AddMonths(10),
            Type: (int)SavingsGoalType.MajorPurchase,
            Priority: (int)SavingsGoalPriority.High,
            InitialAmount: 5_000_000m,
            Description: "Tiết kiệm mua Honda AirBlade"
        );

        // Act
        var result = await _goalService.CreateGoalAsync(_userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Mua Xe Máy Mới");
        result.TargetAmount.Should().Be(30_000_000m);
        result.CurrentAmount.Should().Be(5_000_000m);
        result.RemainingAmount.Should().Be(25_000_000m);
        result.ProgressPercent.Should().BeApproximately(16.67m, 0.01m);
        result.IsCompleted.Should().BeFalse();

        var dbGoal = await _dbContext.SavingsGoals.FirstOrDefaultAsync(g => g.Id == result.Id);
        dbGoal.Should().NotBeNull();
        dbGoal!.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task DepositAsync_DebitsWalletBalanceAndCreditsGoal()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Quỹ Dự Phòng", 50_000_000m, DateTime.Today, DateTime.Today.AddYears(1));
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        var depositRequest = new DepositSavingsGoalDto(
            WalletId: _wallet.Id,
            Amount: 5_000_000m,
            Note: "Trích từ lương tháng"
        );

        // Act
        var contribution = await _goalService.DepositAsync(_userId, goal.Id, depositRequest);

        // Assert
        contribution.Should().NotBeNull();
        contribution.Amount.Should().Be(5_000_000m);
        contribution.WalletName.Should().Be(_wallet.Name);

        // Wallet should be debited from 20M to 15M
        var updatedWallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        updatedWallet!.Balance.Should().Be(15_000_000m);

        // Goal should be credited to 5M
        var updatedGoal = await _dbContext.SavingsGoals.FindAsync(goal.Id);
        updatedGoal!.CurrentAmount.Should().Be(5_000_000m);
        updatedGoal.ProgressPercent.Should().Be(10m);
    }

    [Fact]
    public async Task DepositAsync_WhenWalletInsufficientFunds_ThrowsInvalidOperationException()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Quỹ Mua Nhà", 100_000_000m, DateTime.Today, DateTime.Today.AddYears(3));
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        // Wallet balance is 20M, attempting to deposit 25M
        var depositRequest = new DepositSavingsGoalDto(
            WalletId: _wallet.Id,
            Amount: 25_000_000m,
            Note: "Vượt số dư"
        );

        // Act
        var act = async () => await _goalService.DepositAsync(_userId, goal.Id, depositRequest);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*không đủ để thực hiện giao dịch*");
    }

    [Fact]
    public async Task WithdrawAsync_CreditsWalletAndDebitsGoal()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Quỹ Khẩn Cấp", 30_000_000m, DateTime.Today, DateTime.Today.AddMonths(6), initialAmount: 10_000_000m);
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        var withdrawRequest = new WithdrawSavingsGoalDto(
            TargetWalletId: _wallet.Id,
            Amount: 4_000_000m,
            Reason: "Chi phí khám chữa bệnh khẩn cấp"
        );

        // Act
        var contribution = await _goalService.WithdrawAsync(_userId, goal.Id, withdrawRequest);

        // Assert
        contribution.Should().NotBeNull();
        contribution.Amount.Should().Be(-4_000_000m);

        // Wallet should be credited from 20M to 24M
        var updatedWallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        updatedWallet!.Balance.Should().Be(24_000_000m);

        // Goal should be debited from 10M to 6M
        var updatedGoal = await _dbContext.SavingsGoals.FindAsync(goal.Id);
        updatedGoal!.CurrentAmount.Should().Be(6_000_000m);
    }

    [Fact]
    public async Task WithdrawAsync_ExceedingGoalBalance_ThrowsException()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Quỹ Du Lịch", 10_000_000m, DateTime.Today, DateTime.Today.AddMonths(3), initialAmount: 2_000_000m);
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        var withdrawRequest = new WithdrawSavingsGoalDto(
            TargetWalletId: _wallet.Id,
            Amount: 5_000_000m,
            Reason: "Muốn rút nhiều hơn quỹ có"
        );

        // Act
        var act = async () => await _goalService.WithdrawAsync(_userId, goal.Id, withdrawRequest);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*vượt quá số dư hiện có trong quỹ*");
    }

    [Fact]
    public async Task DeleteGoalAsync_WhenCurrentAmountGreaterThanZero_ThrowsInvalidOperationException()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Quỹ Tiết Kiệm", 20_000_000m, DateTime.Today, DateTime.Today.AddMonths(6), initialAmount: 3_000_000m);
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        // Act
        var act = async () => await _goalService.DeleteGoalAsync(_userId, goal.Id);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*vẫn còn số dư 3.000.000 đ*");
    }

    [Fact]
    public async Task DeleteGoalAsync_WhenBalanceZero_DeletesSuccessfully()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Mục Tiêu Nháp", 10_000_000m, DateTime.Today, DateTime.Today.AddMonths(6), initialAmount: 0m);
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        // Act
        await _goalService.DeleteGoalAsync(_userId, goal.Id);

        // Assert
        var deletedGoal = await _dbContext.SavingsGoals.FindAsync(goal.Id);
        deletedGoal.Should().BeNull();
    }

    [Fact]
    public async Task EvaluateGoalFeasibilityAsync_ReturnsCorrectComparisonWithActiveBudget()
    {
        // Arrange
        // Active Budget = 15,000,000 VND -> 20% Savings limit = 3,000,000 VND/month
        var budget = new Budget(_userId, "Ngân Sách Tháng 10", 15_000_000m, DateTime.Today, DateTime.Today.AddDays(30));
        _dbContext.Budgets.Add(budget);

        // Goal: Target 12M in 4 months => Requires 3M/month => Feasibility = 100%
        var goal = new SavingsGoal(_userId, "Quỹ Du Lịch", 12_000_000m, DateTime.Today, DateTime.Today.AddMonths(4));
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        // Act
        var feasibility = await _goalService.EvaluateGoalFeasibilityAsync(_userId, goal.Id);

        // Assert
        feasibility.Should().NotBeNull();
        feasibility.MonthlyAvailableSavings.Should().Be(3_000_000m);
        feasibility.MonthlyDepositRequired.Should().Be(3_000_000m);
        feasibility.IsFeasible.Should().BeTrue();
        feasibility.AdviceMessage.Should().Contain("Tính khả thi cao");
    }

    [Fact]
    public async Task GetJarsSummaryAsync_AggregatesGoalsByCategories()
    {
        // Arrange
        var goal1 = new SavingsGoal(_userId, "Quỹ Khẩn Cấp", 30_000_000m, DateTime.Today, DateTime.Today.AddYears(1), SavingsGoalType.EmergencyFund, initialAmount: 10_000_000m);
        var goal2 = new SavingsGoal(_userId, "Đầu Tư Cổ Phiếu", 50_000_000m, DateTime.Today, DateTime.Today.AddYears(2), SavingsGoalType.Investment, initialAmount: 15_000_000m);
        _dbContext.SavingsGoals.AddRange(goal1, goal2);
        await _dbContext.SaveChangesAsync();

        // Act
        var summary = await _goalService.GetJarsSummaryAsync(_userId);

        // Assert
        summary.Should().NotBeNull();
        summary.TotalSavedAmount.Should().Be(25_000_000m);
        summary.TotalTargetAmount.Should().Be(80_000_000m);
        summary.ActiveGoalsCount.Should().Be(2);
        summary.JarsBreakdown.Should().HaveCount(2);
    }

    [Fact]
    public async Task EvaluateGoalFeasibilityAsync_WhenNoActiveBudget_UsesFallbackBaselineWithoutDivisionByZero()
    {
        // Arrange - No active budget in database for this user
        var goal = new SavingsGoal(_userId, "Quỹ Mua Xe", 20_000_000m, DateTime.Today, DateTime.Today.AddMonths(5));
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        // Act
        var feasibility = await _goalService.EvaluateGoalFeasibilityAsync(_userId, goal.Id);

        // Assert
        feasibility.Should().NotBeNull();
        feasibility.MonthlyAvailableSavings.Should().Be(2_000_000m); // Fallback to 2M baseline
        feasibility.MonthlyDepositRequired.Should().Be(4_000_000m);
        feasibility.IsFeasible.Should().BeFalse();
        feasibility.AdviceMessage.Should().Contain("⚠️ Cảnh báo khả thi");
    }

    [Fact]
    public async Task ConfigureAutoDepositAsync_WithValidWallet_UpdatesGoalSettings()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Quỹ Tích Lũy Tự Động", 50_000_000m, DateTime.Today, DateTime.Today.AddYears(2));
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        var request = new ConfigureAutoDepositDto(
            IsEnabled: true,
            Amount: 1_500_000m,
            DayOfMonth: 10,
            WalletId: _wallet.Id
        );

        // Act
        var result = await _goalService.ConfigureAutoDepositAsync(_userId, goal.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.IsAutoDepositEnabled.Should().BeTrue();
        result.AutoDepositAmount.Should().Be(1_500_000m);
        result.AutoDepositDayOfMonth.Should().Be(10);
        result.AutoDepositWalletId.Should().Be(_wallet.Id);

        var updatedGoal = await _dbContext.SavingsGoals.FindAsync(goal.Id);
        updatedGoal!.IsAutoDepositEnabled.Should().BeTrue();
        updatedGoal.AutoDepositAmount.Should().Be(1_500_000m);
    }

    [Fact]
    public async Task PatchGoalAsync_WithPartialProperties_ShouldOnlyUpdateProvidedFields()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Mua laptop", 30_000_000m, DateTime.Today, DateTime.Today.AddMonths(6));
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        var patchDto = new PatchSavingsGoalDto(
            Name: "Mua Macbook Pro M4",
            ColorHex: "#6366F1"
        );

        // Act
        var result = await _goalService.PatchGoalAsync(_userId, goal.Id, patchDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Mua Macbook Pro M4");
        result.ColorHex.Should().Be("#6366F1");
        result.TargetAmount.Should().Be(30_000_000m); // Unchanged
    }

    [Fact]
    public async Task PatchGoalStatusAsync_WhenPausingAndResuming_UpdatesStatusCorrectly()
    {
        // Arrange
        var goal = new SavingsGoal(_userId, "Đi du lịch Nhật Bản", 50_000_000m, DateTime.Today, DateTime.Today.AddYears(1));
        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        // Act: Pause
        var paused = await _goalService.PatchGoalStatusAsync(_userId, goal.Id, (int)SavingsGoalStatus.Paused);

        // Assert: Paused
        paused.Status.Should().Be((int)SavingsGoalStatus.Paused);

        // Act: Resume
        var resumed = await _goalService.PatchGoalStatusAsync(_userId, goal.Id, (int)SavingsGoalStatus.Active);

        // Assert: Resumed
        resumed.Status.Should().Be((int)SavingsGoalStatus.Active);
    }
}

