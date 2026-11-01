using ExpenseTracker.Application.DTOs.EmergencyFund;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class EmergencyFundServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly EmergencyFundService _service;
    private readonly Guid _userId = Guid.NewGuid();

    public EmergencyFundServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new EmergencyFundService(_dbContext);
    }

    [Fact]
    public async Task GetRecommendation_WhenNoTransactions_ReturnsDefaultRecommendation()
    {
        // Act
        var result = await _service.GetRecommendationAsync(_userId);

        // Assert
        result.Should().NotBeNull();
        result.MonthlyEssentialExpenses.Should().Be(5_000_000m);
        result.ThreeMonthsAmount.Should().Be(15_000_000m);
        result.SixMonthsAmount.Should().Be(30_000_000m);
        result.TwelveMonthsAmount.Should().Be(60_000_000m);
        result.CurrentMonthsCovered.Should().Be(0m);
        result.RecommendationNote.Should().Contain("tối thiểu 3 tháng");
    }

    [Fact]
    public async Task GetRecommendation_WithNeedsTransactions_CalculatesAccurateNeeds()
    {
        // Arrange: 3 tháng chi phí ăn uống 30.000.000đ (10tr/tháng)
        var wallet = new Wallet(_userId, "Ví Chính", WalletType.Cash, 50_000_000m);
        var category = new Category("Ăn uống", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _dbContext.Wallets.Add(wallet);
        _dbContext.Categories.Add(category);

        var tx = new Transaction(_userId, wallet.Id, category.Id, 30_000_000m, TransactionType.Expense, DateTime.UtcNow.AddDays(-10));
        _dbContext.Transactions.Add(tx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetRecommendationAsync(_userId);

        // Assert
        result.MonthlyEssentialExpenses.Should().Be(10_000_000m);
        result.ThreeMonthsAmount.Should().Be(30_000_000m);
        result.SixMonthsAmount.Should().Be(60_000_000m);
        result.TwelveMonthsAmount.Should().Be(120_000_000m);
    }

    [Fact]
    public async Task SetupFund_CreatesNewEmergencyGoal_AndPerformsInitialDeposit()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Lương", WalletType.BankAccount, 20_000_000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        var request = new SetupEmergencyFundDto
        {
            TargetMonths = 6,
            TargetAmount = 30_000_000m,
            InitialDepositWalletId = wallet.Id,
            InitialDepositAmount = 5_000_000m
        };

        // Act
        var status = await _service.SetupFundAsync(_userId, request);

        // Assert
        status.Should().NotBeNull();
        status.HasFundConfigured.Should().BeTrue();
        status.TargetAmount.Should().Be(30_000_000m);
        status.CurrentAmount.Should().Be(5_000_000m);
        status.IsLocked.Should().BeTrue();

        // Verify wallet debited
        wallet.Balance.Should().Be(15_000_000m);
    }

    [Fact]
    public async Task Deposit_AddsFundsToEmergencyGoal_AndDebitsWallet()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 10_000_000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        await _service.SetupFundAsync(_userId, new SetupEmergencyFundDto { TargetMonths = 3, TargetAmount = 15_000_000m });

        var depositDto = new DepositEmergencyFundDto
        {
            Amount = 3_000_000m,
            SourceWalletId = wallet.Id,
            Note = "Tiết kiệm thêm"
        };

        // Act
        var status = await _service.DepositAsync(_userId, depositDto);

        // Assert
        status.CurrentAmount.Should().Be(3_000_000m);
        wallet.Balance.Should().Be(7_000_000m);
    }

    [Fact]
    public async Task Withdraw_WithValidExplanation_CreditsWallet_AndGeneratesNudge()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Chính", WalletType.Cash, 10_000_000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        await _service.SetupFundAsync(_userId, new SetupEmergencyFundDto
        {
            TargetMonths = 6,
            TargetAmount = 30_000_000m,
            InitialDepositWalletId = wallet.Id,
            InitialDepositAmount = 8_000_000m
        });

        var withdrawDto = new WithdrawEmergencyFundDto
        {
            Amount = 2_000_000m,
            TargetWalletId = wallet.Id,
            EmergencyReason = "Medical",
            DetailedExplanation = "Đi khám bệnh đột xuất tại bệnh viện đa khoa."
        };

        // Act
        var status = await _service.WithdrawAsync(_userId, withdrawDto);

        // Assert
        status.CurrentAmount.Should().Be(6_000_000m);
        wallet.Balance.Should().Be(4_000_000m); // 10tr ban đầu - 8tr nạp + 2tr rút = 4tr

        var nudge = await _dbContext.FinancialNudges.FirstOrDefaultAsync(n => n.UserId == _userId);
        nudge.Should().NotBeNull();
        nudge!.Title.Should().Contain("Cảnh báo giải ngân Quỹ Khẩn Cấp");
    }

    [Fact]
    public async Task Withdraw_WhenExplanationTooShort_ThrowsArgumentException()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Chính", WalletType.Cash, 10_000_000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        await _service.SetupFundAsync(_userId, new SetupEmergencyFundDto
        {
            TargetMonths = 3,
            TargetAmount = 15_000_000m,
            InitialDepositWalletId = wallet.Id,
            InitialDepositAmount = 5_000_000m
        });

        var withdrawDto = new WithdrawEmergencyFundDto
        {
            Amount = 1_000_000m,
            TargetWalletId = wallet.Id,
            EmergencyReason = "Other",
            DetailedExplanation = "Cần tiền" // < 10 ký tự -> Không vượt qua ma sát tâm lý
        };

        // Act & Assert
        var act = async () => await _service.WithdrawAsync(_userId, withdrawDto);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*ma sát tâm lý*");
    }

    [Fact]
    public async Task Withdraw_WhenAmountExceedsCurrentAmount_ThrowsInvalidOperationException()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Chính", WalletType.Cash, 10_000_000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        await _service.SetupFundAsync(_userId, new SetupEmergencyFundDto
        {
            TargetMonths = 3,
            TargetAmount = 15_000_000m,
            InitialDepositWalletId = wallet.Id,
            InitialDepositAmount = 2_000_000m
        });

        var withdrawDto = new WithdrawEmergencyFundDto
        {
            Amount = 5_000_000m, // Rút vượt quá 2tr đang có
            TargetWalletId = wallet.Id,
            EmergencyReason = "JobLoss",
            DetailedExplanation = "Mất việc làm cần tiền trang trải tiền nhà khẩn cấp."
        };

        // Act & Assert
        var act = async () => await _service.WithdrawAsync(_userId, withdrawDto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*vượt quá số dư*");
    }

    [Fact]
    public async Task GetStatus_WhenFundNotConfigured_ReturnsNotConfiguredStatus()
    {
        // Act
        var status = await _service.GetStatusAsync(_userId);

        // Assert
        status.Should().NotBeNull();
        status.HasFundConfigured.Should().BeFalse();
        status.Status.Should().Be("NotConfigured");
        status.IsLocked.Should().BeTrue();
    }
}
