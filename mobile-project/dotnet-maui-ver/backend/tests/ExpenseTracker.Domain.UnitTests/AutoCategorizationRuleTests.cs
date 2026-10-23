using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Unit tests validating AutoCategorizationRule entity business rules and domain invariants (UC-04.07)
public class AutoCategorizationRuleTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _targetCategoryId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidParameters_ShouldInstantiateSuccessfully()
    {
        // Act
        var rule = new AutoCategorizationRule(
            userId: _userId,
            keywordPattern: "WinMart",
            targetCategoryId: _targetCategoryId,
            priorityOrder: 1,
            isRegex: false,
            isCaseSensitive: false
        );

        // Assert
        rule.Should().NotBeNull();
        rule.UserId.Should().Be(_userId);
        rule.KeywordPattern.Should().Be("WinMart");
        rule.TargetCategoryId.Should().Be(_targetCategoryId);
        rule.PriorityOrder.Should().Be(1);
        rule.IsRegex.Should().BeFalse();
        rule.IsCaseSensitive.Should().BeFalse();
        rule.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    [InlineData(null)]
    public void Constructor_WhenKeywordTooShortOrEmpty_ShouldThrowArgumentException(string? invalidPattern)
    {
        // Act
        var act = () => new AutoCategorizationRule(
            userId: _userId,
            keywordPattern: invalidPattern!,
            targetCategoryId: _targetCategoryId
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*tối thiểu 2 ký tự*");
    }

    [Fact]
    public void Constructor_WhenTargetCategoryIdEmpty_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new AutoCategorizationRule(
            userId: _userId,
            keywordPattern: "Starbucks",
            targetCategoryId: Guid.Empty
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Danh mục đích không được để trống*");
    }

    [Fact]
    public void Constructor_WhenPriorityOrderNegative_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new AutoCategorizationRule(
            userId: _userId,
            keywordPattern: "Highlands",
            targetCategoryId: _targetCategoryId,
            priorityOrder: -1
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Thứ tự ưu tiên không được âm*");
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateCorrectly()
    {
        // Arrange
        var rule = new AutoCategorizationRule(_userId, "GrabFood", _targetCategoryId);
        var newCategoryId = Guid.NewGuid();

        // Act
        rule.UpdateDetails("ShopeeFood", newCategoryId, 2, true, true);

        // Assert
        rule.KeywordPattern.Should().Be("ShopeeFood");
        rule.TargetCategoryId.Should().Be(newCategoryId);
        rule.PriorityOrder.Should().Be(2);
        rule.IsRegex.Should().BeTrue();
        rule.IsCaseSensitive.Should().BeTrue();
        rule.UpdatedAt.Should().NotBeNull();
    }
}
