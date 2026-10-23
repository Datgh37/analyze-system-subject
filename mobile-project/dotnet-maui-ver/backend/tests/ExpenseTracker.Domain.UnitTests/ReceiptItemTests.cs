using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Unit tests validating ReceiptItem entity business rules (UC-03.07/03.08)
public class ReceiptItemTests
{
    private readonly Guid _transactionId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidParameters_ShouldInstantiateSuccessfully()
    {
        // Act
        var item = new ReceiptItem(
            transactionId: _transactionId,
            itemName: "Sữa chua Vinamilk lốc 4",
            quantity: 2,
            unitPrice: 28000m,
            totalPrice: 56000m,
            categoryId: _categoryId
        );

        // Assert
        item.Should().NotBeNull();
        item.TransactionId.Should().Be(_transactionId);
        item.ItemName.Should().Be("Sữa chua Vinamilk lốc 4");
        item.Quantity.Should().Be(2);
        item.UnitPrice.Should().Be(28000m);
        item.TotalPrice.Should().Be(56000m);
        item.CategoryId.Should().Be(_categoryId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WhenItemNameEmpty_ShouldThrowArgumentException(string? invalidName)
    {
        // Act
        var act = () => new ReceiptItem(
            transactionId: _transactionId,
            itemName: invalidName!,
            quantity: 1,
            unitPrice: 10000m,
            totalPrice: 10000m
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tên món hàng không được để trống*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenQuantityZeroOrNegative_ShouldThrowArgumentException(decimal invalidQuantity)
    {
        // Act
        var act = () => new ReceiptItem(
            transactionId: _transactionId,
            itemName: "Bánh mì gối",
            quantity: invalidQuantity,
            unitPrice: 20000m,
            totalPrice: 20000m
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Số lượng món hàng phải lớn hơn 0*");
    }

    [Fact]
    public void Constructor_WhenUnitPriceNegative_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new ReceiptItem(
            transactionId: _transactionId,
            itemName: "Bánh mì gối",
            quantity: 1,
            unitPrice: -5000m,
            totalPrice: 20000m
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Đơn giá không được âm*");
    }
}
