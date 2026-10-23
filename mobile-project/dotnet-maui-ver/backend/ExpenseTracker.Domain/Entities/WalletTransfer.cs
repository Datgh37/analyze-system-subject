using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Internal transfer entity recording atomic money movement between two user wallets
public class WalletTransfer : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid SourceWalletId { get; private set; }
    public Wallet SourceWallet { get; private set; } = null!;

    public Guid DestinationWalletId { get; private set; }
    public Wallet DestinationWallet { get; private set; } = null!;

    public decimal Amount { get; private set; }
    public DateTime TransferDate { get; private set; }
    public string? Note { get; private set; }

    // Parameterless constructor for EF Core materialization
    private WalletTransfer() { }

    public WalletTransfer(
        Guid userId,
        Guid sourceWalletId,
        Guid destinationWalletId,
        decimal amount,
        DateTime transferDate,
        string? note = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền chuyển phải lớn hơn 0.", nameof(amount));

        if (sourceWalletId == destinationWalletId)
            throw new ArgumentException("Ví nguồn và ví đích không được trùng nhau.", nameof(destinationWalletId));

        UserId = userId;
        SourceWalletId = sourceWalletId;
        DestinationWalletId = destinationWalletId;
        Amount = amount;
        TransferDate = DateTime.SpecifyKind(transferDate, DateTimeKind.Utc);
        Note = note?.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
