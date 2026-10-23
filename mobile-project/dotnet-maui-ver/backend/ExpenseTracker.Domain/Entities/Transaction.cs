using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Core financial ledger transaction record (Income and Expenses)
public class Transaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public decimal Amount { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string? Note { get; private set; }

    // Behavioral and analytics flags
    public bool IsEssential { get; private set; }
    public bool IsWindfall { get; private set; }
    public bool IsRecurring { get; private set; }
    public bool ExcludeFromReport { get; private set; }

    // Optional 1-1 relationship with Payslip details (UC-02.08)
    public SalarySlipDetail? SalarySlipDetail { get; private set; }

    // Optional 1-N relationship with itemized receipt lines (UC-03.07/03.08)
    public ICollection<ReceiptItem> ReceiptItems { get; private set; } = new List<ReceiptItem>();

    // Parameterless constructor for EF Core materialization
    private Transaction() { }

    public Transaction(
        Guid userId,
        Guid walletId,
        Guid categoryId,
        decimal amount,
        TransactionType transactionType,
        DateTime transactionDate,
        string? note = null,
        bool isEssential = false,
        bool isWindfall = false,
        bool isRecurring = false,
        bool excludeFromReport = false)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền giao dịch phải lớn hơn 0.", nameof(amount));

        UserId = userId;
        WalletId = walletId;
        CategoryId = categoryId;
        Amount = amount;
        TransactionType = transactionType;
        TransactionDate = transactionDate.Kind == DateTimeKind.Utc 
            ? transactionDate 
            : DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc);
        Note = note?.Trim();
        IsEssential = isEssential;
        IsWindfall = isWindfall;
        IsRecurring = isRecurring;
        ExcludeFromReport = excludeFromReport;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        Guid walletId,
        Guid categoryId,
        decimal amount,
        DateTime transactionDate,
        string? note,
        bool isEssential,
        bool isWindfall,
        bool isRecurring,
        bool excludeFromReport)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền giao dịch phải lớn hơn 0.", nameof(amount));

        WalletId = walletId;
        CategoryId = categoryId;
        Amount = amount;
        TransactionDate = transactionDate.Kind == DateTimeKind.Utc 
            ? transactionDate 
            : DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc);
        Note = note?.Trim();
        IsEssential = isEssential;
        IsWindfall = isWindfall;
        IsRecurring = isRecurring;
        ExcludeFromReport = excludeFromReport;
        UpdatedAt = DateTime.UtcNow;
    }
}
