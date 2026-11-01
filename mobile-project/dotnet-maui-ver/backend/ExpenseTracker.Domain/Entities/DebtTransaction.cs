using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Giao dịch thanh toán trả nợ cá nhân (UC-06.07)
public class DebtTransaction : BaseEntity
{
    public Guid DebtId { get; private set; }
    public Debt Debt { get; private set; } = null!;

    public Guid WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = null!;

    public decimal Amount { get; private set; }
    public decimal PrincipalPaid { get; private set; }
    public decimal InterestPaid { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? Note { get; private set; }

    // Parameterless constructor for EF Core materialization
    private DebtTransaction() { }

    public DebtTransaction(
        Guid debtId,
        Guid walletId,
        decimal amount,
        decimal principalPaid,
        decimal interestPaid,
        DateTime paymentDate,
        string? note = null)
    {
        if (debtId == Guid.Empty)
            throw new ArgumentException("Mã khoản nợ không hợp lệ.", nameof(debtId));

        if (walletId == Guid.Empty)
            throw new ArgumentException("Mã ví thanh toán không hợp lệ.", nameof(walletId));

        if (amount <= 0)
            throw new ArgumentException("Số tiền thanh toán phải lớn hơn 0.", nameof(amount));

        if (principalPaid < 0)
            throw new ArgumentException("Tiền gốc trả không được âm.", nameof(principalPaid));

        if (interestPaid < 0)
            throw new ArgumentException("Tiền lãi trả không được âm.", nameof(interestPaid));

        if (principalPaid + interestPaid != amount)
            throw new ArgumentException("Tổng tiền gốc và tiền lãi phải bằng tổng số tiền thanh toán.", nameof(amount));

        DebtId = debtId;
        WalletId = walletId;
        Amount = amount;
        PrincipalPaid = principalPaid;
        InterestPaid = interestPaid;
        PaymentDate = paymentDate.Kind == DateTimeKind.Utc 
            ? paymentDate 
            : DateTime.SpecifyKind(paymentDate, DateTimeKind.Utc);
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}

