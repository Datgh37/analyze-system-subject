using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Auth;

namespace ExpenseTracker.Maui.ViewModels;

// View model orchestrating user login flow, credentials validation, and shell navigation
public partial class LoginViewModel : BaseViewModel
{
    private readonly IClientAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    public LoginViewModel(IClientAuthService authService)
    {
        _authService = authService;
        Title = "Đăng Nhập";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Vui lòng nhập đầy đủ email và mật khẩu.";
            return;
        }

        try
        {
            IsBusy = true;
            var (success, error) = await _authService.LoginAsync(Email, Password);

            if (success)
            {
                await Shell.Current.GoToAsync("//WalletsPage");
            }
            else
            {
                ErrorMessage = error ?? "Thông tin đăng nhập không chính xác. Vui lòng thử lại.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToRegisterAsync()
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }

    // Allows previewing Wallets UI directly during inspection
    [RelayCommand]
    private async Task SkipToWalletsAsync()
    {
        await Shell.Current.GoToAsync("//WalletsPage");
    }
}
