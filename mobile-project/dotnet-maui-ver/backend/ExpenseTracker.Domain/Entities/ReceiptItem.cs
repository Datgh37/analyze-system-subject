using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Itemized line item extracted from receipt or added to expense transaction (UC-03.07/03.08)
public class ReceiptItem : BaseEntity
{
    public Guid TransactionId { get; private set; }
    public Transaction Transaction { get; private set; } = null!;

    public string ItemName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    // Optional category mapping per item line
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    // Parameterless constructor for EF Core materialization
    private ReceiptItem() { }

    public ReceiptItem(
        Guid transactionId,
        string itemName,
        decimal quantity,
        decimal unitPrice,
        decimal totalPrice,
        Guid? categoryId = null)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Tên món hàng không được để trống.", nameof(itemName));

        if (quantity <= 0)
            throw new ArgumentException("Số lượng món hàng phải lớn hơn 0.", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Đơn giá không được âm.", nameof(unitPrice));

        if (totalPrice < 0)
            throw new ArgumentException("Thành tiền không được âm.", nameof(totalPrice));

        TransactionId = transactionId;
        ItemName = itemName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string itemName,
        decimal quantity,
        decimal unitPrice,
        decimal totalPrice,
        Guid? categoryId)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Tên món hàng không được để trống.", nameof(itemName));

        if (quantity <= 0)
            throw new ArgumentException("Số lượng món hàng phải lớn hơn 0.", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Đơn giá không được âm.", nameof(unitPrice));

        if (totalPrice < 0)
            throw new ArgumentException("Thành tiền không được âm.", nameof(totalPrice));

        ItemName = itemName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }
}
