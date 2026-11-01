namespace ExpenseTracker.Domain.Enums;

// Loại hình khoản nợ cá nhân (UC-06.05)
public enum DebtType
{
    Personal = 0,            // Vay mượn cá nhân (người thân, bạn bè)
    CreditCard = 1,          // Thẻ tín dụng ngân hàng
    InstallmentLoan = 2,     // Vay tiêu dùng, mua hàng trả góp
    MortgageOrBankLoan = 3   // Vay ngân hàng, thế chấp
}

// Trạng thái của khoản nợ cá nhân
public enum DebtStatus
{
    Active = 0,   // Đang hoạt động, còn dư nợ
    PaidOff = 1,  // Đã tất toán hoàn toàn sạch nợ
    Overdue = 2   // Quá hạn thanh toán
}

