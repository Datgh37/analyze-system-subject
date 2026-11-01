#if ANDROID
#pragma warning disable CA1416, CS8602, CS8604

using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.Notification;
using AndroidX.Core.App;
using ExpenseTracker.Maui.Services.Notifications;

namespace ExpenseTracker.Maui.Platforms.Android.Services;

// Android Native Notification Listener Service (UC-03.12 Production Implementation)
// Listens for balance variation notifications from target banking & e-wallet applications,
// parses deducted amount, and raises a clickable actionable notification directly to ExpensesPage
[Service(Label = "BankNotificationListenerService",
    Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
    Exported = true)]
[IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
public class BankNotificationListenerService : NotificationListenerService
{
    private const PendingIntentFlags SafeImmutableFlags = PendingIntentFlags.UpdateCurrent | (PendingIntentFlags)0x04000000;

    public override void OnNotificationPosted(StatusBarNotification? sbn)
    {
        if (sbn == null || string.IsNullOrWhiteSpace(sbn.PackageName))
            return;

        if (!BankNotificationParser.IsSupportedPackage(sbn.PackageName))
            return;

        var extras = sbn.Notification?.Extras;
        if (extras == null)
            return;

        string title = extras.GetCharSequence(Notification.ExtraTitle)?.ToString() ?? string.Empty;
        string text = extras.GetCharSequence(Notification.ExtraText)?.ToString() ?? string.Empty;

        var parseResult = BankNotificationParser.Parse(sbn.PackageName, title, text);
        if (!parseResult.IsSuccess)
            return;

        // Bắn Native Notification yêu cầu người dùng chạm để tự động lưu chi tiêu
        PostDeductionActionNotification(parseResult);
    }

    private void PostDeductionActionNotification(BankDeductionResult result)
    {
        try
        {
            var context = global::Android.App.Application.Context;
            int notificationId = (int)(DateTime.UtcNow.Ticks % int.MaxValue);

            // Xây dựng Deep Link kèm query parameters cho Shell điều hướng
            string targetRoute = $"///ExpensesPage?amount={result.Amount}&bank={Uri.EscapeDataString(result.BankName)}&note={Uri.EscapeDataString(result.ExtractedNote)}";

            var launchIntent = new Intent(context, typeof(MainActivity));
            launchIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);
            launchIntent.PutExtra("target_route", targetRoute);

            var pendingIntent = PendingIntent.GetActivity(
                context,
                notificationId,
                launchIntent,
                SafeImmutableFlags);

            int smallIcon = context.ApplicationInfo?.Icon != 0
                ? (context.ApplicationInfo?.Icon ?? global::Android.Resource.Drawable.IcDialogInfo)
                : global::Android.Resource.Drawable.IcDialogInfo;

            string notifTitle = $"💳 Phát hiện chi tiêu: -{result.Amount:N0} đ";
            string notifMessage = $"[{result.BankName}] {result.ExtractedNote}. Chạm để tự động lưu vào sổ chi tiêu ngay!";

            var builder = new NotificationCompat.Builder(context, NotificationChannelIds.BalanceFluctuation)
                .SetSmallIcon(smallIcon)
                .SetContentTitle(notifTitle)
                .SetContentText(notifMessage)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(notifMessage))
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

            System.Diagnostics.Debug.WriteLine($"[BankNotificationListener] Đã phát Action Notification cho {result.BankName}: -{result.Amount:N0}đ");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BankNotificationListener] Lỗi khi phát Action Notification: {ex.Message}");
        }
    }
}
#endif
