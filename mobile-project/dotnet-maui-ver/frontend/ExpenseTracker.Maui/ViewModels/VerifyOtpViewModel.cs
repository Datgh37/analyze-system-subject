using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Auth;

namespace ExpenseTracker.Maui.ViewModels;

// View model managing 6-digit OTP verification and resend countdown
[QueryProperty(nameof(Email), "email")]
public partial class VerifyOtpViewModel : BaseViewModel
{
    private readonly IClientAuthService _authService;
    private IDispatcherTimer? _countdownTimer;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _otpCode = string.Empty;

    [ObservableProperty]
    private int _countdownSeconds = 60;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResendTextColor))]
    private bool _canResend = false;

    public string ResendTextColor => CanResend ? "#4F46E5" : "#94A3B8";

    [ObservableProperty]
    private string _resendButtonText = "Gửi lại mã (60s)";

    [ObservableProperty]
    private string? _infoMessage;

    public VerifyOtpViewModel(IClientAuthService authService)
    {
        _authService = authService;
        Title = "Xác Thực Tài Khoản";
    }

    // Called when navigation parameter is applied
    partial void OnEmailChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            StartCountdown();
        }
    }

    public void StartCountdown()
    {
        _countdownTimer?.Stop();
        CountdownSeconds = 60;
        CanResend = false;
        ResendButtonText = "Gửi lại mã (60s)";

        var dispatcher = Application.Current?.Dispatcher ?? Dispatcher.GetForCurrentThread();
        if (dispatcher == null) return;

        _countdownTimer = dispatcher.CreateTimer();
        _countdownTimer.Interval = TimeSpan.FromSeconds(1);
        _countdownTimer.Tick += (s, e) =>
        {
            if (CountdownSeconds > 1)
            {
                CountdownSeconds--;
                ResendButtonText = $"Gửi lại mã ({CountdownSeconds}s)";
            }
            else
            {
                CountdownSeconds = 0;
                CanResend = true;
                ResendButtonText = "Gửi lại mã OTP";
                _countdownTimer?.Stop();
            }
        };
        _countdownTimer.Start();
    }

    [RelayCommand]
    private async Task VerifyOtpAsync()
    {
        if (IsBusy) return;

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Địa chỉ email không xác định.";
            return;
        }

        var trimmedCode = OtpCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(trimmedCode) || trimmedCode.Length != 6)
        {
            ErrorMessage = "Vui lòng nhập đầy đủ mã OTP 6 chữ số.";
            return;
        }

        try
        {
            IsBusy = true;
            var (success, error) = await _authService.VerifyOtpAsync(Email, trimmedCode);

            if (success)
            {
                _countdownTimer?.Stop();
                await Shell.Current.GoToAsync("//WalletsPage");
            }
            else
            {
                ErrorMessage = error ?? "Mã OTP không chính xác hoặc đã hết hạn. Vui lòng kiểm tra lại.";
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
    private async Task ResendOtpAsync()
    {
        if (IsBusy || !CanResend) return;

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Địa chỉ email không xác định.";
            return;
        }

        try
        {
            IsBusy = true;
            var (success, error) = await _authService.ResendOtpAsync(Email);

            if (success)
            {
                InfoMessage = "Mã xác thực mới đã được gửi tới email của bạn.";
                StartCountdown();
            }
            else
            {
                ErrorMessage = error ?? "Không thể gửi lại mã vào lúc này. Vui lòng thử lại sau.";
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
        _countdownTimer?.Stop();
        await Shell.Current.GoToAsync("..");
    }
}
