namespace ExpenseTracker.Domain.Enums;

// Classifies personal income into economic source groups
public enum IncomeCategoryGroup
{
    Active = 1,   // Thu nhập chủ động (Lương, Thưởng, Công tác phí, Làm thêm)
    Passive = 2,  // Thu nhập thụ động (Lãi tiết kiệm, Cổ tức, Cho thuê tài sản)
    Other = 3     // Thu nhập khác (Quà tặng, Trúng thưởng, Hoàn tiền)
}
