using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

public class SavingsGoalTests
{
    [Fact]
    public void Constructor_WithValidParameters_InstantiatesSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Quỹ Dự Phòng Khẩn Cấp";
        var targetAmount = 50_000_000m;
        var startDate = new DateTime(2026, 1, 1);
        var targetDate = new DateTime(2026, 12, 31);

        // Act
        var goal = new SavingsGoal(
            userId,
            name,
            targetAmount,
            startDate,
            targetDate,
            SavingsGoalType.EmergencyFund,
            SavingsGoalPriority.High
        );

        // Assert
        goal.Should().NotBeNull();
        goal.UserId.Should().Be(userId);
        goal.Name.Should().Be(name);
        goal.TargetAmount.Should().Be(targetAmount);
        goal.CurrentAmount.Should().Be(0m);
        goal.RemainingAmount.Should().Be(targetAmount);
        goal.ProgressPercent.Should().Be(0m);
        goal.Type.Should().Be(SavingsGoalType.EmergencyFund);
        goal.Priority.Should().Be(SavingsGoalPriority.High);
        goal.Status.Should().Be(SavingsGoalStatus.Active);
        goal.IsCompleted.Should().BeFalse();
        goal.IsEmergencyFund.Should().BeTrue();
        goal.Contributions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithInitialAmountEqualOrGreaterTarget_MarksCompleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var targetAmount = 20_000_000m;

        // Act
        var goal = new SavingsGoal(
            userId,
            "Mua Laptop Mới",
            targetAmount,
            DateTime.Today,
            DateTime.Today.AddMonths(6),
            SavingsGoalType.MajorPurchase,
            SavingsGoalPriority.Medium,
            initialAmount: 25_000_000m
        );

        // Assert
        goal.CurrentAmount.Should().Be(25_000_000m);
        goal.Status.Should().Be(SavingsGoalStatus.Completed);
        goal.IsCompleted.Should().BeTrue();
        goal.CompletedAt.Should().NotBeNull();
        goal.ProgressPercent.Should().Be(100m);
        goal.RemainingAmount.Should().Be(0m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_ThrowsArgumentException(string invalidName)
    {
        // Act
        var act = () => new SavingsGoal(
            Guid.NewGuid(),
            invalidName,
            10_000_000m,
            DateTime.Today,
            DateTime.Today.AddMonths(3)
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tên mục tiêu tích lũy không được để trống.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void Constructor_WithNonPositiveTargetAmount_ThrowsArgumentException(decimal invalidTarget)
    {
        // Act
        var act = () => new SavingsGoal(
            Guid.NewGuid(),
            "Học Chứng Chỉ AWS",
            invalidTarget,
            DateTime.Today,
            DateTime.Today.AddMonths(3)
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Số tiền mục tiêu tích lũy phải lớn hơn 0 đ.*");
    }

    [Fact]
    public void Constructor_WithTargetDateBeforeStartDate_ThrowsArgumentException()
    {
        // Act
        var act = () => new SavingsGoal(
            Guid.NewGuid(),
            "Du lịch Nhật Bản",
            30_000_000m,
            new DateTime(2026, 10, 1),
            new DateTime(2026, 9, 1)
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Hạn chót hoàn thành mục tiêu phải sau hoặc bằng ngày bắt đầu.*");
    }

    [Fact]
    public void Deposit_WithPositiveAmount_IncreasesBalanceAndCreatesContribution()
    {
        // Arrange
        var goal = new SavingsGoal(Guid.NewGuid(), "Quỹ Du Lịch", 10_000_000m, DateTime.Today, DateTime.Today.AddMonths(5));
        var walletId = Guid.NewGuid();

        // Act
        var contribution = goal.Deposit(3_000_000m, walletId, "Tiết kiệm lương tháng 10");

        // Assert
        goal.CurrentAmount.Should().Be(3_000_000m);
        goal.ProgressPercent.Should().Be(30m);
        goal.RemainingAmount.Should().Be(7_000_000m);
        goal.Contributions.Should().HaveCount(1);
        contribution.Amount.Should().Be(3_000_000m);
        contribution.Type.Should().Be(ContributionType.Deposit);
        contribution.WalletId.Should().Be(walletId);
    }

    [Fact]
    public void Deposit_WhenReachingTarget_MarksGoalCompleted()
    {
        // Arrange
        var goal = new SavingsGoal(Guid.NewGuid(), "Quỹ Tiết Kiệm", 5_000_000m, DateTime.Today, DateTime.Today.AddMonths(2));
        var walletId = Guid.NewGuid();

        // Act
        goal.Deposit(5_000_000m, walletId, "Nạp đủ mục tiêu");

        // Assert
        goal.CurrentAmount.Should().Be(5_000_000m);
        goal.Status.Should().Be(SavingsGoalStatus.Completed);
        goal.IsCompleted.Should().BeTrue();
        goal.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Withdraw_WithValidAmountAndReason_DecreasesBalance()
    {
        // Arrange
        var goal = new SavingsGoal(Guid.NewGuid(), "Quỹ Khẩn Cấp", 20_000_000m, DateTime.Today, DateTime.Today.AddMonths(12), initialAmount: 10_000_000m);
        var walletId = Guid.NewGuid();

        // Act
        var contribution = goal.Withdraw(4_000_000m, walletId, "Sửa xe máy bị hỏng đột xuất");

        // Assert
        goal.CurrentAmount.Should().Be(6_000_000m);
        goal.ProgressPercent.Should().Be(30m);
        goal.Contributions.Should().HaveCount(1);
        contribution.Amount.Should().Be(-4_000_000m);
        contribution.Type.Should().Be(ContributionType.Withdrawal);
        contribution.Note.Should().Be("Sửa xe máy bị hỏng đột xuất");
    }

    [Fact]
    public void Withdraw_ExceedingCurrentBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var goal = new SavingsGoal(Guid.NewGuid(), "Quỹ Đầu Tư", 50_000_000m, DateTime.Today, DateTime.Today.AddYears(2), initialAmount: 5_000_000m);

        // Act
        var act = () => goal.Withdraw(6_000_000m, Guid.NewGuid(), "Cần tiền đầu tư khác");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*vượt quá số dư hiện có trong quỹ*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Withdraw_WithEmptyReason_ThrowsArgumentException_EnforcingThalerNudge(string emptyReason)
    {
        // Arrange
        var goal = new SavingsGoal(Guid.NewGuid(), "Quỹ Học Phí", 15_000_000m, DateTime.Today, DateTime.Today.AddMonths(6), initialAmount: 5_000_000m);

        // Act
        var act = () => goal.Withdraw(1_000_000m, Guid.NewGuid(), emptyReason);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Vui lòng nêu rõ lý do rút tiền (Cú hích ma sát nhận thức Richard Thaler Nudge).*");
    }

    [Fact]
    public void CalculateMonthlyDepositRequired_CalculatesCorrectMonthlyAmount()
    {
        // Arrange
        var today = new DateTime(2026, 1, 1);
        var targetDate = new DateTime(2026, 6, 1); // 5 months
        var goal = new SavingsGoal(Guid.NewGuid(), "Quỹ Mua Xe", 25_000_000m, today, targetDate, initialAmount: 5_000_000m);

        // Act
        // Remaining = 20M, months = 5 => 4M/tháng
        var requiredMonthly = goal.CalculateMonthlyDepositRequired(today);

        // Assert
        requiredMonthly.Should().Be(4_000_000m);
    }

    [Fact]
    public void ConfigureAutoDeposit_WithValidSettings_EnablesAutoDeposit()
    {
        // Arrange
        var goal = new SavingsGoal(Guid.NewGuid(), "Quỹ Hưu Trí", 100_000_000m, DateTime.Today, DateTime.Today.AddYears(5));
        var walletId = Guid.NewGuid();

        // Act
        goal.ConfigureAutoDeposit(true, 2_000_000m, 5, walletId);

        // Assert
        goal.IsAutoDepositEnabled.Should().BeTrue();
        goal.AutoDepositAmount.Should().Be(2_000_000m);
        goal.AutoDepositDayOfMonth.Should().Be(5);
        goal.AutoDepositWalletId.Should().Be(walletId);
    }
}
