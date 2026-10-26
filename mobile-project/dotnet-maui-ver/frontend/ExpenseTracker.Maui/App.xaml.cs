using ExpenseTracker.Maui.Services.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Maui;

public partial class App : Application
{
    private readonly IClientAuthService _authService;

    public App(IClientAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        _ = CheckAutoLoginAsync(shell);
        return new Window(shell);
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