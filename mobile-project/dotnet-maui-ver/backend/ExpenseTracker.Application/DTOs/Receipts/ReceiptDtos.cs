using ExpenseTracker.Application.DTOs.Transactions;

namespace ExpenseTracker.Application.DTOs.Receipts;

// Item line parsed by AI Vision OCR from retail/supermarket receipts (UC-03.07/03.08)
public record ParsedReceiptItemDto(
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? SuggestedCategoryId,
    string? SuggestedCategoryName,
    double Confidence
);

// Comprehensive result of receipt extraction and deterministic mathematical audit
public record ReceiptParsedDataDto(
    string MerchantName,
    DateTime InvoiceDate,
    List<ParsedReceiptItemDto> Items,
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
public record ConfirmReceiptRequest(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? MerchantName = null,
    string? Note = null,
    bool IsEssential = false,
    List<CreateReceiptItemRequest>? Items = null,
    bool RememberRules = false
);
