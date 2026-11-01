using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Thực thể Quản lý Khoản nợ Cá nhân (UC-06.05 & UC-06.07)
public class Debt : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;
    public DebtType DebtType { get; private set; }
    public string CreditorName { get; private set; } = string.Empty;

    public decimal PrincipalAmount { get; private set; }
    public decimal CurrentBalance { get; private set; }
    public decimal InterestRateAnnual { get; private set; } // % / năm (VD: 10.5)
    public decimal MinimumPayment { get; private set; }     // Khoản trả tối thiểu / tháng
    public int DueDateMonthly { get; private set; }         // Ngày đến hạn hàng tháng (1-31)

    public DateTime StartDate { get; private set; }
    public DateTime? ExpectedPayoffDate { get; private set; }
    public DebtStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public ICollection<DebtTransaction> Transactions { get; private set; } = new List<DebtTransaction>();

    // Parameterless constructor for EF Core materialization
    private Debt() { }

    public Debt(
        Guid userId,
        string name,
        DebtType debtType,
        string creditorName,
        decimal principalAmount,
        decimal interestRateAnnual,
        decimal minimumPayment,
        int dueDateMonthly,
        DateTime startDate,
        DateTime? expectedPayoffDate = null,
        decimal? currentBalance = null,
        string? notes = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Mã người dùng không hợp lệ.", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên khoản nợ không được để trống.", nameof(name));

        if (string.IsNullOrWhiteSpace(creditorName))
            throw new ArgumentException("Tên bên cho vay / chủ nợ không được để trống.", nameof(creditorName));

        if (principalAmount <= 0)
            throw new ArgumentException("Số tiền gốc ban đầu phải lớn hơn 0.", nameof(principalAmount));

        if (interestRateAnnual < 0)
            throw new ArgumentException("Lãi suất năm không được âm.", nameof(interestRateAnnual));

        if (minimumPayment < 0)
            throw new ArgumentException("Khoản trả tối thiểu không được âm.", nameof(minimumPayment));

        if (dueDateMonthly < 1 || dueDateMonthly > 31)
            throw new ArgumentException("Ngày đến hạn hàng tháng phải từ 1 đến 31.", nameof(dueDateMonthly));

        var initialBalance = currentBalance ?? principalAmount;
        if (initialBalance < 0)
            throw new ArgumentException("Dư nợ hiện tại không được âm.", nameof(currentBalance));

        UserId = userId;
        Name = name.Trim();
        DebtType = debtType;
        CreditorName = creditorName.Trim();
        PrincipalAmount = principalAmount;
        CurrentBalance = initialBalance;
        InterestRateAnnual = interestRateAnnual;
        MinimumPayment = minimumPayment;
        DueDateMonthly = dueDateMonthly;
        StartDate = startDate.Kind == DateTimeKind.Utc ? startDate : DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        ExpectedPayoffDate = expectedPayoffDate.HasValue 
            ? (expectedPayoffDate.Value.Kind == DateTimeKind.Utc ? expectedPayoffDate.Value : DateTime.SpecifyKind(expectedPayoffDate.Value, DateTimeKind.Utc))
            : null;
        Status = initialBalance == 0 ? DebtStatus.PaidOff : DebtStatus.Active;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public DebtTransaction RecordPayment(
        Guid walletId,
        decimal amount,
        decimal principalPaid,
        decimal interestPaid,
        DateTime paymentDate,
        string? note = null)
    {
        if (Status == DebtStatus.PaidOff)
            throw new InvalidOperationException("Khoản nợ này đã được tất toán hoàn toàn.");

        if (principalPaid > CurrentBalance)
            throw new InvalidOperationException($"Số tiền gốc trả ({principalPaid:N0}đ) không được vượt quá dư nợ hiện tại ({CurrentBalance:N0}đ).");

        var transaction = new DebtTransaction(
            Id,
            walletId,
            amount,
            principalPaid,
            interestPaid,
            paymentDate,
            note);

        CurrentBalance -= principalPaid;

        if (CurrentBalance <= 0)
        {
            CurrentBalance = 0;
            Status = DebtStatus.PaidOff;
        }

        UpdatedAt = DateTime.UtcNow;
        return transaction;
    }

    public void UpdateDetails(
        string name,
        DebtType debtType,
        string creditorName,
        decimal interestRateAnnual,
        decimal minimumPayment,
        int dueDateMonthly,
        DateTime? expectedPayoffDate,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên khoản nợ không được để trống.", nameof(name));

        if (string.IsNullOrWhiteSpace(creditorName))
            throw new ArgumentException("Tên bên cho vay / chủ nợ không được để trống.", nameof(creditorName));

        if (interestRateAnnual < 0)
            throw new ArgumentException("Lãi suất năm không được âm.", nameof(interestRateAnnual));

        if (minimumPayment < 0)
            throw new ArgumentException("Khoản trả tối thiểu không được âm.", nameof(minimumPayment));

        if (dueDateMonthly < 1 || dueDateMonthly > 31)
            throw new ArgumentException("Ngày đến hạn hàng tháng phải từ 1 đến 31.", nameof(dueDateMonthly));

        Name = name.Trim();
        DebtType = debtType;
        CreditorName = creditorName.Trim();
        InterestRateAnnual = interestRateAnnual;
        MinimumPayment = minimumPayment;
        DueDateMonthly = dueDateMonthly;
        ExpectedPayoffDate = expectedPayoffDate.HasValue
            ? (expectedPayoffDate.Value.Kind == DateTimeKind.Utc ? expectedPayoffDate.Value : DateTime.SpecifyKind(expectedPayoffDate.Value, DateTimeKind.Utc))
            : null;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkOverdue()
    {
        if (Status == DebtStatus.Active)
        {
            Status = DebtStatus.Overdue;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Reopen(decimal newBalance)
    {
        if (newBalance <= 0)
            throw new ArgumentException("Dư nợ mở lại phải lớn hơn 0.", nameof(newBalance));

        CurrentBalance = newBalance;
        Status = DebtStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }
}
