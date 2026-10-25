using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Wallet aggregate root managing multi-wallet balance and ACID transaction invariants
public class Wallet : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;
    public WalletType WalletType { get; private set; } = WalletType.Cash;
    public decimal Balance { get; private set; }
    public decimal InitialBalance { get; private set; }
    public string Currency { get; private set; } = "VND";

    // Optional banking and credit card properties
    public string? AccountNumber { get; private set; }
    public string? BankName { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public int? StatementDay { get; private set; }
    public int? PaymentDueDay { get; private set; }

    // UI presentation and reporting settings
    public string ColorHex { get; private set; } = "#0066FF";
    public string IconKey { get; private set; } = "wallet";
    public int DisplayOrder { get; private set; }
    public bool IsExcludedFromTotal { get; private set; }
    public WalletStatus Status { get; private set; } = WalletStatus.Active;
    public int Version { get; private set; } = 1;

    // Navigation collections for transactions and audit balance adjustments
    public ICollection<BalanceAdjustment> BalanceAdjustments { get; private set; } = new List<BalanceAdjustment>();
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    // Parameterless constructor for EF Core materialization
    private Wallet() { }

    public Wallet(
        Guid userId,
        string name,
        WalletType walletType,
        decimal initialBalance,
        string currency = "VND",
        string? accountNumber = null,
        string? bankName = null,
        decimal? creditLimit = null,
        int? statementDay = null,
        int? paymentDueDay = null,
        string colorHex = "#0066FF",
        string iconKey = "wallet",
        bool isExcludedFromTotal = false)
    {
        UserId = userId;
        Name = name.Trim();
        WalletType = walletType;
        InitialBalance = initialBalance;
        Balance = initialBalance;
        Currency = string.IsNullOrWhiteSpace(currency) ? "VND" : currency.Trim().ToUpperInvariant();
        AccountNumber = accountNumber?.Trim();
        BankName = bankName?.Trim();
        CreditLimit = creditLimit;
        StatementDay = statementDay;
        PaymentDueDay = paymentDueDay;
        ColorHex = string.IsNullOrWhiteSpace(colorHex) ? "#0066FF" : colorHex.Trim();
        IconKey = string.IsNullOrWhiteSpace(iconKey) ? "wallet" : iconKey.Trim();
        IsExcludedFromTotal = isExcludedFromTotal;
        Status = WalletStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    // Credits money into wallet balance
    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền nạp phải lớn hơn 0.", nameof(amount));

        Balance += amount;
        Version++;
        UpdatedAt = DateTime.UtcNow;
    }

    // Debits money from wallet balance ensuring non-negative constraints for cash/bank
    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền trừ phải lớn hơn 0.", nameof(amount));

        if (WalletType != WalletType.CreditCard && Balance - amount < 0)
            throw new InvalidOperationException("Số dư khả dụng trong ví không đủ để thực hiện giao dịch.");

        if (WalletType == WalletType.CreditCard && CreditLimit.HasValue)
        {
            // For credit card, negative balance represents debt
            if (Math.Abs(Balance - amount) > CreditLimit.Value && Balance - amount < 0)
                throw new InvalidOperationException("Giao dịch vượt quá hạn mức tín dụng cho phép.");
        }

        Balance -= amount;
        Version++;
        UpdatedAt = DateTime.UtcNow;
    }

    // Updates wallet configuration and metadata
    public void UpdateDetails(
        string name,
        string? accountNumber,
        string? bankName,
        decimal? creditLimit,
        int? statementDay,
        int? paymentDueDay,
        string colorHex,
        string iconKey,
        bool isExcludedFromTotal)
    {
        Name = name.Trim();
        AccountNumber = accountNumber?.Trim();
        BankName = bankName?.Trim();
        CreditLimit = creditLimit;
        StatementDay = statementDay;
        PaymentDueDay = paymentDueDay;
        ColorHex = string.IsNullOrWhiteSpace(colorHex) ? "#0066FF" : colorHex.Trim();
        IconKey = string.IsNullOrWhiteSpace(iconKey) ? "wallet" : iconKey.Trim();
        IsExcludedFromTotal = isExcludedFromTotal;
        UpdatedAt = DateTime.UtcNow;
    }

    // Overwrites balance during reconciliation and increments version
    public void SetReconciledBalance(decimal newBalance)
    {
        Balance = newBalance;
        Version++;
        UpdatedAt = DateTime.UtcNow;
    }

    // Changes archive status
    public void Archive()
    {
        Status = WalletStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    // Activates archived wallet
    public void Activate()
    {
        Status = WalletStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    // Partially updates wallet properties via HTTP PATCH (RFC 7396)
    public void PatchDetails(
        string? name = null,
        string? colorHex = null,
        string? iconKey = null,
        bool? isExcludedFromTotal = null,
        WalletStatus? status = null)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
        if (!string.IsNullOrWhiteSpace(colorHex)) ColorHex = colorHex.Trim();
        if (!string.IsNullOrWhiteSpace(iconKey)) IconKey = iconKey.Trim();
        if (isExcludedFromTotal.HasValue) IsExcludedFromTotal = isExcludedFromTotal.Value;
        if (status.HasValue) Status = status.Value;
        UpdatedAt = DateTime.UtcNow;
    }
}
