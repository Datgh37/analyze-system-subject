using System;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Nudges;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class CoolingOffServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CoolingOffService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _watchCategory;
    private readonly Category _rentCategory;

    public CoolingOffServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new CoolingOffService(_dbContext, NullLogger<CoolingOffService>.Instance);

        _wallet = new Wallet(_userId, "Ví chính", WalletType.Cash, 10_000_000m);
        _watchCategory = new Category("Đồ công nghệ", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Wants);
        _rentCategory = new Category("Tiền nhà", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);

        _dbContext.Wallets.Add(_wallet);
        _dbContext.Categories.AddRange(_watchCategory, _rentCategory);
        _dbContext.SaveChanges();
    }

    // UC-09.05 Test 1: Expense >= 3M triggers cooling-off and computes work hours accurately
    [Fact]
    public async Task EvaluateExpenseForCoolingOff_WithLargeExpense_ShouldRequireCoolingOffAndCalculateWorkHours()
    {
        // Arrange: 15M monthly income -> 160h/month = ~93,750 VND/hour
        var recurring = new RecurringIncome(_userId, "Lương", 15_000_000m, _wallet.Id, _watchCategory.Id, RecurringFrequency.Monthly, 1);
        _dbContext.RecurringIncomes.Add(recurring);
        await _dbContext.SaveChangesAsync();

        var request = new CoolingOffEvaluationRequestDto
        {
            WalletId = _wallet.Id,
            CategoryId = _watchCategory.Id,
            Amount = 4_500_000m,
            Description = "Smartwatch cao cấp"
        };

        // Act
        var result = await _service.EvaluateExpenseForCoolingOffAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.RequiresCoolingOff.Should().BeTrue();
        result.Value.EquivalentWorkHours.Should().BeGreaterThan(40m); // 4.5M / 93.75k = 48 hours
        result.Value.ImpactAnalysisMessage.Should().Contain("KHOẢNG TRỄ TÂM LÝ 24H");
    }

    // UC-09.05 Test 2: Create cooling-off pending transaction does NOT deduct wallet balance
    [Fact]
    public async Task CreateCoolingOffTransaction_ShouldCreatePendingItemWithoutDeductingWallet()
    {
        // Arrange
        var request = new CreateCoolingOffRequestDto
        {
            WalletId = _wallet.Id,
            CategoryId = _watchCategory.Id,
            Amount = 5_000_000m,
            Description = "Tai nghe Sony",
            CooldownHours = 24
        };

        // Act
        var result = await _service.CreateCoolingOffTransactionAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(CoolingOffStatus.PendingCoolingOff);
        result.Value.Amount.Should().Be(5_000_000m);

        // Verify wallet balance is UNTOUCHED (positive friction guarantee)
        var freshWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        freshWallet.Balance.Should().Be(10_000_000m);
    }

    // UC-09.05 Test 3: Confirm purchase before cooldown expires should fail
    [Fact]
    public async Task ConfirmPurchase_BeforeCooldownExpires_ShouldReturnFailure()
    {
        // Arrange: Transaction with cooldown in the future
        var cooldown = DateTime.UtcNow.AddHours(24);
        var tx = new CoolingOffTransaction(_userId, _wallet.Id, _watchCategory.Id, 3_000_000m, "Loa", cooldown, 30m, 100_000m);
        _dbContext.CoolingOffTransactions.Add(tx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.ConfirmPurchaseAsync(_userId, tx.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Khoảng trễ tâm lý chưa kết thúc");
    }

    // UC-09.05 Test 4: Cancel purchase cancels transaction and protects money
    [Fact]
    public async Task CancelPurchase_WhenPending_ShouldCancelTransactionAndKeepFundsIntact()
    {
        // Arrange
        var cooldown = DateTime.UtcNow.AddHours(24);
        var tx = new CoolingOffTransaction(_userId, _wallet.Id, _watchCategory.Id, 3_500_000m, "Đồng hồ", cooldown, 35m, 120_000m);
        _dbContext.CoolingOffTransactions.Add(tx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.CancelPurchaseAsync(_userId, tx.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        tx.Status.Should().Be(CoolingOffStatus.Cancelled);

        var freshWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        freshWallet.Balance.Should().Be(10_000_000m); // Preserved 100%
    }

    // UC-09.05 Test 5: Emergency bypass with valid reason deducts wallet and records transaction
    [Fact]
    public async Task BypassCoolingOff_WithValidReason_ShouldDeductWalletAndRecordTransaction()
    {
        // Arrange
        var cooldown = DateTime.UtcNow.AddHours(24);
        var tx = new CoolingOffTransaction(_userId, _wallet.Id, _watchCategory.Id, 2_000_000m, "Màn hình", cooldown, 20m, 80_000m);
        _dbContext.CoolingOffTransactions.Add(tx);
        await _dbContext.SaveChangesAsync();

        var request = new BypassCoolingOffRequestDto
        {
            Reason = "Màn hình phục vụ công việc bị nứt vỡ cần mua khẩn cấp."
        };

        // Act
        var result = await _service.BypassCoolingOffAsync(_userId, tx.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        tx.Status.Should().Be(CoolingOffStatus.Bypassed);

        // Verify wallet deducted
        var freshWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        freshWallet.Balance.Should().Be(8_000_000m);

        // Verify transaction recorded
        var realTx = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.UserId == _userId);
        realTx.Should().NotBeNull();
        realTx!.Amount.Should().Be(2_000_000m);
        realTx.Note.Should().Contain("Mở khóa khẩn cấp");
    }
}
