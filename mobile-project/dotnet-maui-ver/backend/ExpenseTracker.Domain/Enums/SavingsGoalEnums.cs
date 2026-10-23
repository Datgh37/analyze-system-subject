namespace ExpenseTracker.Domain.Enums;

// Type of personal savings goal (UC-06.01)
public enum SavingsGoalType
{
    EmergencyFund = 1,        // Quỹ dự phòng khẩn cấp (3-6 tháng chi phí thiết yếu)
    MajorPurchase = 2,        // Mua sắm tài sản lớn (Xe, nhà, thiết bị)
    Investment = 3,           // Quỹ đầu tư / Tự do tài chính
    Education = 4,            // Quỹ học tập & Phát triển bản thân
    Travel = 5,               // Quỹ du lịch & Trải nghiệm
    Other = 6                 // Mục tiêu khác
}

// Lifecycle status of a savings goal
public enum SavingsGoalStatus
{
    Active = 1,
    Completed = 2,
    Paused = 3,
    Cancelled = 4
}

// Priority level of the financial goal
public enum SavingsGoalPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

// Direction or type of savings contribution transaction (UC-06.02, UC-06.03)
public enum ContributionType
{
    Deposit = 1,              // Nạp tiền từ ví vào quỹ tích lũy
    Withdrawal = 2,           // Rút tiền từ quỹ tích lũy về ví
    InterestOrBonus = 3       // Tiền lãi ngân hàng hoặc khoản thưởng cộng dồn vào quỹ
}
