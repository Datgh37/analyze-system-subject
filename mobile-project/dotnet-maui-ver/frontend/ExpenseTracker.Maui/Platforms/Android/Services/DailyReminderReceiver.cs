#if ANDROID
#pragma warning disable CA1416, CS8602, CS8604

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using ExpenseTracker.Maui.Services.Notifications;

namespace ExpenseTracker.Maui.Platforms.Android.Services;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { "com.expensetracker.action.DAILY_REMINDER" })]
public class DailyReminderReceiver : BroadcastReceiver
{
    public const int DailyReminderNotificationId = 9999;
    public const int DailyReminderPendingIntentId = 1002;
    private const PendingIntentFlags SafeImmutableFlags = PendingIntentFlags.UpdateCurrent | (PendingIntentFlags)0x04000000;

    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null) return;

        // 1. Kiểm tra xem người dùng có đang bật tính năng nhắc nhở không
        bool isEnabled = Preferences.Default.Get("daily_expense_reminder_enabled", true);
        if (!isEnabled) return;

        // 2. Xây dựng Intent mở thẳng màn hình Ghi Chép Chi Tiêu khi người dùng bấm vào Notification
        var launchIntent = new Intent(context, typeof(MainActivity));
        launchIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);
        launchIntent.PutExtra("target_route", "///ExpensesPage");

        var pendingIntent = PendingIntent.GetActivity(
            context,
            1003,
            launchIntent,
            SafeImmutableFlags);

        int smallIcon = context.ApplicationInfo?.Icon != 0
            ? (context.ApplicationInfo?.Icon ?? global::Android.Resource.Drawable.IcDialogInfo)
            : global::Android.Resource.Drawable.IcDialogInfo;

        const string title = "⏰ Nhắc Nhở Tài Chính 21:00";
        const string message = "Đã 21:00 rồi! Đừng quên ghi lại các khoản chi hôm nay để bảo vệ hạn mức an toàn DSA nhé 💎";

        var builder = new NotificationCompat.Builder(context, NotificationChannelIds.DailyReminder)
            .SetSmallIcon(smallIcon)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetStyle(new NotificationCompat.BigTextStyle().BigText(message))
            .SetAutoCancel(true)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetDefaults((int)(NotificationDefaults.Sound | NotificationDefaults.Vibrate))
            .SetContentIntent(pendingIntent);

        var notificationManager = NotificationManagerCompat.From(context);
        var notification = builder.Build();
        if (notification != null)
        {
            notificationManager.Notify(DailyReminderNotificationId, notification);
        }

        // 3. Tự động lên lịch cho 21:00 của ngày tiếp theo để chu kỳ tiếp tục mãi mãi
        ScheduleNextDayReminder(context);
    }

    public static void ScheduleNextDayReminder(Context context)
    {
        var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
        if (alarmManager == null) return;

        int hour = Preferences.Default.Get("daily_expense_reminder_hour", 21);
        int minute = Preferences.Default.Get("daily_expense_reminder_minute", 0);

        var now = DateTime.Now;
        var nextRun = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0).AddDays(1);
        var epochDiff = nextRun.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long triggerAtMillis = (long)epochDiff.TotalMilliseconds;

        var reminderIntent = new Intent(context, typeof(DailyReminderReceiver));
        var pendingIntent = PendingIntent.GetBroadcast(
            context,
            DailyReminderPendingIntentId,
            reminderIntent,
            SafeImmutableFlags);

        if (pendingIntent != null)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
            else
            {
                alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
        }
    }
}
#endif
