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
)
{
    public string DisplayCategoryIcon => !string.IsNullOrWhiteSpace(CategoryIcon)
        ? CategoryIcon
        : (CategoryName?.ToLowerInvariant() switch
        {
            null => TransactionType == 1 ? "📈" : "📉",
            var n when n.Contains("ăn") || n.Contains("uống") || n.Contains("food") || n.Contains("bữa") => "🍜",
            var n when n.Contains("mua") || n.Contains("sắm") || n.Contains("shop") || n.Contains("chợ") || n.Contains("siêu thị") => "🛍️",
            var n when n.Contains("xe") || n.Contains("xăng") || n.Contains("grab") || n.Contains("di chuyển") => "🚗",
            var n when n.Contains("điện") || n.Contains("nước") || n.Contains("nhà") || n.Contains("bill") || n.Contains("mạng") => "💡",
            var n when n.Contains("lương") || n.Contains("thu nhập") || n.Contains("thưởng") || n.Contains("salary") => "💰",
            var n when n.Contains("học") || n.Contains("sách") || n.Contains("khóa học") => "📚",
            var n when n.Contains("chơi") || n.Contains("giải trí") || n.Contains("phim") || n.Contains("game") => "🎬",
            var n when n.Contains("y tế") || n.Contains("thuốc") || n.Contains("khám") => "💊",
            var n when n.Contains("cafe") || n.Contains("cà phê") || n.Contains("trà") => "☕",
            _ => TransactionType == 1 ? "📈" : "📉"
        });

    public string DisplayCategoryBg => !string.IsNullOrWhiteSpace(CategoryColor)
        ? CategoryColor
        : (TransactionType == 1 ? "#ECFDF5" : "#FFF1F2");

    public string DisplayTypePrefix => TransactionType == 1 ? "+" : "-";

    public string DisplayVectorIcon => (CategoryName?.ToLowerInvariant()) switch
    {
        var n when n != null && (n.Contains("lương") || n.Contains("thu nhập") || n.Contains("thưởng")) => "ic_income.png",
        var n when n != null && (n.Contains("ví") || n.Contains("tiền")) => "ic_wallet.png",
        var n when n != null && (n.Contains("ngân sách") || n.Contains("hạn mức")) => "ic_budget.png",
        var n when n != null && (n.Contains("tích lũy") || n.Contains("tiết kiệm")) => "ic_savings.png",
        var n when n != null && (n.Contains("quy tắc") || n.Contains("tự động")) => "ic_rules.png",
        _ => TransactionType == 1 ? "ic_income.png" : "ic_expense.png"
    };
}

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
