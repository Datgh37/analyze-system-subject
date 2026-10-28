using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Config;
using ExpenseTracker.Maui.Services.Notifications;

namespace ExpenseTracker.Maui.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IServerConfigService _serverConfig;
    private readonly INativeNotificationService _nativeNotification;

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
        INativeNotificationService nativeNotification)
    {
        _serverConfig = serverConfig;
        _nativeNotification = nativeNotification;
        Title = "Cài Đặt Máy Chủ & Demo";

        LoadConfig();

        _serverConfig.OnConfigChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(LoadConfig);
        };
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
                PingResult = "🟢 Kết nối máy chủ thành công! (HTTP 200 OK)";
            }
            else
            {
                PingSuccess = false;
                PingResult = "🔴 Không thể kết nối tới máy chủ! Vui lòng kiểm tra lại đường truyền hoặc Tunnel.";
            }
        }
        catch (Exception ex)
        {
            PingSuccess = false;
            PingResult = $"🔴 Lỗi: {ex.Message}";
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
                SimulationStatus = $"💳 Đã bắt thành công: -{parseResult.Amount:N0}đ tại Highlands Coffee!";

                // Bắn thông báo Native Heads-Up
                await _nativeNotification.ShowNotificationAsync(
                    $"💳 Phát hiện chi tiêu: -{parseResult.Amount:N0} đ",
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
                "⏰ ĐÃ HẸN GIỜ BÁO THỨC SAU 10 GIÂY!",
                "1. Bạn hãy VUỐT TẮT ỨNG DỤNG HOÀN TOÀN khỏi danh sách đa nhiệm (Killed State) ngay bây giờ.\n2. Đưa điện thoại cho Thầy cầm hoặc đặt lên bàn.\n\nSau đúng 10 giây, Android OS sẽ tự động đánh thức và rung chuông phát thông báo lên màn hình!",
                "Tôi đã hiểu, vuốt tắt ngay");
        }
    }
}
