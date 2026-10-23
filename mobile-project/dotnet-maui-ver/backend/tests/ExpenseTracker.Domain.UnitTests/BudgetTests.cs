using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

public class BudgetTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesBudgetSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Ngân sách Tháng 10/2026";
        var totalAmount = 15_000_000m;
        var startDate = new DateTime(2026, 10, 1);
        var endDate = new DateTime(2026, 10, 31);

        // Act
        var budget = new Budget(userId, name, totalAmount, startDate, endDate, BudgetPeriodType.Monthly, BudgetMethod.Rule503020);

        // Assert
        budget.Should().NotBeNull();
        budget.UserId.Should().Be(userId);
        budget.Name.Should().Be(name);
        budget.TotalAmount.Should().Be(totalAmount);
        budget.PeriodType.Should().Be(BudgetPeriodType.Monthly);
        budget.Method.Should().Be(BudgetMethod.Rule503020);
        budget.Status.Should().Be(BudgetStatus.Active);
        budget.Allocations.Should().BeEmpty();
        budget.TotalAllocated.Should().Be(0m);
        budget.TotalSpent.Should().Be(0m);
        budget.TotalRemaining.Should().Be(totalAmount);
        budget.TotalDays.Should().Be(31);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_ThrowsArgumentException(string invalidName)
    {
        // Act
        var action = () => new Budget(Guid.NewGuid(), invalidName, 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Tên kế hoạch ngân sách không được để trống.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-500_000)]
    public void Constructor_WithNonPositiveAmount_ThrowsArgumentException(decimal invalidAmount)
    {
        // Act
        var action = () => new Budget(Guid.NewGuid(), "Budget", invalidAmount, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Tổng ngân sách cấp phát phải lớn hơn 0 đ.*");
    }

    [Fact]
    public void Constructor_WhenEndDateBeforeStartDate_ThrowsArgumentException()
    {
        // Act
        var action = () => new Budget(Guid.NewGuid(), "Budget", 5_000_000m, new DateTime(2026, 10, 31), new DateTime(2026, 10, 1));

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Ngày kết thúc ngân sách phải sau hoặc bằng ngày bắt đầu.*");
    }

    [Fact]
    public void AddAllocation_WithValidLimit_AddsAllocationAndUpdatesComputedProperties()
    {
        // Arrange
        var budget = new Budget(Guid.NewGuid(), "Tháng 10", 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        var catId1 = Guid.NewGuid();
        var catId2 = Guid.NewGuid();

        // Act
        budget.AddAllocation(catId1, 5_000_000m, 80m);
        budget.AddAllocation(catId2, 3_000_000m, 85m);

        // Assert
        budget.Allocations.Should().HaveCount(2);
        budget.TotalAllocated.Should().Be(8_000_000m);
        budget.TotalSpent.Should().Be(0m);
        budget.TotalRemaining.Should().Be(10_000_000m);
    }

    [Fact]
    public void AddAllocation_DuplicateCategory_ThrowsArgumentException()
    {
        // Arrange
        var budget = new Budget(Guid.NewGuid(), "Tháng 10", 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        var catId = Guid.NewGuid();
        budget.AddAllocation(catId, 2_000_000m);

        // Act
        var action = () => budget.AddAllocation(catId, 3_000_000m);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Danh mục này đã được phân bổ trong kế hoạch ngân sách.*");
    }

    [Fact]
    public void RecordExpense_UpdatesCurrentSpentAndCalculatesRemainingCorrectly()
    {
        // Arrange
        var budget = new Budget(Guid.NewGuid(), "Tháng 10", 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        var catId = Guid.NewGuid();
        budget.AddAllocation(catId, 5_000_000m, 80m);

        // Act
        budget.RecordExpense(catId, 2_000_000m);

        // Assert
        var alloc = budget.Allocations.First();
        alloc.CurrentSpent.Should().Be(2_000_000m);
        alloc.RemainingAmount.Should().Be(3_000_000m);
        alloc.SpentPercent.Should().Be(40m);
        alloc.IsAlertTriggered.Should().BeFalse();
        alloc.IsExceeded.Should().BeFalse();

        budget.TotalSpent.Should().Be(2_000_000m);
        budget.TotalRemaining.Should().Be(8_000_000m);
        budget.OverallSpentPercent.Should().Be(20m);
    }

    [Fact]
    public void RecordExpense_WhenReachingThreshold_TriggersAlert()
    {
        // Arrange
        var budget = new Budget(Guid.NewGuid(), "Tháng 10", 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        var catId = Guid.NewGuid();
        budget.AddAllocation(catId, 1_000_000m, 80m);

        // Act: Spend 850,000 VND (85% >= 80%)
        budget.RecordExpense(catId, 850_000m);

        // Assert
        var alloc = budget.Allocations.First();
        alloc.IsAlertTriggered.Should().BeTrue();
        alloc.IsExceeded.Should().BeFalse();
    }

    [Fact]
    public void RebalanceAllocation_UpdatesAllocatedLimit()
    {
        // Arrange
        var budget = new Budget(Guid.NewGuid(), "Tháng 10", 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        var catId = Guid.NewGuid();
        budget.AddAllocation(catId, 2_000_000m);

        // Act
        budget.RebalanceAllocation(catId, 3_500_000m);

        // Assert
        budget.Allocations.First().AllocatedAmount.Should().Be(3_500_000m);
    }

    [Fact]
    public void DailySpendingAllowance_CalculatesRemainingTodayCorrectly()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;
        var dsa = new DailySpendingAllowance(budgetId, today, 200_000m, 50_000m, 0.95m);

        // Assert
        dsa.DailyAllowanceAmount.Should().Be(200_000m);
        dsa.ActualSpentToday.Should().Be(50_000m);
        dsa.RemainingAllowanceToday.Should().Be(150_000m);
        dsa.VelocityIndex.Should().Be(0.95m);

        // Act: spend 200,000 VND more (total spent = 250,000 > 200,000)
        dsa.RecordExpenseToday(200_000m);

        // Assert
        dsa.ActualSpentToday.Should().Be(250_000m);
        dsa.RemainingAllowanceToday.Should().Be(-50_000m);
    }
}
