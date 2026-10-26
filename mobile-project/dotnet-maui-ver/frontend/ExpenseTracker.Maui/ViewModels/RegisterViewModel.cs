using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Auth;

namespace ExpenseTracker.Maui.ViewModels;

// View model managing new user account creation and password confirmation
public partial class RegisterViewModel : BaseViewModel
{
    private readonly IClientAuthService _authService;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string _baseCurrency = "VND";

    public RegisterViewModel(IClientAuthService authService)
    {
        _authService = authService;
        Title = "Đăng Ký Tài Khoản";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "Vui lòng nhập họ và tên của bạn.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Vui lòng nhập địa chỉ email hợp lệ.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Mật khẩu xác nhận không khớp.";
            return;
        }

        try
        {
            IsBusy = true;
            var (success, error) = await _authService.RegisterAsync(FullName, Email, Password, BaseCurrency);

            if (success)
            {
                await Shell.Current.GoToAsync($"VerifyOtpPage?email={Uri.EscapeDataString(Email)}");
            }
            else
            {
                ErrorMessage = error ?? "Đăng ký không thành công. Vui lòng thử lại.";
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
    private async Task NavigateToLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
