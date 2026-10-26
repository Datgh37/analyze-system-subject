using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace ExpenseTracker.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        HandleNotificationDeepLink(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        HandleNotificationDeepLink(intent);
    }

    private void HandleNotificationDeepLink(Intent? intent)
    {
        if (intent == null) return;

        var targetRoute = intent.GetStringExtra("target_route");
        if (!string.IsNullOrEmpty(targetRoute))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    // Chờ Shell khởi tạo xong nếu ứng dụng vừa mở
                    int retries = 0;
                    while (Shell.Current == null && retries < 10)
                    {
                        await Task.Delay(200);
                        retries++;
                    }

                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync(targetRoute);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Lỗi điều hướng từ thông báo DeepLink: {ex.Message}");
                }
            });
        }
    }
}
