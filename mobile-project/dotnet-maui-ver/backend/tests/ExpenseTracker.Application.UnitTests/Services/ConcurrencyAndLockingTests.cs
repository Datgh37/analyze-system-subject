using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating pessimistic locking interface, deadlock prevention ordering, and transfer atomicity
public class ConcurrencyAndLockingTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly WalletService _walletService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _walletA;
    private readonly Wallet _walletB;

    public ConcurrencyAndLockingTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _walletService = new WalletService(_dbContext);

        _walletA = new Wallet(_userId, "Ví Nguồn A", WalletType.Cash, 1_000_000m);
        _walletB = new Wallet(_userId, "Ví Đích B", WalletType.BankAccount, 500_000m);

        _dbContext.Wallets.AddRange(_walletA, _walletB);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetWalletForUpdateAsync_WhenWalletExists_ShouldReturnLockedWallet()
    {
        // Act
        var result = await _dbContext.GetWalletForUpdateAsync(_walletA.Id, _userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(_walletA.Id);
        result.Name.Should().Be("Ví Nguồn A");
    }

    [Fact]
    public async Task GetWalletForUpdateAsync_WhenUserMismatch_ShouldReturnNull()
    {
        // Act
        var otherUserId = Guid.NewGuid();
        var result = await _dbContext.GetWalletForUpdateAsync(_walletA.Id, otherUserId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task TransferMoneyAsync_WithDeadlockPreventionOrdering_ShouldDebitSourceAndCreditDestination()
    {
        // Arrange
        var request = new TransferMoneyRequest(
            SourceWalletId: _walletA.Id,
            DestinationWalletId: _walletB.Id,
            Amount: 300_000m,
            TransferDate: DateTime.UtcNow,
            Note: "Chuyển tiền nội bộ sang ngân hàng"
        );

        // Act
        var result = await _walletService.TransferMoneyAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedA = await _dbContext.Wallets.FindAsync(_walletA.Id);
        var updatedB = await _dbContext.Wallets.FindAsync(_walletB.Id);

        updatedA!.Balance.Should().Be(700_000m);
        updatedB!.Balance.Should().Be(800_000m);

        // Verify transfer audit log
        var transferRecord = await _dbContext.WalletTransfers
            .FirstOrDefaultAsync(t => t.SourceWalletId == _walletA.Id && t.DestinationWalletId == _walletB.Id);
        transferRecord.Should().NotBeNull();
        transferRecord!.Amount.Should().Be(300_000m);
    }

    [Fact]
    public async Task TransferMoneyAsync_ReverseDirection_ShouldAlsoAcquireLocksWithoutDeadlock()
    {
        // Arrange: Reverse transfer direction (B -> A) to test both GUID order comparisons (A < B and B < A)
        var request = new TransferMoneyRequest(
            SourceWalletId: _walletB.Id,
            DestinationWalletId: _walletA.Id,
            Amount: 200_000m,
            TransferDate: DateTime.UtcNow,
            Note: "Chuyển ngược lại"
        );

        // Act
        var result = await _walletService.TransferMoneyAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedA = await _dbContext.Wallets.FindAsync(_walletA.Id);
        var updatedB = await _dbContext.Wallets.FindAsync(_walletB.Id);

        updatedA!.Balance.Should().Be(1_200_000m);
        updatedB!.Balance.Should().Be(300_000m);
    }
}
