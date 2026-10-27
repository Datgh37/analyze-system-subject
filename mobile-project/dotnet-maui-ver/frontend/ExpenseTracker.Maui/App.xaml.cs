using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Config;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Maui;

public partial class App : Application
{
    private readonly IClientAuthService _authService;
    private readonly IServerConfigService _serverConfigService;

    public App(IClientAuthService authService, IServerConfigService serverConfigService)
    {
        InitializeComponent();
        _authService = authService;
        _serverConfigService = serverConfigService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        _ = CheckAutoLoginAsync(shell);
        _ = CheckServerConnectionAsync();
        return new Window(shell);
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