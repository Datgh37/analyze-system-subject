using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Maui.Services.Notifications;

public class DesktopFallbackNotificationService : INativeNotificationService
{
    private readonly ILogger<DesktopFallbackNotificationService> _logger;

    public DesktopFallbackNotificationService(ILogger<DesktopFallbackNotificationService> logger)
    {
        _logger = logger;
    }

    public Task InitializeChannelsAsync()
    {
        _logger.LogInformation("[DesktopFallback] Notification channels initialized (mocked for desktop/tests)");
        return Task.CompletedTask;
    }

    public async Task ShowNotificationAsync(string title, string message, string channelId, int notificationId = 0, string? targetRoute = null)
    {
        _logger.LogInformation("[DesktopFallback] Notification: [{Channel}] {Title}: {Message}", channelId, title, message);

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Shell.Current != null)
            {
                if (!string.IsNullOrEmpty(targetRoute))
                {
                    bool view = await Shell.Current.DisplayAlertAsync(title, message, "Xem ngay", "Đóng");
                    if (view)
                    {
                        await Shell.Current.GoToAsync(targetRoute);
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync(title, message, "Đóng");
                }
            }
        });
    }

    public Task ScheduleDailyReminderAsync(int targetHour = 21, int targetMinute = 0)
    {
        Preferences.Default.Set("daily_expense_reminder_enabled", true);
        Preferences.Default.Set("daily_expense_reminder_hour", targetHour);
        Preferences.Default.Set("daily_expense_reminder_minute", targetMinute);
        _logger.LogInformation("[DesktopFallback] Daily reminder scheduled at {Hour:D2}:{Minute:D2}", targetHour, targetMinute);
        return Task.CompletedTask;
    }

    public Task CancelDailyReminderAsync()
    {
        Preferences.Default.Set("daily_expense_reminder_enabled", false);
        _logger.LogInformation("[DesktopFallback] Daily reminder cancelled");
        return Task.CompletedTask;
    }

    public Task<bool> RequestPermissionAsync()
    {
        return Task.FromResult(true);
    }
}
