using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Aggregate Root for Friction-by-Design Cooling-Off Period (UC-09.05)
public class CoolingOffTransaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid WalletId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime CoolingOffUntil { get; private set; }
    public CoolingOffStatus Status { get; private set; }
    public decimal EquivalentWorkHours { get; private set; }
    public decimal ProjectedDsaReduction { get; private set; }
    public string? BypassReason { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    // Navigation properties
    public User? User { get; private set; }
    public Wallet? Wallet { get; private set; }
    public Category? Category { get; private set; }

    private CoolingOffTransaction() { } // EF Core constructor

    public CoolingOffTransaction(
        Guid userId,
        Guid walletId,
        Guid categoryId,
        decimal amount,
        string description,
        DateTime coolingOffUntil,
        decimal equivalentWorkHours,
        decimal projectedDsaReduction)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (walletId == Guid.Empty)
            throw new ArgumentException("WalletId cannot be empty.", nameof(walletId));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        if (coolingOffUntil <= DateTime.UtcNow)
            throw new ArgumentException("CoolingOffUntil must be in the future.", nameof(coolingOffUntil));

        Id = Guid.NewGuid();
        UserId = userId;
        WalletId = walletId;
        CategoryId = categoryId;
        Amount = amount;
        Description = description?.Trim() ?? string.Empty;
        CoolingOffUntil = DateTime.SpecifyKind(coolingOffUntil, DateTimeKind.Utc);
        Status = CoolingOffStatus.PendingCoolingOff;
        EquivalentWorkHours = Math.Max(0m, equivalentWorkHours);
        ProjectedDsaReduction = Math.Max(0m, projectedDsaReduction);
        CreatedAt = DateTime.UtcNow;
    }

    // UC-09.05: Confirm purchase after the cooling-off duration has elapsed
    public void ConfirmPurchase()
    {
        if (Status != CoolingOffStatus.PendingCoolingOff)
            throw new InvalidOperationException($"Cannot confirm transaction in '{Status}' status.");

        if (DateTime.UtcNow < CoolingOffUntil)
            throw new InvalidOperationException("Cannot confirm purchase before the cooling-off period has expired without emergency bypass.");

        Status = CoolingOffStatus.Confirmed;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // UC-09.05: Cancel purchase to defeat impulse buying and save money
    public void CancelPurchase()
    {
        if (Status != CoolingOffStatus.PendingCoolingOff)
            throw new InvalidOperationException($"Cannot cancel transaction in '{Status}' status.");

        Status = CoolingOffStatus.Cancelled;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // UC-09.05: Emergency bypass with mandatory reason
    public void BypassWithReason(string reason)
    {
        if (Status != CoolingOffStatus.PendingCoolingOff)
            throw new InvalidOperationException($"Cannot bypass transaction in '{Status}' status.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Emergency bypass requires a valid reason.", nameof(reason));

        Status = CoolingOffStatus.Bypassed;
        BypassReason = reason.Trim();
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
