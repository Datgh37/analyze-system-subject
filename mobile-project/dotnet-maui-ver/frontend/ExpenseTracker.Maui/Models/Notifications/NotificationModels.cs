namespace ExpenseTracker.Maui.Models.Notifications;

public class InAppNotificationModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "System"; // BudgetAlert, BalanceUpdate, AiReceiptProcessed, DailyReminder, System
    public string? PayloadJson { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Visual formatting properties for UI presentation
    public string TypeIcon => Type switch
    {
        "BudgetAlert" => "⚠️",
        "BalanceUpdate" => "💰",
        "AiReceiptProcessed" => "🧾",
        "DailyReminder" => "⏰",
        _ => "🔔"
    };

    public string TypeColorHex => Type switch
    {
        "BudgetAlert" => "#EF4444",      // Đỏ cảnh báo
        "BalanceUpdate" => "#10B981",     // Xanh lá biến động số dư
        "AiReceiptProcessed" => "#0066FF",// Xanh dương OCR
        "DailyReminder" => "#F59E0B",     // Vàng nhắc nhở
        _ => "#8B5CF6"                    // Tím hệ thống
    };

    public string FormattedCreatedAt => CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
}

public record RegisterPushTokenRequestModel(
    string DeviceToken,
    string Platform,
    string? DeviceId);

public record SendTestNotificationRequestModel(
    string Title,
    string Message,
    string Type,
    string? TargetRoute,
    string? TargetId);
