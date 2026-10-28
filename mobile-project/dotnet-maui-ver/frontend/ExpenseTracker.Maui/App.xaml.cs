using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Config;
using ExpenseTracker.Maui.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Maui;

public partial class App : Application
{
    private readonly IClientAuthService _authService;
    private readonly IServerConfigService _serverConfigService;
    private readonly INativeNotificationService _nativeNotificationService;

    public App(
        IClientAuthService authService, 
        IServerConfigService serverConfigService,
        INativeNotificationService nativeNotificationService)
    {
        InitializeComponent();
        _authService = authService;
        _serverConfigService = serverConfigService;
        _nativeNotificationService = nativeNotificationService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        _ = CheckAutoLoginAsync(shell);
        _ = CheckServerConnectionAsync();
        _ = RequestStartupPermissionsAsync();
        return new Window(shell);
    }

    private async Task RequestStartupPermissionsAsync()
    {
        try
        {
            // Đợi 800ms để giao diện Shell ổn định rồi kích hoạt hỏi quyền thông báo hệ thống
            await Task.Delay(800);
            await _nativeNotificationService.RequestPermissionAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App] Lỗi kích hoạt quyền khởi động: {ex.Message}");
        }
    }

    private async Task CheckServerConnectionAsync()
    {
        await _serverConfigService.StartConnectionCheckWithRetryAsync(message =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Current?.Windows.Count > 0 && Current.Windows[0].Page != null)
                {
                    await Current.Windows[0].Page!.DisplayAlertAsync("Thông Báo", message, "Đóng");
                }
            });
        });
    }

    private async Task CheckAutoLoginAsync(AppShell shell)
    {
        try
        {
            var isAuthenticated = await _authService.InitializeAsync();
            if (!isAuthenticated)
            {
                // Direct unauthenticated user to Login screen
                await shell.GoToAsync("//LoginPage");
            }
            else
            {
                // Direct authenticated user to primary Wallets dashboard
                await shell.GoToAsync("//WalletsPage");
            }
        }
        catch
        {
            await shell.GoToAsync("//LoginPage");
        }
    }
}