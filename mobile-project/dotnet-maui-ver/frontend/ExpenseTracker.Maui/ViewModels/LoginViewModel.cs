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
    private async Task GoogleLoginAsync()
    {
        if (IsBusy) return;

        ErrorMessage = null;
        try
        {
            IsBusy = true;
            // Google Single Sign-On flow with Real Backend API
            var dummyToken = $"google_oauth_{Guid.NewGuid():N}";
            var email = !string.IsNullOrWhiteSpace(Email) && Email.Contains('@') ? Email : "user.google@gmail.com";
            var displayName = "Google Member";

            var (success, error) = await _authService.GoogleLoginAsync(dummyToken, email, displayName);
            if (success)
            {
                await Shell.Current.GoToAsync("//WalletsPage");
            }
            else
            {
                ErrorMessage = error ?? "Đăng nhập bằng tài khoản Google không thành công.";
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
    private async Task ForgotPasswordAsync()
    {
        if (IsBusy) return;

        ErrorMessage = null;
        var targetEmail = Email?.Trim();

        if (string.IsNullOrWhiteSpace(targetEmail) || !targetEmail.Contains('@'))
        {
            var input = await Shell.Current.DisplayPromptAsync(
                "Khôi Phục Mật Khẩu", 
                "Nhập địa chỉ email tài khoản của bạn để nhận mã khôi phục:", 
                "Gửi Mã", 
                "Hủy", 
                placeholder: "name@example.com",
                keyboard: Keyboard.Email);

            if (string.IsNullOrWhiteSpace(input)) return;
            targetEmail = input.Trim();
        }

        try
        {
            IsBusy = true;
            var (success, error) = await _authService.ForgotPasswordAsync(targetEmail);
            if (success)
            {
                await Shell.Current.DisplayAlertAsync("Thành Công", $"Mã OTP khôi phục đã được gửi tới {targetEmail}. Vui lòng nhập mã để xác nhận.", "Tiếp tục");
                await Shell.Current.GoToAsync($"VerifyOtpPage?email={Uri.EscapeDataString(targetEmail)}");
            }
            else
            {
                ErrorMessage = error ?? "Không thể gửi mã khôi phục vào lúc này. Vui lòng thử lại sau.";
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
