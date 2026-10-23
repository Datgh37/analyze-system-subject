namespace ExpenseTracker.Maui.Models.Transactions;

// Transaction presentation model matching backend DTO
public record TransactionModel(
    Guid Id,
    Guid UserId,
    Guid WalletId,
    string WalletName,
    Guid CategoryId,
    string CategoryName,
    string CategoryIcon,
    string CategoryColor,
    decimal Amount,
    int TransactionType,
    DateTime TransactionDate,
    string? Note,
    bool IsEssential,
    bool IsWindfall,
    bool IsRecurring,
    bool ExcludeFromReport,
    DateTime CreatedAt
);

public record CreateIncomeModel(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsWindfall = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false
);

public record UpdateIncomeModel(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsWindfall = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false
);

public record CreateReceiptItemModel(
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? CategoryId = null
);

public record ReceiptItemModel(
    Guid Id,
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? CategoryId,
    string? CategoryName
);

public record CreateExpenseModel(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsEssential = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false,
    List<CreateReceiptItemModel>? Items = null
);

public record UpdateExpenseModel(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsEssential = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false,
    List<CreateReceiptItemModel>? Items = null
);

public record ExpenseDetailModel(
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
    List<ReceiptItemModel> Items,
    DateTime CreatedAt
);
