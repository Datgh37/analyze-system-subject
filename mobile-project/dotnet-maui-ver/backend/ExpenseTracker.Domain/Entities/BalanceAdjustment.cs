using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Balance adjustment entity recording immutable balance reconciliation audits
public class BalanceAdjustment : BaseEntity
{
    public Guid WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = null!;

    public decimal OldBalance { get; private set; }
    public decimal NewBalance { get; private set; }
    public decimal DifferenceAmount { get; private set; }
    public string? Reason { get; private set; }
    public DateTime AdjustmentDate { get; private set; }
    public string? ProofImageUrl { get; private set; }

    // Parameterless constructor for EF Core materialization
    private BalanceAdjustment() { }

    public BalanceAdjustment(
        Guid walletId,
        decimal oldBalance,
        decimal newBalance,
        string? reason = null,
        DateTime? adjustmentDate = null,
        string? proofImageUrl = null)
    {
        WalletId = walletId;
        OldBalance = oldBalance;
        NewBalance = newBalance;
        DifferenceAmount = newBalance - oldBalance;
        Reason = reason?.Trim();
        AdjustmentDate = adjustmentDate ?? DateTime.UtcNow;
        ProofImageUrl = proofImageUrl?.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
