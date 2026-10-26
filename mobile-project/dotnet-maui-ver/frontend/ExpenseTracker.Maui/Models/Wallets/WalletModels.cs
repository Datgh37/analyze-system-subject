namespace ExpenseTracker.Maui.Models.Wallets;

// Enumeration of wallet types for UI categorization
public enum MobileWalletType
{
    Cash = 1,
    BankAccount = 2,
    EWallet = 3,
    CreditCard = 4
}

// Data model representing a wallet on mobile client
public record WalletModel(
    Guid Id,
    Guid UserId,
    string Name,
    MobileWalletType WalletType,
    decimal Balance,
    decimal InitialBalance,
    string Currency,
    string? AccountNumber,
    string? BankName,
    decimal? CreditLimit,
    int? StatementDay,
    int? PaymentDueDay,
    string ColorHex,
    string IconKey,
    int DisplayOrder,
    bool IsExcludedFromTotal,
    int Status,
    int Version,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public string FormattedBalance => $"{Balance:N0} {Currency}";

    public string WalletTypeName => WalletType switch
    {
        MobileWalletType.Cash => "Tiền mặt",
        MobileWalletType.BankAccount => "Ngân hàng",
        MobileWalletType.EWallet => "Ví điện tử",
        MobileWalletType.CreditCard => "Thẻ tín dụng",
        _ => "Ví cá nhân"
    };

    public string SubtitleInfo => !string.IsNullOrEmpty(BankName)
        ? $"{BankName} • {AccountNumber}"
        : WalletTypeName;

    public string DisplayIcon => IconKey?.ToLowerInvariant() switch
    {
        "wallet" or "cash" => "💵",
        "bank" => "🏦",
        "momo" or "ewallet" => "📱",
        "credit" or "card" => "💳",
        _ => WalletType switch
        {
            MobileWalletType.Cash => "💵",
            MobileWalletType.BankAccount => "🏦",
            MobileWalletType.EWallet => "📱",
            MobileWalletType.CreditCard => "💳",
            _ => "💰"
        }
    };

    public string DisplayVectorIcon => IconKey?.ToLowerInvariant() switch
    {
        "wallet" or "cash" => "ic_cash.png",
        "bank" => "ic_bank.png",
        "momo" or "ewallet" => "ic_ewallet.png",
        "credit" or "card" => "ic_card.png",
        _ => WalletType switch
        {
            MobileWalletType.Cash => "ic_cash.png",
            MobileWalletType.BankAccount => "ic_bank.png",
            MobileWalletType.EWallet => "ic_ewallet.png",
            MobileWalletType.CreditCard => "ic_card.png",
            _ => "ic_wallet.png"
        }
    };
}

// Request model for creating a wallet from mobile app
public record CreateWalletModel(
    string Name,
    MobileWalletType WalletType,
    decimal InitialBalance,
    string Currency = "VND",
    string? AccountNumber = null,
    string? BankName = null,
    decimal? CreditLimit = null,
    int? StatementDay = null,
    int? PaymentDueDay = null,
    string ColorHex = "#0066FF",
    string IconKey = "wallet",
    bool IsExcludedFromTotal = false
);

// Request model for updating a wallet
public record UpdateWalletModel(
    string Name,
    string? AccountNumber = null,
    string? BankName = null,
    decimal? CreditLimit = null,
    int? StatementDay = null,
    int? PaymentDueDay = null,
    string ColorHex = "#0066FF",
    string IconKey = "wallet",
    bool IsExcludedFromTotal = false
);

// Request model for partially updating an existing wallet (HTTP PATCH)
public record PatchWalletModel(
    string? Name = null,
    string? ColorHex = null,
    string? IconKey = null,
    bool? IsExcludedFromTotal = null,
    int? Status = null
);

// Request model for transferring money between wallets
public record TransferMoneyModel(
    Guid SourceWalletId,
    Guid DestinationWalletId,
    decimal Amount,
    DateTime TransferDate,
    string? Note = null
);

// Request model for balance adjustment
public record AdjustBalanceModel(
    decimal NewBalance,
    string? Reason = null,
    DateTime? AdjustmentDate = null,
    string? ProofImageUrl = null
);

// Data model for balance adjustment history
public record BalanceAdjustmentModel(
    Guid Id,
    Guid WalletId,
    decimal OldBalance,
    decimal NewBalance,
    decimal DifferenceAmount,
    string? Reason,
    DateTime AdjustmentDate,
    string? ProofImageUrl,
    DateTime CreatedAt
);
