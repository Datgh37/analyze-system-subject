#if ANDROID
using Android.App;
using Android.Content;

namespace ExpenseTracker.Maui.Platforms.Android.Services;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted })]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null) return;

        if (intent.Action == Intent.ActionBootCompleted)
        {
            bool isEnabled = Preferences.Default.Get("daily_expense_reminder_enabled", true);
            if (isEnabled)
            {
                // Khởi động lại lịch hẹn báo thức sau khi máy khởi động lại
                DailyReminderReceiver.ScheduleNextDayReminder(context);
            }
        }
    }
}
#endif
