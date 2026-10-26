namespace ExpenseTracker.Maui.Services.Notifications;

public interface INativeNotificationService
{
    Task InitializeChannelsAsync();
    Task ShowNotificationAsync(string title, string message, string channelId, int notificationId = 0, string? targetRoute = null);
    Task ScheduleDailyReminderAsync(int targetHour = 21, int targetMinute = 0);
    Task CancelDailyReminderAsync();
    Task<bool> RequestPermissionAsync();
}

public static class NotificationChannelIds
{
    public const string DsaAlerts = "channel_dsa_alerts";
    public const string BalanceFluctuation = "channel_balance_fluctuation";
    public const string DailyReminder = "channel_daily_reminder";
}
