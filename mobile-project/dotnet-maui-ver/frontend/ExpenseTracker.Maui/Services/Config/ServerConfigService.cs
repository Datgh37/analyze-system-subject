using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Services.Realtime.Messages;

namespace ExpenseTracker.Maui.Services.Config;

public class ServerConfigService : IServerConfigService
{
    private const string PrefKeyIsLiveApiMode = "ExpenseTracker_IsLiveApiMode";
    private const string PrefKeyServerBaseUrl = "ExpenseTracker_ServerBaseUrl";

    private string _connectionStatus = "Đang kiểm tra kết nối...";
    private bool _isCheckingConnection;

    public const string ProductionTunnelUrl = "https://expense-api.hobbyshop-ecommerce.me";

    public static string LocalhostUrl => DeviceInfo.Platform == DevicePlatform.Android
        ? "http://10.0.2.2:5018"
        : "http://localhost:5018";

    public event EventHandler? OnConfigChanged;

    public static string GetDefaultServerUrl()
    {
        return LocalhostUrl;
    }

    public bool IsLiveApiMode
    {
        get => Preferences.Default.Get(PrefKeyIsLiveApiMode, true);
        set
        {
            Preferences.Default.Set(PrefKeyIsLiveApiMode, value);
            _connectionStatus = value ? "Chế độ: Live Backend API" : "Chế độ: Demo Ngoại Tuyến (Offline Mock)";
            OnConfigChanged?.Invoke(this, EventArgs.Empty);
            WeakReferenceMessenger.Default.Send(new ServerModeChangedMessage(value));
        }
    }

    public string ServerBaseUrl
    {
        get => Preferences.Default.Get(PrefKeyServerBaseUrl, GetDefaultServerUrl());
        set
        {
            var trimmed = value?.Trim() ?? string.Empty;
            if (trimmed.EndsWith("/"))
            {
                trimmed = trimmed.TrimEnd('/');
            }
            Preferences.Default.Set(PrefKeyServerBaseUrl, trimmed);
            OnConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string ConnectionStatus => _connectionStatus;

    public async Task<bool> PingServerAsync(string? url = null)
    {
        var targetUrl = string.IsNullOrWhiteSpace(url) ? ServerBaseUrl : url.Trim().TrimEnd('/');
        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            return false;
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var healthUrl = $"{targetUrl}/api/health";
            var response = await client.GetAsync(healthUrl, cts.Token);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task StartConnectionCheckWithRetryAsync(Action<string>? onToastCallback = null)
    {
        if (_isCheckingConnection)
        {
            return;
        }

        // Nếu người dùng đã chủ động tắt sang Offline Mock thì không cần ping backend ngầm
        if (!IsLiveApiMode)
        {
            _connectionStatus = "Chế độ: Demo Ngoại Tuyến (Offline Mock)";
            return;
        }

        _isCheckingConnection = true;

        try
        {
            // Lần 1: Ping tức thì khi mở app
            _connectionStatus = "Đang kiểm tra kết nối máy chủ...";
            bool isOnline = await PingServerAsync(ServerBaseUrl);

            // Tự động kiểm tra fallback thông minh: nếu URL hiện tại fail (ví dụ chưa bật Tunnel) nhưng Localhost đang chạy
            if (!isOnline && ServerBaseUrl != LocalhostUrl)
            {
                bool isLocalAlive = await PingServerAsync(LocalhostUrl);
                if (isLocalAlive)
                {
                    ServerBaseUrl = LocalhostUrl;
                    _connectionStatus = "Đã tự động kết nối Backend Localhost (Port 5018)";
                    return;
                }
            }

            if (isOnline)
            {
                _connectionStatus = "Đã kết nối Live Backend API";
                return;
            }

            // Retry Policy: Thử lại 3 lần, mỗi lần cách nhau 15 giây
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                _connectionStatus = $"Không thấy máy chủ. Đang thử lại ({attempt}/3) sau 15s...";
                await Task.Delay(TimeSpan.FromSeconds(15));

                // Nếu người dùng đã gạt tay sang offline trong lúc đang chờ retry, thoát ngay
                if (!IsLiveApiMode)
                {
                    _connectionStatus = "Chế độ: Demo Ngoại Tuyến (Offline Mock)";
                    return;
                }

                isOnline = await PingServerAsync(ServerBaseUrl);
                if (isOnline)
                {
                    _connectionStatus = "Đã kết nối Live Backend API";
                    return;
                }

                // Kiểm tra lại Localhost trong lúc retry
                if (ServerBaseUrl != LocalhostUrl)
                {
                    bool isLocalAlive = await PingServerAsync(LocalhostUrl);
                    if (isLocalAlive)
                    {
                        ServerBaseUrl = LocalhostUrl;
                        _connectionStatus = "Đã tự động kết nối Backend Localhost (Port 5018)";
                        return;
                    }
                }
            }

            // Sau 3 lần retry vẫn không có kết nối: Tự động chuyển sang Offline Mock Mode
            IsLiveApiMode = false;
            _connectionStatus = "Server offline, đang chuyển sang chế độ offline";
            onToastCallback?.Invoke("Server offline, đang chuyển sang chế độ offline");
        }
        finally
        {
            _isCheckingConnection = false;
        }
    }

    public void ResetToDefaults()
    {
        Preferences.Default.Remove(PrefKeyIsLiveApiMode);
        Preferences.Default.Remove(PrefKeyServerBaseUrl);
        _connectionStatus = "Đã khôi phục cài đặt mặc định";
        OnConfigChanged?.Invoke(this, EventArgs.Empty);
    }
}
