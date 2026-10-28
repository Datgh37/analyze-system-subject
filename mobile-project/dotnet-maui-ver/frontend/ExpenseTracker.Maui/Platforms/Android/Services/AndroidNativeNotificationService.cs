#if ANDROID
#pragma warning disable CA1416, CS8602, CS8604

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using ExpenseTracker.Maui.Services.Notifications;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Maui.Platforms.Android.Services;

public class AndroidNativeNotificationService : INativeNotificationService
{
    private readonly ILogger<AndroidNativeNotificationService> _logger;
    private const int DailyReminderPendingIntentId = 1002;
    private const PendingIntentFlags SafeImmutableFlags = PendingIntentFlags.UpdateCurrent | (PendingIntentFlags)0x04000000; // FLAG_IMMUTABLE

    public AndroidNativeNotificationService(ILogger<AndroidNativeNotificationService> logger)
    {
        _logger = logger;
        _ = InitializeChannelsAsync();
    }

    public Task InitializeChannelsAsync()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var context = global::Android.App.Application.Context;
            var notificationManager = (NotificationManager?)context.GetSystemService(Context.NotificationService);

            if (notificationManager != null)
            {
                // Channel 1: Cảnh báo hạn mức DSA (Khẩn cấp, Rung, Âm thanh)
                var dsaChannel = new NotificationChannel(
                    NotificationChannelIds.DsaAlerts,
                    "🚨 Cảnh Báo Hạn Mức DSA",
                    NotificationImportance.High)
                {
                    Description = "Cảnh báo tức thì khi chi tiêu tiếp cận hoặc vượt ngưỡng an toàn DSA"
                };
                dsaChannel.EnableVibration(true);
                dsaChannel.SetVibrationPattern(new long[] { 0, 250, 150, 250 });
                notificationManager.CreateNotificationChannel(dsaChannel);

                // Channel 2: Biến động số dư & Giao dịch
                var balanceChannel = new NotificationChannel(
                    NotificationChannelIds.BalanceFluctuation,
                    "💳 Biến Động Số Dư & Giao Dịch",
                    NotificationImportance.High)
                {
                    Description = "Thông báo ghi nhận thu chi và cập nhật biến động số dư các tài khoản ví"
                };
                balanceChannel.EnableVibration(true);
                notificationManager.CreateNotificationChannel(balanceChannel);

                // Channel 3: Nhắc nhở tài chính 21:00
                var reminderChannel = new NotificationChannel(
                    NotificationChannelIds.DailyReminder,
                    "⏰ Nhắc Nhở Tài Chính 21:00",
                    NotificationImportance.High)
                {
                    Description = "Nhắc nhở kiểm tra và ghi lại chi tiêu cuối ngày để bảo vệ kỷ luật ngân sách"
                };
                reminderChannel.EnableVibration(true);
                notificationManager.CreateNotificationChannel(reminderChannel);

                _logger.LogInformation("[AndroidNativeNotificationService] 3 Notification Channels registered successfully");
            }
        }

        return Task.CompletedTask;
    }

    public async Task<bool> RequestPermissionAsync()
    {
        return await NotificationPermissionHelper.EnsureNotificationPermissionAsync();
    }

    public async Task ShowNotificationAsync(string title, string message, string channelId, int notificationId = 0, string? targetRoute = null)
    {
        try
        {
            var hasPermission = await RequestPermissionAsync();
            if (!hasPermission)
            {
                _logger.LogWarning("[AndroidNativeNotificationService] Permission denied for POST_NOTIFICATIONS");
                return;
            }

            var context = global::Android.App.Application.Context;
            if (notificationId == 0)
            {
                notificationId = (int)(DateTime.UtcNow.Ticks % int.MaxValue);
            }

            var intent = new Intent(context, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);
            if (!string.IsNullOrEmpty(targetRoute))
            {
                intent.PutExtra("target_route", targetRoute);
            }

            var pendingIntent = PendingIntent.GetActivity(
                context,
                notificationId,
                intent,
                SafeImmutableFlags);

            int smallIcon = context.ApplicationInfo?.Icon != 0
                ? context.ApplicationInfo.Icon
                : global::Android.Resource.Drawable.IcDialogInfo;

            var builder = new NotificationCompat.Builder(context, channelId)
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
                notificationManager.Notify(notificationId, notification);
            }

            _logger.LogInformation("[AndroidNativeNotificationService] Sent notification #{Id} to channel '{Channel}'", notificationId, channelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AndroidNativeNotificationService] Failed to show notification");
        }
    }

    public Task ScheduleDailyReminderAsync(int targetHour = 21, int targetMinute = 0)
    {
        try
        {
            var context = global::Android.App.Application.Context;
            Preferences.Default.Set("daily_expense_reminder_enabled", true);
            Preferences.Default.Set("daily_expense_reminder_hour", targetHour);
            Preferences.Default.Set("daily_expense_reminder_minute", targetMinute);

            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, targetHour, targetMinute, 0);
            if (now >= nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var epochDiff = nextRun.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long triggerAtMillis = (long)epochDiff.TotalMilliseconds;

            var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
            var reminderIntent = new Intent(context, typeof(DailyReminderReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(
                context,
                DailyReminderPendingIntentId,
                reminderIntent,
                SafeImmutableFlags);

            if (alarmManager != null && pendingIntent != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var showIntent = PendingIntent.GetActivity(
                        context,
                        1004,
                        new Intent(context, typeof(MainActivity)),
                        SafeImmutableFlags);
                    var alarmClockInfo = new AlarmManager.AlarmClockInfo(triggerAtMillis, showIntent);
                    alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
                }
                else
                {
                    alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
                }

                _logger.LogInformation("[AndroidNativeNotificationService] Exact AlarmManager scheduled for {TargetTime}", nextRun.ToString("HH:mm dd/MM/yyyy"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AndroidNativeNotificationService] Failed to schedule exact alarm");
        }

        return Task.CompletedTask;
    }

    public Task ScheduleTestReminderAsync(int delaySeconds = 10)
    {
        try
        {
            Preferences.Default.Set("daily_expense_reminder_enabled", true);
            var context = global::Android.App.Application.Context;
            var triggerAtMillis = Java.Lang.JavaSystem.CurrentTimeMillis() + (delaySeconds * 1000);

            var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
            var reminderIntent = new Intent(context, typeof(DailyReminderReceiver));
            reminderIntent.SetAction("com.expensetracker.action.DAILY_REMINDER");
            var pendingIntent = PendingIntent.GetBroadcast(
                context,
                DailyReminderPendingIntentId,
                reminderIntent,
                SafeImmutableFlags);

            if (alarmManager != null && pendingIntent != null)
            {
                // Sử dụng SetAlarmClock để bảo đảm hệ điều hành Android đánh thức máy chính xác đến từng giây kể cả khi tắt app (Killed state) và Doze mode
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var showIntent = PendingIntent.GetActivity(
                        context,
                        1004,
                        new Intent(context, typeof(MainActivity)),
                        SafeImmutableFlags);
                    var alarmClockInfo = new AlarmManager.AlarmClockInfo(triggerAtMillis, showIntent);
                    alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
                }
                else
                {
                    alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
                }

                _logger.LogInformation("[AndroidNativeNotificationService] Test Alarm Clock scheduled in {Delay}s for Killed-state verification", delaySeconds);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AndroidNativeNotificationService] Failed to schedule test reminder");
        }

        return Task.CompletedTask;
    }

    public Task CancelDailyReminderAsync()
    {
        try
        {
            Preferences.Default.Set("daily_expense_reminder_enabled", false);
            var context = global::Android.App.Application.Context;
            var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
            var reminderIntent = new Intent(context, typeof(DailyReminderReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(
                context,
                DailyReminderPendingIntentId,
                reminderIntent,
                SafeImmutableFlags);

            if (alarmManager != null && pendingIntent != null)
            {
                alarmManager.Cancel(pendingIntent);
                _logger.LogInformation("[AndroidNativeNotificationService] Exact AlarmManager cancelled");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AndroidNativeNotificationService] Failed to cancel daily reminder alarm");
        }

        return Task.CompletedTask;
    }
}
#endif
