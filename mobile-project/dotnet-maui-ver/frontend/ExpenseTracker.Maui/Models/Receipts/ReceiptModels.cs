using CommunityToolkit.Mvvm.ComponentModel;

namespace ExpenseTracker.Maui.Models.Receipts;

// Item line parsed by AI Vision OCR from retail/supermarket receipts (UC-03.07/03.08)
public partial class ParsedReceiptItemModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayQuantityPrice))]
    private string _itemName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayQuantityPrice))]
    private decimal _quantity = 1m;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayQuantityPrice))]
    private decimal _unitPrice;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayQuantityPrice))]
    private decimal _totalPrice;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCategory))]
    private Guid? _suggestedCategoryId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CategoryBadge))]
    private string? _suggestedCategoryName;

    [ObservableProperty]
    private double _confidence = 0.95;

    // UI Helper properties
    public string DisplayQuantityPrice => $"{Quantity:N0} x {UnitPrice:N0} đ = {TotalPrice:N0} đ";
    public string CategoryBadge => string.IsNullOrEmpty(SuggestedCategoryName) ? "Chưa phân loại" : SuggestedCategoryName;
    public bool HasCategory => SuggestedCategoryId.HasValue;

    public void RecalculateTotalPrice()
    {
        TotalPrice = Quantity * UnitPrice;
    }
}

// Item payload to persist when confirming receipt
public record CreateReceiptItemModel(
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? CategoryId = null
);

// Comprehensive result of receipt extraction and deterministic mathematical audit
public record ReceiptParsedDataModel(
    string MerchantName,
    DateTime InvoiceDate,
    List<ParsedReceiptItemModel> Items,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    decimal CalculatedTotal,
    decimal DiscrepancyAmount,
    bool IsMathDiscrepancy,
    string? AuditWarningMessage,
    double ConfidenceScore
);

// Payload to confirm and commit parsed receipt into permanent financial ledger (UC-03.08 Step 6)
public record ConfirmReceiptModel(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? MerchantName = null,
    string? Note = null,
    bool IsEssential = false,
    List<CreateReceiptItemModel>? Items = null,
    bool RememberRules = false
);
