using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Config;
using ExpenseTracker.Maui.Services.Notifications;
using ExpenseTracker.Maui.Services.Security;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Maui;

public partial class App : Application
{
    private readonly IClientAuthService _authService;
    private readonly IServerConfigService _serverConfigService;
    private readonly INativeNotificationService _nativeNotificationService;
    private readonly ITokenStorageService _tokenStorage;

    public App(
        IClientAuthService authService, 
        IServerConfigService serverConfigService,
        INativeNotificationService nativeNotificationService,
        ITokenStorageService tokenStorage)
    {
        InitializeComponent();
        _authService = authService;
        _serverConfigService = serverConfigService;
        _nativeNotificationService = nativeNotificationService;
        _tokenStorage = tokenStorage;
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
            var hasActiveSession = await _tokenStorage.HasActiveSessionAsync();
            var isQuickLoginEnabled = await _tokenStorage.IsQuickLoginEnabledAsync();

            // NGUYÊN TẮC BẤT DI BẤT DỊCH:
            // Nếu người dùng đã chủ động bấm Đăng xuất (hasActiveSession == false),
            // TUYỆT ĐỐI KHÔNG HIỆN Quick Login, bắt buộc vào thẳng LoginPage!
            if (!hasActiveSession)
            {
                await shell.GoToAsync("//LoginPage");
                return;
            }

            // Nếu phiên còn hoạt động VÀ người dùng BẬT tính năng Đăng nhập nhanh
            if (isQuickLoginEnabled)
            {
                await shell.GoToAsync("//QuickLoginPage");
                return;
            }

            // Nếu người dùng TẮT Quick Login nhưng phiên vẫn còn hiệu lực
            var isAuthenticated = await _authService.InitializeAsync();
            if (isAuthenticated)
            {
                await shell.GoToAsync("//WalletsPage");
            }
            else
            {
                await shell.GoToAsync("//LoginPage");
            }
        }
        catch
        {
            await shell.GoToAsync("//LoginPage");
        }
    }
}