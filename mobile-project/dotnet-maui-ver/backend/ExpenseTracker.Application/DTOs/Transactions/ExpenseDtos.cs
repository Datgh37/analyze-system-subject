namespace ExpenseTracker.Application.DTOs.Transactions;

// DTO representing itemized line item inside an expense
public record ReceiptItemDto(
    Guid Id,
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? CategoryId,
    string? CategoryName
);

// Request payload for line item creation
public record CreateReceiptItemRequest(
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? CategoryId = null
);

// Request to record a new expense transaction (UC-03.01)
public record CreateExpenseTransactionRequest(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsEssential = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false,
    List<CreateReceiptItemRequest>? Items = null
);

// Request to update an existing expense transaction (UC-03.04)
public record UpdateExpenseTransactionRequest(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsEssential = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false,
    List<CreateReceiptItemRequest>? Items = null
);

// Comprehensive expense details DTO including itemized receipt lines
public record ExpenseDetailDto(
    Guid Id,
    Guid UserId,
    Guid WalletId,
    string WalletName,
    Guid CategoryId,
    string CategoryName,
    string CategoryIcon,
    string CategoryColor,
    decimal Amount,
    DateTime TransactionDate,
    string? Note,
    bool IsEssential,
    bool IsRecurring,
    bool ExcludeFromReport,
    List<ReceiptItemDto> Items,
    DateTime CreatedAt
);
