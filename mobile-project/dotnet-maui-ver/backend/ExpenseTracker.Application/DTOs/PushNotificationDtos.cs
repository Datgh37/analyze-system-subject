namespace ExpenseTracker.Application.DTOs;

public record PushDeliveryResult(
    bool Success,
    string Provider,
    int DeliveredCount,
    string? MessageId = null,
    string? ErrorReason = null)
{
    public static PushDeliveryResult Successful(string provider, int count = 1, string? messageId = null)
        => new(true, provider, count, messageId);

    public static PushDeliveryResult Failed(string provider, string reason)
        => new(false, provider, 0, null, reason);
}
