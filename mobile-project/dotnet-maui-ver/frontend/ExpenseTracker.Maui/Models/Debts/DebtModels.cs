namespace ExpenseTracker.Maui.Models.Debts;

public enum DebtType
{
    Personal = 0,            // Vay mượn cá nhân (người thân, bạn bè)
    CreditCard = 1,          // Thẻ tín dụng ngân hàng
    InstallmentLoan = 2,     // Vay tiêu dùng, mua hàng trả góp
    MortgageOrBankLoan = 3   // Vay ngân hàng, thế chấp
}

public enum DebtStatus
{
    Active = 0,   // Đang trả nợ
    PaidOff = 1,  // Đã tất toán sạch nợ
    Overdue = 2   // Quá hạn
}

public class DebtModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DebtType DebtType { get; set; }
    public string DebtTypeName { get; set; } = string.Empty;
    public string CreditorName { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ProgressPercent { get; set; }
    public decimal InterestRateAnnual { get; set; }
    public decimal MinimumPayment { get; set; }
    public int DueDateMonthly { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ExpectedPayoffDate { get; set; }
    public DebtStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Helpers cho giao diện hiển thị
    public string FormattedCurrentBalance => $"{CurrentBalance:N0} đ";
    public string FormattedPrincipal => $"{PrincipalAmount:N0} đ";
    public string FormattedPaidAmount => $"{PaidAmount:N0} đ";
    public string FormattedMinimumPayment => $"{MinimumPayment:N0} đ / tháng";
    public string FormattedInterestRate => $"{InterestRateAnnual:F1}% / năm";
    public string FormattedProgress => $"{ProgressPercent:F1}%";
    public string DueDateText => $"Hạn ngày {DueDateMonthly} hàng tháng";

    public string StatusBadgeColorHex => Status switch
    {
        DebtStatus.PaidOff => "#10B981", // Xanh lá
        DebtStatus.Overdue => "#EF4444", // Đỏ
        _ => "#3B82F6"                   // Xanh dương
    };

    public string TypeIcon => DebtType switch
    {
        DebtType.Personal => "user_friends.png",
        DebtType.CreditCard => "credit_card.png",
        DebtType.InstallmentLoan => "shopping_bag.png",
        DebtType.MortgageOrBankLoan => "account_balance.png",
        _ => "money.png"
    };
}

public class DebtTransactionModel
{
    public Guid Id { get; set; }
    public Guid DebtId { get; set; }
    public Guid WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PrincipalPaid { get; set; }
    public decimal InterestPaid { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public string FormattedAmount => $"-{Amount:N0} đ";
    public string FormattedPrincipal => $"Gốc: {PrincipalPaid:N0} đ";
    public string FormattedInterest => $"Lãi: {InterestPaid:N0} đ";
    public string FormattedDate => PaymentDate.ToString("dd/MM/yyyy HH:mm");
}

public class DebtDetailModel
{
    public DebtModel Debt { get; set; } = new();
    public List<DebtTransactionModel> Transactions { get; set; } = new();
}

public class DebtSummaryModel
{
    public decimal TotalCurrentDebt { get; set; }
    public decimal TotalInitialPrincipal { get; set; }
    public decimal TotalPaidPrincipal { get; set; }
    public decimal TotalMonthlyMinimumPayment { get; set; }
    public int ActiveDebtsCount { get; set; }
    public int PaidOffDebtsCount { get; set; }

    public string FormattedTotalDebt => $"{TotalCurrentDebt:N0} đ";
    public string FormattedTotalPrincipal => $"{TotalInitialPrincipal:N0} đ";
    public string FormattedTotalPaid => $"{TotalPaidPrincipal:N0} đ";
    public string FormattedTotalMonthlyMin => $"{TotalMonthlyMinimumPayment:N0} đ";
}

public class CreateDebtRequest
{
    public string Name { get; set; } = string.Empty;
    public int DebtType { get; set; }
    public string CreditorName { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRateAnnual { get; set; }
    public decimal MinimumPayment { get; set; }
    public int DueDateMonthly { get; set; } = 15;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime? ExpectedPayoffDate { get; set; }
    public decimal? CurrentBalance { get; set; }
    public string? Notes { get; set; }
}

public class UpdateDebtRequest
{
    public string Name { get; set; } = string.Empty;
    public int DebtType { get; set; }
    public string CreditorName { get; set; } = string.Empty;
    public decimal InterestRateAnnual { get; set; }
    public decimal MinimumPayment { get; set; }
    public int DueDateMonthly { get; set; } = 15;
    public DateTime? ExpectedPayoffDate { get; set; }
    public string? Notes { get; set; }
}

public class RecordDebtPaymentRequest
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public decimal PrincipalPaid { get; set; }
    public decimal InterestPaid { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public string? Note { get; set; }
}

