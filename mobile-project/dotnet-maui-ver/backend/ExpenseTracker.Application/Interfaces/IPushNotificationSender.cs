using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface IPushNotificationSender
{
    string ProviderName { get; }

    Task<PushDeliveryResult> SendToUserAsync(
        Guid userId,
        string title,
        string message,
        string type,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default);

    Task<PushDeliveryResult> SendToTokenAsync(
        string deviceToken,
        string title,
        string message,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default);
}
