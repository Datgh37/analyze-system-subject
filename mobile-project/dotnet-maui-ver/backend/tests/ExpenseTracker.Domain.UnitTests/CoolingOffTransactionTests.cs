using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

public class CoolingOffTransactionTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _walletId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var cooldown = DateTime.UtcNow.AddHours(24);

        // Act
        var tx = new CoolingOffTransaction(
            _userId,
            _walletId,
            _categoryId,
            5_000_000m,
            "Đồng hồ thông minh",
            cooldown,
            equivalentWorkHours: 41.6m,
            projectedDsaReduction: 150_000m);

        // Assert
        tx.Id.Should().NotBeEmpty();
        tx.Amount.Should().Be(5_000_000m);
        tx.Status.Should().Be(CoolingOffStatus.PendingCoolingOff);
        tx.EquivalentWorkHours.Should().Be(41.6m);
        tx.ProjectedDsaReduction.Should().Be(150_000m);
        tx.BypassReason.Should().BeNull();
    }

    [Fact]
    public void ConfirmPurchase_BeforeCooldownExpires_ShouldThrowInvalidOperationException()
    {
        // Arrange: Cooldown is 24h in the future
        var cooldown = DateTime.UtcNow.AddHours(24);
        var tx = new CoolingOffTransaction(_userId, _walletId, _categoryId, 3_000_000m, "Tai nghe", cooldown, 25m, 100_000m);

        // Act
        var act = () => tx.ConfirmPurchase();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cooling-off period has expired*");
    }

    [Fact]
    public void CancelPurchase_WhenPending_ShouldTransitionToCancelled()
    {
        // Arrange
        var cooldown = DateTime.UtcNow.AddHours(24);
        var tx = new CoolingOffTransaction(_userId, _walletId, _categoryId, 4_000_000m, "Loa bluetooth", cooldown, 33m, 120_000m);

        // Act
        tx.CancelPurchase();

        // Assert
        tx.Status.Should().Be(CoolingOffStatus.Cancelled);
        tx.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public void BypassWithReason_WhenValidReason_ShouldTransitionToBypassed()
    {
        // Arrange
        var cooldown = DateTime.UtcNow.AddHours(24);
        var tx = new CoolingOffTransaction(_userId, _walletId, _categoryId, 6_000_000m, "Điện thoại thay thế", cooldown, 50m, 200_000m);

        // Act
        tx.BypassWithReason("Điện thoại cũ bị hỏng đột xuất cần mua ngay để làm việc.");

        // Assert
        tx.Status.Should().Be(CoolingOffStatus.Bypassed);
        tx.BypassReason.Should().Contain("Điện thoại cũ bị hỏng");
        tx.ResolvedAt.Should().NotBeNull();
    }
}
