using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Unit tests validating RecurringIncome domain invariants and due date calculation with short month alignment (UC-02.07)
public class RecurringIncomeTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _walletId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidArguments_ShouldInstantiateProperly()
    {
        // Arrange & Act
        var income = new RecurringIncome(
            _userId,
            "Lương hàng tháng",
            25000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 5
        );

        // Assert
        Assert.Equal("Lương hàng tháng", income.Name);
        Assert.Equal(25000000m, income.ExpectedAmount);
        Assert.Equal(_walletId, income.WalletId);
        Assert.Equal(_categoryId, income.CategoryId);
        Assert.Equal(RecurringFrequency.Monthly, income.Frequency);
        Assert.Equal(5, income.DayOfMonth);
        Assert.True(income.AutoRecord);
        Assert.True(income.IsActive);
        Assert.Null(income.LastGeneratedDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? name)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RecurringIncome(
            _userId,
            name!,
            10000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 1
        ));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void Constructor_WithNonPositiveAmount_ShouldThrowArgumentException(decimal amount)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RecurringIncome(
            _userId,
            "Tiền nhà",
            amount,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 1
        ));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void Constructor_WithInvalidDayOfMonth_ShouldThrowArgumentOutOfRangeException(int day)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecurringIncome(
            _userId,
            "Lương",
            10000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: day
        ));
    }

    [Fact]
    public void CalculateNextDueDate_Monthly_WithDay31_InFebruary_ShouldAlignToFeb28( )
    {
        // Arrange: Start on 31 Jan 2026, set dayOfMonth = 31 (UC-02.07 E1)
        var startDate = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Tiền hoa hồng",
            5000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 31,
            autoRecord: true,
            startDate: startDate
        );

        // Act
        var nextDue = income.CalculateNextDueDate(startDate);

        // Assert: 2026 is non-leap year -> Feb has 28 days
        Assert.Equal(2026, nextDue.Year);
        Assert.Equal(2, nextDue.Month);
        Assert.Equal(28, nextDue.Day);
    }

    [Fact]
    public void CalculateNextDueDate_Monthly_WithDay31_InApril_ShouldAlignToApr30()
    {
        // Arrange: From 31 March 2026, dayOfMonth = 31 (UC-02.07 E1)
        var baseDate = new DateTime(2026, 3, 31, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Lương quản lý",
            30000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 31,
            autoRecord: true,
            startDate: baseDate
        );

        // Act
        var nextDue = income.CalculateNextDueDate(baseDate);

        // Assert: April has 30 days
        Assert.Equal(2026, nextDue.Year);
        Assert.Equal(4, nextDue.Month);
        Assert.Equal(30, nextDue.Day);
    }

    [Fact]
    public void CalculateNextDueDate_Monthly_WhenBeforeTargetDayInCurrentMonth_ShouldStayInSameMonth()
    {
        // Arrange: Base date is 10 May 2026, DayOfMonth = 25
        var baseDate = new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Lương ngày 25",
            20000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 25,
            autoRecord: true,
            startDate: baseDate
        );

        // Assert: Initial due date should be 25 May 2026
        Assert.Equal(new DateTime(2026, 5, 25, 0, 0, 0, DateTimeKind.Utc), income.NextDueDate);
    }

    [Fact]
    public void CalculateNextDueDate_Weekly_ShouldAdvanceBy7Days()
    {
        // Arrange
        var baseDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Thu nhập theo tuần",
            2000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Weekly,
            dayOfMonth: 1,
            autoRecord: true,
            startDate: baseDate
        );

        // Act
        var nextDue = income.CalculateNextDueDate(baseDate);

        // Assert
        Assert.Equal(baseDate.AddDays(7), nextDue);
    }

    [Fact]
    public void MarkProcessed_ShouldSetLastGeneratedDate_AndAdvanceNextDueDate()
    {
        // Arrange
        var executionDate = new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Lương ngày 5",
            15000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 5,
            autoRecord: true,
            startDate: executionDate
        );

        // Act
        income.MarkProcessed(executionDate);

        // Assert
        Assert.Equal(executionDate, income.LastGeneratedDate);
        Assert.Equal(new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc), income.NextDueDate);
    }

    [Fact]
    public void ToggleStatus_ShouldFlipIsActive()
    {
        // Arrange
        var income = new RecurringIncome(
            _userId,
            "Thu nhập",
            10000000m,
            _walletId,
            _categoryId,
            RecurringFrequency.Monthly,
            dayOfMonth: 10
        );
        Assert.True(income.IsActive);

        // Act 1: Toggle off
        income.ToggleStatus();
        Assert.False(income.IsActive);

        // Act 2: Toggle on
        income.ToggleStatus();
        Assert.True(income.IsActive);
    }
}
