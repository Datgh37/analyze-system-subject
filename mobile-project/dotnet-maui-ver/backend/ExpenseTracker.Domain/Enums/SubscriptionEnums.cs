namespace ExpenseTracker.Domain.Enums;

// Chu kỳ thanh toán thuê bao định kỳ (UC-03.09)
public enum SubscriptionBillingCycle
{
    Weekly = 1,     // Hàng tuần
    Monthly = 2,    // Hàng tháng (phổ biến nhất: Netflix, Spotify, iCloud)
    Quarterly = 3,  // Hàng quý (3 tháng/lần)
    Yearly = 4      // Hàng năm (Adobe, Google One, Canva Pro)
}

// Trạng thái vòng đời của một thuê bao dịch vụ
public enum SubscriptionStatus
{
    Active = 1,     // Đang hoạt động và tự động gia hạn
    Cancelled = 2,  // Đã hủy theo dõi / Hủy gói dịch vụ
    Paused = 3,     // Tạm ngưng dịch vụ
    FreeTrial = 4   // Đang trong thời gian dùng thử miễn phí (cần cảnh báo trước khi trừ tiền)
}

