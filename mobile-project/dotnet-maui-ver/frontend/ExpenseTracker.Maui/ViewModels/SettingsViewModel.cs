using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Config;
using ExpenseTracker.Maui.Services.Notifications;
using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Biometrics;
using ExpenseTracker.Maui.Services.Security;

namespace ExpenseTracker.Maui.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IServerConfigService _serverConfig;
    private readonly INativeNotificationService _nativeNotification;
    private readonly ITokenStorageService _tokenStorage;
    private readonly IBiometricAuthService _biometricAuth;
    private readonly IClientAuthService _authService;

    [ObservableProperty]
    private bool _isBiometricQuickLoginEnabled = true;

    [ObservableProperty]
    private string _biometricHardwareStatusText = "Đang kiểm tra phần cứng...";

    [ObservableProperty]
    private bool _isBiometricSupported = true;

    [ObservableProperty]
    private bool _isLiveApiMode;

    [ObservableProperty]
    private string _serverUrl = string.Empty;

    [ObservableProperty]
    private string _connectionStatusText = string.Empty;

    [ObservableProperty]
    private bool _isPinging;

    [ObservableProperty]
    private string? _pingResult;

    [ObservableProperty]
    private bool _pingSuccess;

    [ObservableProperty]
    private string? _simulationStatus;

    public SettingsViewModel(
        IServerConfigService serverConfig,
        INativeNotificationService nativeNotification,
        ITokenStorageService tokenStorage,
        IBiometricAuthService biometricAuth,
        IClientAuthService authService)
    {
        _serverConfig = serverConfig;
        _nativeNotification = nativeNotification;
        _tokenStorage = tokenStorage;
        _biometricAuth = biometricAuth;
        _authService = authService;
        Title = "Cài Đặt Hệ Thống & Bảo Mật";

        LoadConfig();
        _ = LoadBiometricConfigAsync();

        _serverConfig.OnConfigChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(LoadConfig);
        };
    }

    private async Task LoadBiometricConfigAsync()
    {
        try
        {
            IsBiometricQuickLoginEnabled = await _tokenStorage.IsQuickLoginEnabledAsync();

            var capability = await _biometricAuth.CheckCapabilityAsync();
            switch (capability)
            {
                case BiometricCapabilityStatus.Available:
                    BiometricHardwareStatusText = "Cảm biến vân tay / FaceID sẵn sàng hoạt động.";
                    IsBiometricSupported = true;
                    break;
                case BiometricCapabilityStatus.NotEnrolled:
                    BiometricHardwareStatusText = "Máy có cảm biến nhưng chưa thiết lập vân tay trong hệ điều hành.";
                    IsBiometricSupported = false;
                    break;
                case BiometricCapabilityStatus.Unsupported:
                default:
                    BiometricHardwareStatusText = "Thiết bị không hỗ trợ phần cứng sinh trắc học.";
                    IsBiometricSupported = false;
                    break;
            }
        }
        catch (Exception ex)
        {
            BiometricHardwareStatusText = $"Lỗi kiểm tra: {ex.Message}";
        }
    }

    partial void OnIsBiometricQuickLoginEnabledChanged(bool value)
    {
        _ = _tokenStorage.SetQuickLoginEnabledAsync(value);
    }

    private void LoadConfig()
    {
        IsLiveApiMode = _serverConfig.IsLiveApiMode;
        ServerUrl = _serverConfig.ServerBaseUrl;
        ConnectionStatusText = _serverConfig.ConnectionStatus;
    }

    partial void OnIsLiveApiModeChanged(bool value)
    {
        if (_serverConfig != null && _serverConfig.IsLiveApiMode != value)
        {
            _serverConfig.IsLiveApiMode = value;
            ConnectionStatusText = _serverConfig.ConnectionStatus;
        }
    }

    [RelayCommand]
    private void ToggleMode(bool isLive)
    {
        _serverConfig.IsLiveApiMode = isLive;
        LoadConfig();
    }

    [RelayCommand]
    private void SaveServerUrl()
    {
        if (string.IsNullOrWhiteSpace(ServerUrl))
        {
            ServerUrl = ServerConfigService.GetDefaultServerUrl();
        }

        _serverConfig.ServerBaseUrl = ServerUrl;
        LoadConfig();
    }

    [RelayCommand]
    private async Task UseLocalhostAsync()
    {
        ServerUrl = ServerConfigService.LocalhostUrl;
        _serverConfig.ServerBaseUrl = ServerUrl;
        LoadConfig();
        await PingServerAsync();
    }

    [RelayCommand]
    private async Task UseTunnelAsync()
    {
        ServerUrl = ServerConfigService.ProductionTunnelUrl;
        _serverConfig.ServerBaseUrl = ServerUrl;
        LoadConfig();
        await PingServerAsync();
    }

    [RelayCommand]
    private async Task PingServerAsync()
    {
        if (IsPinging) return;

        try
        {
            IsPinging = true;
            PingResult = "Đang kiểm tra kết nối tới máy chủ...";
            PingSuccess = false;

            bool isAlive = await _serverConfig.PingServerAsync(ServerUrl);

            if (isAlive)
            {
                PingSuccess = true;
                PingResult = "Kết nối máy chủ thành công (HTTP 200 OK).";
            }
            else
            {
                PingSuccess = false;
                PingResult = "Không thể kết nối máy chủ. Vui lòng kiểm tra lại đường truyền hoặc Tunnel.";
            }
        }
        catch (Exception ex)
        {
            PingSuccess = false;
            PingResult = $"Lỗi kết nối: {ex.Message}";
        }
        finally
        {
            IsPinging = false;
        }
    }

    [RelayCommand]
    private void ResetDefaults()
    {
        _serverConfig.ResetToDefaults();
        LoadConfig();
        PingResult = null;
    }

    [RelayCommand]
    private async Task SimulateBankNotificationAsync()
    {
        try
        {
            // Mô phỏng nhận diện biến động số dư MoMo: -75.000đ tại Highlands Coffee
            string rawMsg = "Ban da thanh toan 75.000d tai Highlands Coffee. So du vi con lai: 2.450.000d";
            var parseResult = BankNotificationParser.Parse("com.mservice.momotransfer", "Ví MoMo", rawMsg);

            if (parseResult.IsSuccess)
            {
                SimulationStatus = $"Đã phát hiện thành công: -{parseResult.Amount:N0} đ tại Highlands Coffee.";

                // Bắn thông báo Native Heads-Up
                await _nativeNotification.ShowNotificationAsync(
                    $"Phát hiện chi tiêu: -{parseResult.Amount:N0} đ",
                    $"[Ví MoMo] {parseResult.ExtractedNote}. Chạm để tự động lưu ngay!",
                    NotificationChannelIds.BalanceFluctuation,
                    101,
                    $"///ExpensesPage?amount={parseResult.Amount}&bank={Uri.EscapeDataString(parseResult.BankName)}&note={Uri.EscapeDataString(parseResult.ExtractedNote)}");

                if (Shell.Current?.CurrentPage != null)
                {
                    await Shell.Current.CurrentPage.DisplayAlertAsync(
                        "Kiểm Chứng Bank Notification",
                        $"Hệ thống đã nhận diện thông báo từ Ví MoMo:\n- Số tiền: -{parseResult.Amount:N0} VNĐ\n- Nội dung: {parseResult.ExtractedNote}\n\nThông báo Heads-up đã được đẩy lên thanh trạng thái Android!",
                        "Đóng");
                }
            }
            else
            {
                SimulationStatus = "Không bóc tách được tin nhắn mẫu!";
            }
        }
        catch (Exception ex)
        {
            SimulationStatus = $"Lỗi giả lập: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ScheduleTestReminderAsync()
    {
        await _nativeNotification.RequestPermissionAsync();
        await _nativeNotification.ScheduleTestReminderAsync(10);

        if (Shell.Current?.CurrentPage != null)
        {
            await Shell.Current.CurrentPage.DisplayAlertAsync(
                "Đã Hẹn Giờ Nhắc Nhở Sau 10 Giây",
                "1. Bạn hãy VUỐT TẮT ỨNG DỤNG HOÀN TOÀN khỏi danh sách đa nhiệm (Killed State) ngay bây giờ.\n2. Đưa điện thoại cho Thầy cầm hoặc đặt lên bàn.\n\nSau đúng 10 giây, Android OS sẽ tự động đánh thức và rung chuông phát thông báo lên màn hình!",
                "Tôi đã hiểu, vuốt tắt ngay");
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (Shell.Current?.CurrentPage != null)
        {
            bool confirm = await Shell.Current.CurrentPage.DisplayAlertAsync(
                "Xác Nhận Đăng Xuất",
                "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản không?\n\n* Lưu ý: Khi đã đăng xuất, tính năng Đăng nhập nhanh bằng Sinh trắc học sẽ tự động khóa và bắt buộc đăng nhập bằng Mật khẩu ở lần mở app tiếp theo.",
                "Đăng Xuất",
                "Hủy");

            if (!confirm) return;
        }

        try
        {
            await _authService.LogoutAsync();
            await _tokenStorage.PurgeSessionOnLogoutAsync();
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current?.CurrentPage != null)
            {
                await Shell.Current.CurrentPage.DisplayAlertAsync("Lỗi", $"Đăng xuất thất bại: {ex.Message}", "Đóng");
            }
        }
    }
}
