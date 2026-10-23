using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Record of a deposit or withdrawal contribution to a savings goal (UC-06.02, UC-06.03)
public class SavingsGoalContribution
{
    public Guid Id { get; private set; }
    public Guid SavingsGoalId { get; private set; }
    public Guid WalletId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public decimal Amount { get; private set; }
    public ContributionType Type { get; private set; } = ContributionType.Deposit;
    public DateTime ContributionDate { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public SavingsGoal? SavingsGoal { get; private set; }
    public Wallet? Wallet { get; private set; }
    public Transaction? Transaction { get; private set; }

    private SavingsGoalContribution() { } // EF Core constructor

    public SavingsGoalContribution(
        Guid savingsGoalId,
        Guid walletId,
        decimal amount,
        ContributionType type,
        DateTime contributionDate,
        string? note = null,
        Guid? transactionId = null)
    {
        if (amount == 0)
            throw new ArgumentException("Số tiền giao dịch quỹ tích lũy không được bằng 0 đ.", nameof(amount));

        Id = Guid.NewGuid();
        SavingsGoalId = savingsGoalId;
        WalletId = walletId;
        Amount = amount;
        Type = type;
        ContributionDate = contributionDate;
        Note = note?.Trim();
        TransactionId = transactionId;
        CreatedAt = DateTime.UtcNow;
    }

    public void LinkTransaction(Guid transactionId)
    {
        TransactionId = transactionId;
    }
}
