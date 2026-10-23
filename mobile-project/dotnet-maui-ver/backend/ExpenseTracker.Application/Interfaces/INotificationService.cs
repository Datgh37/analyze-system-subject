using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface INotificationService
{
    Task RegisterPushTokenAsync(Guid userId, RegisterPushTokenRequest request, CancellationToken cancellationToken = default);
    Task<List<InAppNotificationDto>> GetNotificationsAsync(Guid userId, bool? unreadOnly = null, int take = 50, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<InAppNotificationDto> CreateAndSendNotificationAsync(Guid userId, string title, string message, string type, object? payload = null, CancellationToken cancellationToken = default);
}
