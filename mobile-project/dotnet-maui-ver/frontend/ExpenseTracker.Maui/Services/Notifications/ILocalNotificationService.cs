namespace ExpenseTracker.Maui.Services.Notifications;

public interface ILocalNotificationService
{
    Task ScheduleDailyExpenseReminderAsync(int targetHour = 21, int targetMinute = 0);
    Task CancelDailyExpenseReminderAsync();
    Task ShowInAppAlertAsync(string title, string message, string type = "System", string? targetRoute = null);
}
