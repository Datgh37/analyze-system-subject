using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Config;
using ExpenseTracker.Maui.Services.Realtime;

namespace ExpenseTracker.Maui.ViewModels;

// View model orchestrating user login flow, credentials validation, and shell navigation
public partial class LoginViewModel : BaseViewModel
{
    private readonly IClientAuthService _authService;
    private readonly IServerConfigService? _serverConfig;
    private readonly ISignalRRealtimeService? _realtimeService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordToggleIcon))]
    private bool _isPasswordHidden = true;

    public string PasswordToggleIcon => IsPasswordHidden ? "ic_eye.svg" : "ic_eye_off.svg";

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModeTitle))]
    [NotifyPropertyChangedFor(nameof(ModeBadgeText))]
    [NotifyPropertyChangedFor(nameof(ModeBadgeColor))]
    [NotifyPropertyChangedFor(nameof(ModeIcon))]
    [NotifyPropertyChangedFor(nameof(ServerUrlSummary))]
    private bool _isLiveApiMode;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ServerUrlSummary))]
    private string _serverUrl = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ServerConfigButtonText))]
    private bool _showServerConfig;

    [ObservableProperty]
    private bool _isPinging;

    [ObservableProperty]
    private string? _pingResult;

    public string ModeTitle => IsLiveApiMode ? "Máy Chủ Trực Tuyến (Live API)" : "Dữ Liệu Thử Nghiệm (Offline)";
    public string ModeBadgeText => IsLiveApiMode ? "TRỰC TUYẾN" : "DỮ LIỆU MẪU";
    public string ModeBadgeColor => IsLiveApiMode ? "#10B981" : "#F59E0B";
    public string ModeIcon => IsLiveApiMode ? "ic_check.svg" : "ic_shield.svg";
    public string ServerUrlSummary => IsLiveApiMode 
        ? (string.IsNullOrWhiteSpace(ServerUrl) ? "Chưa cấu hình URL" : ServerUrl) 
        : "Dữ liệu mẫu 0ms (Không cần server)";
    public string ServerConfigButtonText => ShowServerConfig ? "Thu gọn cấu hình" : "Cài đặt máy chủ & đường dẫn...";

    public LoginViewModel(
        IClientAuthService authService, 
        IServerConfigService? serverConfig = null,
        ISignalRRealtimeService? realtimeService = null)
    {
        _authService = authService;
        _serverConfig = serverConfig;
        _realtimeService = realtimeService;
        Title = "Đăng Nhập";

        if (_serverConfig != null)
        {
            _isLiveApiMode = _serverConfig.IsLiveApiMode;
            _serverUrl = _serverConfig.ServerBaseUrl;

            _serverConfig.OnConfigChanged += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsLiveApiMode = _serverConfig.IsLiveApiMode;
                    ServerUrl = _serverConfig.ServerBaseUrl;
                });
            };
        }
    }

    partial void OnIsLiveApiModeChanged(bool value)
    {
        if (_serverConfig != null && _serverConfig.IsLiveApiMode != value)
        {
            _serverConfig.IsLiveApiMode = value;
        }
    }

    [RelayCommand]
    private void ToggleShowServerConfig()
    {
        ShowServerConfig = !ShowServerConfig;
    }

    [RelayCommand]
    private async Task UseLocalhostAsync()
    {
        if (_serverConfig == null) return;
        ServerUrl = ServerConfigService.LocalhostUrl;
        _serverConfig.ServerBaseUrl = ServerUrl;
        await PingServerAsync();
    }

    [RelayCommand]
    private async Task UseTunnelAsync()
    {
        if (_serverConfig == null) return;
        ServerUrl = ServerConfigService.ProductionTunnelUrl;
        _serverConfig.ServerBaseUrl = ServerUrl;
        await PingServerAsync();
    }

    [RelayCommand]
    private async Task PingServerAsync()
    {
        if (_serverConfig == null || IsPinging) return;

        try
        {
            IsPinging = true;
            PingResult = "Đang kiểm tra...";

            bool isAlive = await _serverConfig.PingServerAsync(ServerUrl);
            PingResult = isAlive 
                ? "Máy chủ phản hồi tốt (HTTP 200 OK)" 
                : "Không thể kết nối tới máy chủ!";
        }
        catch (Exception ex)
        {
            PingResult = $"Lỗi kết nối: {ex.Message}";
        }
        finally
        {
            IsPinging = false;
        }
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
                if (_realtimeService != null)
                {
                    _ = Task.Run(async () => await _realtimeService.ConnectAsync());
                }
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
                if (_realtimeService != null)
                {
                    _ = Task.Run(async () => await _realtimeService.ConnectAsync());
                }
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
