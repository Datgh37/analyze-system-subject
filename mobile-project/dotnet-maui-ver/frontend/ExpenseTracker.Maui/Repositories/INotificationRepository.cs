using ExpenseTracker.Maui.Models.Notifications;

namespace ExpenseTracker.Maui.Repositories;

public interface INotificationRepository
{
    Task<List<InAppNotificationModel>> GetNotificationsAsync(bool? unreadOnly = null, int take = 50);
    Task<bool> MarkAsReadAsync(Guid notificationId);
    Task<bool> MarkAllAsReadAsync();
    Task<bool> RegisterPushTokenAsync(string token, string platform);
    Task<InAppNotificationModel?> SendTestNotificationAsync(string title, string message, string type, string? route = null, string? id = null);
}
