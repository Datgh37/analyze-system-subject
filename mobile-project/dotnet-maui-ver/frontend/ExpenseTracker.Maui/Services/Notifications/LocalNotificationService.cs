using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Maui.Services.Notifications;

public class LocalNotificationService : ILocalNotificationService, IDisposable
{
    private readonly ILogger<LocalNotificationService> _logger;
    private Timer? _dailyReminderTimer;
    private const string ReminderEnabledKey = "daily_expense_reminder_enabled";
    private const string ReminderHourKey = "daily_expense_reminder_hour";
    private const string ReminderMinuteKey = "daily_expense_reminder_minute";

    public LocalNotificationService(ILogger<LocalNotificationService> logger)
    {
        _logger = logger;

        // Restore scheduled daily reminder if previously active
        var isEnabled = Preferences.Default.Get(ReminderEnabledKey, true);
        if (isEnabled)
        {
            var hour = Preferences.Default.Get(ReminderHourKey, 21);
            var minute = Preferences.Default.Get(ReminderMinuteKey, 0);
            _ = ScheduleDailyExpenseReminderAsync(hour, minute);
        }
    }

    public Task ScheduleDailyExpenseReminderAsync(int targetHour = 21, int targetMinute = 0)
    {
        try
        {
            _dailyReminderTimer?.Dispose();

            Preferences.Default.Set(ReminderEnabledKey, true);
            Preferences.Default.Set(ReminderHourKey, targetHour);
            Preferences.Default.Set(ReminderMinuteKey, targetMinute);

            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, targetHour, targetMinute, 0);

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var initialDelay = nextRun - now;
            var interval = TimeSpan.FromHours(24);

            _dailyReminderTimer = new Timer(async _ =>
            {
                await OnDailyReminderFiredAsync();
            }, null, initialDelay, interval);

            _logger.LogInformation("Daily expense reminder scheduled for {TargetTime} (next in {Minutes} mins)",
                nextRun.ToString("HH:mm dd/MM/yyyy"), (int)initialDelay.TotalMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while scheduling daily expense reminder");
        }

        return Task.CompletedTask;
    }

    public Task CancelDailyExpenseReminderAsync()
    {
        _dailyReminderTimer?.Dispose();
        _dailyReminderTimer = null;
        Preferences.Default.Set(ReminderEnabledKey, false);
        _logger.LogInformation("Daily expense reminder cancelled");
        return Task.CompletedTask;
    }

    public async Task ShowInAppAlertAsync(string title, string message, string type = "System", string? targetRoute = null)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Shell.Current != null)
            {
                if (!string.IsNullOrEmpty(targetRoute))
                {
                    var viewDetails = await Shell.Current.DisplayAlertAsync(title, message, "Xem ngay", "Đóng");
                    if (viewDetails)
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

    private async Task OnDailyReminderFiredAsync()
    {
        _logger.LogInformation("Daily expense reminder triggered at 21:00");

        await ShowInAppAlertAsync(
            "⏰ Nhắc nhở tài chính cuối ngày",
            "Đã 21:00 rồi! Đừng quên ghi lại các khoản chi tiêu hôm nay để duy trì hạn mức chi tiêu hàng ngày (DSA) và bảo vệ túi tiền của bạn nhé!",
            "DailyReminder",
            "///ExpensesPage");
    }

    public void Dispose()
    {
        _dailyReminderTimer?.Dispose();
    }
}
