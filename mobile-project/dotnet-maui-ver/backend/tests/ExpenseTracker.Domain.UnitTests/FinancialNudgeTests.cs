using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

public class FinancialNudgeTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Act
        var nudge = new FinancialNudge(
            _userId,
            NudgeType.DsaThreshold,
            "Cảnh báo hạn mức DSA",
            "Bạn đã tiêu 90% hạn mức ngày",
            "ExpenseCreated");

        // Assert
        nudge.Id.Should().NotBeEmpty();
        nudge.UserId.Should().Be(_userId);
        nudge.NudgeType.Should().Be(NudgeType.DsaThreshold);
        nudge.Title.Should().Be("Cảnh báo hạn mức DSA");
        nudge.Message.Should().Be("Bạn đã tiêu 90% hạn mức ngày");
        nudge.Status.Should().Be(NudgeStatus.Active);
        nudge.IsApprovedByUser.Should().BeNull();
        nudge.ResolvedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        // Act
        var act = () => new FinancialNudge(_userId, NudgeType.ZombieSubscription, invalidTitle!, "Valid message", "Trigger");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Approve_WhenActive_ShouldTransitionToResolvedAndSetApproved()
    {
        // Arrange
        var nudge = new FinancialNudge(_userId, NudgeType.DeficitRebalance, "Đề xuất bù trừ", "Bù trừ 500k", "RebalanceTrigger");

        // Act
        nudge.Approve();

        // Assert
        nudge.Status.Should().Be(NudgeStatus.Resolved);
        nudge.IsApprovedByUser.Should().BeTrue();
        nudge.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public void Dismiss_WhenActive_ShouldTransitionToDismissed()
    {
        // Arrange
        var nudge = new FinancialNudge(_userId, NudgeType.PayYourselfFirst, "Nhắc nhở tích lũy", "Trích 20% lương", "IncomeTrigger");

        // Act
        nudge.Dismiss();

        // Assert
        nudge.Status.Should().Be(NudgeStatus.Dismissed);
        nudge.IsApprovedByUser.Should().BeFalse();
        nudge.ResolvedAt.Should().NotBeNull();
    }
}
