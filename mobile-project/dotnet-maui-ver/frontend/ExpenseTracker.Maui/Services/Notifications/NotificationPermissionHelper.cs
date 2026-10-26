namespace ExpenseTracker.Maui.Services.Notifications;

// Helper managing runtime notification permissions especially for Android 13+ (API 33 Tiramisu)
public static class NotificationPermissionHelper
{
    private const string PostNotificationPermission = "android.permission.POST_NOTIFICATIONS";

    public static async Task<bool> EnsureNotificationPermissionAsync()
    {
#if ANDROID
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            var context = activity ?? Android.App.Application.Context;

            var permissionStatus = AndroidX.Core.Content.ContextCompat.CheckSelfPermission(
                context, 
                PostNotificationPermission);

            if (permissionStatus == Android.Content.PM.Permission.Granted)
            {
                return true;
            }

            if (activity != null)
            {
                AndroidX.Core.App.ActivityCompat.RequestPermissions(
                    activity, 
                    new[] { PostNotificationPermission }, 
                    1001);
            }

            // Return current state after request prompt
            return AndroidX.Core.Content.ContextCompat.CheckSelfPermission(
                context, 
                PostNotificationPermission) == Android.Content.PM.Permission.Granted;
        }

        return true;
#else
        await Task.CompletedTask;
        return true;
#endif
    }

    public static void OpenNotificationListenerSettings()
    {
#if ANDROID
#pragma warning disable CA1416
        try
        {
            var intent = new Android.Content.Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            intent.SetFlags(Android.Content.ActivityFlags.NewTask);
            global::Android.App.Application.Context.StartActivity(intent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationPermissionHelper] Cannot open listener settings: {ex.Message}");
        }
#pragma warning restore CA1416
#endif
    }
}
