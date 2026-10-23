using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Unit tests validating Wallet domain invariants, debit/credit rules and credit card constraints
public class WalletTests
{
    [Fact]
    public void Credit_ShouldIncreaseBalance_AndIncrementVersion()
    {
        // Arrange
        var wallet = new Wallet(Guid.NewGuid(), "Tiền mặt", WalletType.Cash, 100000m);

        // Act
        wallet.Credit(50000m);

        // Assert
        Assert.Equal(150000m, wallet.Balance);
        Assert.Equal(2, wallet.Version);
    }

    [Fact]
    public void Debit_ShouldDecreaseBalance_AndIncrementVersion()
    {
        // Arrange
        var wallet = new Wallet(Guid.NewGuid(), "Tiền mặt", WalletType.Cash, 100000m);

        // Act
        wallet.Debit(40000m);

        // Assert
        Assert.Equal(60000m, wallet.Balance);
        Assert.Equal(2, wallet.Version);
    }

    [Fact]
    public void Debit_WhenExceedingBalanceForCash_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var wallet = new Wallet(Guid.NewGuid(), "Tiền mặt", WalletType.Cash, 100000m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => wallet.Debit(150000m));
    }

    [Fact]
    public void Debit_WhenCreditCardWithinLimit_ShouldSucceed()
    {
        // Arrange
        var wallet = new Wallet(
            Guid.NewGuid(),
            "Visa Platinum",
            WalletType.CreditCard,
            0m,
            creditLimit: 50000000m
        );

        // Act
        wallet.Debit(20000000m);

        // Assert
        Assert.Equal(-20000000m, wallet.Balance);
    }

    [Fact]
    public void Debit_WhenCreditCardExceedsLimit_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var wallet = new Wallet(
            Guid.NewGuid(),
            "Visa Platinum",
            WalletType.CreditCard,
            0m,
            creditLimit: 10000000m
        );

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => wallet.Debit(15000000m));
    }

    [Fact]
    public void SetReconciledBalance_ShouldOverwriteBalance_AndIncrementVersion()
    {
        // Arrange
        var wallet = new Wallet(Guid.NewGuid(), "Ví Momo", WalletType.EWallet, 50000m);

        // Act
        wallet.SetReconciledBalance(80000m);

        // Assert
        Assert.Equal(80000m, wallet.Balance);
        Assert.Equal(2, wallet.Version);
    }
}
