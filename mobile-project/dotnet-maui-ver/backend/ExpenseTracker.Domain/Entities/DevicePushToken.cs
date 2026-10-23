using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class DevicePushToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string DeviceToken { get; private set; } = string.Empty;
    public string Platform { get; private set; } = "Android"; // Android, iOS, Windows
    public string? DeviceId { get; private set; }
    public DateTime LastActiveAt { get; private set; }

    // Navigation property
    public User? User { get; private set; }

    private DevicePushToken() { } // Required by EF Core

    public DevicePushToken(
        Guid userId,
        string deviceToken,
        string platform = "Android",
        string? deviceId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        DeviceToken = deviceToken;
        Platform = platform;
        DeviceId = deviceId;
        LastActiveAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateToken(string newDeviceToken)
    {
        DeviceToken = newDeviceToken;
        LastActiveAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordActivity()
    {
        LastActiveAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
