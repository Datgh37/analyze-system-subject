using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating ACID wallet balance synchronization during income lifecycle
public class TransactionServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TransactionService _transactionService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _category;

    public TransactionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _transactionService = new TransactionService(_dbContext);

        _wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 1_000_000m);
        _category = new Category("Tiền lương", TransactionType.Income, isSystem: true);

        _dbContext.Wallets.Add(_wallet);
        _dbContext.Categories.Add(_category);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task CreateIncomeAsync_ShouldCreditWalletBalance_AndPersistTransaction()
    {
        // Arrange
        var request = new CreateIncomeTransactionRequest(
            WalletId: _wallet.Id,
            CategoryId: _category.Id,
            Amount: 5_000_000m,
            TransactionDate: DateTime.UtcNow,
            Note: "Lương tháng 10"
        );

        // Act
        var result = await _transactionService.CreateIncomeAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(5_000_000m);

        // Verify wallet credited: 1,000,000 + 5,000,000 = 6,000,000
        var updatedWallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        updatedWallet!.Balance.Should().Be(6_000_000m);
    }

    [Fact]
    public async Task UpdateIncomeAsync_WhenAmountIncreasedInSameWallet_ShouldCreditDelta()
    {
        // Arrange
        var createReq = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 2_000_000m, DateTime.UtcNow);
        var created = await _transactionService.CreateIncomeAsync(_userId, createReq);
        // Balance: 1,000,000 + 2,000,000 = 3,000,000

        var updateReq = new UpdateIncomeTransactionRequest(_wallet.Id, _category.Id, 3_500_000m, DateTime.UtcNow, Note: "Tăng thêm");

        // Act
        var result = await _transactionService.UpdateIncomeAsync(_userId, created.Value!.Id, updateReq);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(3_500_000m);

        // Delta: +1,500,000 -> New Balance: 3,000,000 + 1,500,000 = 4,500,000
        var updatedWallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        updatedWallet!.Balance.Should().Be(4_500_000m);
    }

    [Fact]
    public async Task UpdateIncomeAsync_WhenChangingWallet_ShouldRevertOldAndCreditNew()
    {
        // Arrange
        var secondWallet = new Wallet(_userId, "Ví Ngân Hàng", WalletType.BankAccount, 500_000m);
        await _dbContext.Wallets.AddAsync(secondWallet);
        await _dbContext.SaveChangesAsync();

        var createReq = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 1_000_000m, DateTime.UtcNow);
        var created = await _transactionService.CreateIncomeAsync(_userId, createReq);
        // Wallet 1: 1M + 1M = 2M. Wallet 2: 500K

        var updateReq = new UpdateIncomeTransactionRequest(secondWallet.Id, _category.Id, 1_000_000m, DateTime.UtcNow);

        // Act
        var result = await _transactionService.UpdateIncomeAsync(_userId, created.Value!.Id, updateReq);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Wallet 1 reverted: 2M - 1M = 1M
        var w1 = await _dbContext.Wallets.FindAsync(_wallet.Id);
        w1!.Balance.Should().Be(1_000_000m);

        // Wallet 2 credited: 500K + 1M = 1.5M
        var w2 = await _dbContext.Wallets.FindAsync(secondWallet.Id);
        w2!.Balance.Should().Be(1_500_000m);
    }

    [Fact]
    public async Task DeleteIncomeAsync_ShouldRevertCreditedAmount_AndSoftDelete()
    {
        // Arrange
        var createReq = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 2_000_000m, DateTime.UtcNow);
        var created = await _transactionService.CreateIncomeAsync(_userId, createReq);
        // Balance: 1M + 2M = 3M

        // Act
        var result = await _transactionService.DeleteIncomeAsync(_userId, created.Value!.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Wallet reverted: 3M - 2M = 1M
        var wallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        wallet!.Balance.Should().Be(1_000_000m);

        // Transaction soft deleted
        var tx = await _dbContext.Transactions.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == created.Value!.Id);
        tx.Should().NotBeNull();
        tx!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetTotalIncomeAmountAsync_ShouldSumOnlyActiveIncomesInSelectedMonth()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var req1 = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 3_000_000m, new DateTime(now.Year, now.Month, 5, 0, 0, 0, DateTimeKind.Utc));
        var req2 = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 2_000_000m, new DateTime(now.Year, now.Month, 10, 0, 0, 0, DateTimeKind.Utc));
        var reqOtherMonth = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 10_000_000m, new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1));

        await _transactionService.CreateIncomeAsync(_userId, req1);
        await _transactionService.CreateIncomeAsync(_userId, req2);
        await _transactionService.CreateIncomeAsync(_userId, reqOtherMonth);

        // Act
        var result = await _transactionService.GetTotalIncomeAmountAsync(_userId, now.Month, now.Year);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(5_000_000m);
    }

    [Fact]
    public async Task UpdateIncomeAsync_WhenInsufficientWalletBalanceToDebit_ShouldFailWithAccountingMessage()
    {
        // Arrange
        var createReq = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 5_000_000m, DateTime.UtcNow);
        var created = await _transactionService.CreateIncomeAsync(_userId, createReq);
        // Wallet balance now: 1M + 5M = 6M

        // Simulate user spending money from wallet down to 100K
        _wallet.Debit(5_900_000m);
        await _dbContext.SaveChangesAsync();

        // Try to update income down to 1M (delta = -4M, but wallet only has 100K)
        var updateReq = new UpdateIncomeTransactionRequest(_wallet.Id, _category.Id, 1_000_000m, DateTime.UtcNow);

        // Act
        var result = await _transactionService.UpdateIncomeAsync(_userId, created.Value!.Id, updateReq);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("số dư khả dụng hiện tại của ví không đủ để khấu trừ");
    }

    [Fact]
    public async Task DeleteIncomeAsync_WhenInsufficientWalletBalanceToDebit_ShouldFailWithAccountingMessage()
    {
        // Arrange
        var createReq = new CreateIncomeTransactionRequest(_wallet.Id, _category.Id, 5_000_000m, DateTime.UtcNow);
        var created = await _transactionService.CreateIncomeAsync(_userId, createReq);
        // Wallet balance now: 1M + 5M = 6M

        // Simulate user spending money down to 500K
        _wallet.Debit(5_500_000m);
        await _dbContext.SaveChangesAsync();

        // Act: Try to delete 5M income
        var result = await _transactionService.DeleteIncomeAsync(_userId, created.Value!.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("số dư khả dụng hiện tại của ví không đủ để khấu trừ");
    }
}
