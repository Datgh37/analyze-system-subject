namespace ExpenseTracker.Maui.Models.Receipts;

// Item line parsed by AI Vision OCR from retail/supermarket receipts (UC-03.07/03.08)
public class ParsedReceiptItemModel
{
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1m;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public Guid? SuggestedCategoryId { get; set; }
    public string? SuggestedCategoryName { get; set; }
    public double Confidence { get; set; }

    // UI Helper properties
    public string DisplayQuantityPrice => $"{Quantity:N0} x {UnitPrice:N0} đ = {TotalPrice:N0} đ";
    public string CategoryBadge => string.IsNullOrEmpty(SuggestedCategoryName) ? "Chưa phân loại" : SuggestedCategoryName;
    public bool HasCategory => SuggestedCategoryId.HasValue;
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
