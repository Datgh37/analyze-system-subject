using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests.Entities;

public class SubscriptionTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidParameters_InitializesSubscriptionCorrectly()
    {
        // Arrange
        var nextBilling = DateTime.UtcNow.AddDays(10);

        // Act
        var sub = new Subscription(
            _userId,
            "Netflix Premium",
            260_000m,
            SubscriptionBillingCycle.Monthly,
            nextBilling,
            _categoryId,
            notes: "Gói xem phim 4K");

        // Assert
        sub.Id.Should().NotBeEmpty();
        sub.UserId.Should().Be(_userId);
        sub.Name.Should().Be("Netflix Premium");
        sub.Amount.Should().Be(260_000m);
        sub.BillingCycle.Should().Be(SubscriptionBillingCycle.Monthly);
        sub.Status.Should().Be(SubscriptionStatus.Active);
        sub.MonthlyCost.Should().Be(260_000m);
        sub.AnnualCost.Should().Be(3_120_000m);
        sub.RemindDaysBefore.Should().Be(3);
    }

    [Theory]
    [InlineData(SubscriptionBillingCycle.Weekly, 100_000, 5_200_000)]
    [InlineData(SubscriptionBillingCycle.Monthly, 200_000, 2_400_000)]
    [InlineData(SubscriptionBillingCycle.Quarterly, 600_000, 2_400_000)]
    [InlineData(SubscriptionBillingCycle.Yearly, 1_200_000, 1_200_000)]
    public void AnnualCost_CalculatesAccurateProjectionBasedOnCycle(SubscriptionBillingCycle cycle, decimal amount, decimal expectedAnnual)
    {
        // Act
        var sub = new Subscription(
            _userId,
            "Service",
            amount,
            cycle,
            DateTime.UtcNow.AddDays(15),
            _categoryId);

        // Assert
        sub.AnnualCost.Should().Be(expectedAnnual);
    }

    [Fact]
    public void RenewToNextCycle_AdvancesBillingDateAccurately()
    {
        // Arrange
        var initialDate = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        var sub = new Subscription(
            _userId,
            "Spotify",
            59_000m,
            SubscriptionBillingCycle.Monthly,
            initialDate,
            _categoryId,
            status: SubscriptionStatus.FreeTrial);

        // Act
        sub.RenewToNextCycle();

        // Assert
        sub.NextBillingDate.Should().Be(new DateTime(2026, 11, 15, 0, 0, 0, DateTimeKind.Utc));
        sub.Status.Should().Be(SubscriptionStatus.Active); // FreeTrial chuyển thành Active sau kỳ đầu
    }

    [Fact]
    public void Cancel_UpdatesStatusToCancelled()
    {
        // Arrange
        var sub = new Subscription(_userId, "iCloud", 19_000m, SubscriptionBillingCycle.Monthly, DateTime.UtcNow.AddDays(5), _categoryId);

        // Act
        sub.Cancel();

        // Assert
        sub.Status.Should().Be(SubscriptionStatus.Cancelled);
        sub.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WhenEmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => new Subscription(_userId, "", 100_000m, SubscriptionBillingCycle.Monthly, DateTime.UtcNow, _categoryId);
        act.Should().Throw<ArgumentException>().WithMessage("*không được để trống*");
    }

    [Fact]
    public void Constructor_WhenNegativeAmountOnActive_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => new Subscription(_userId, "Gym", -50_000m, SubscriptionBillingCycle.Monthly, DateTime.UtcNow, _categoryId);
        act.Should().Throw<ArgumentException>().WithMessage("*phải lớn hơn 0*");
    }
}

