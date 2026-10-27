namespace ExpenseTracker.Maui.Services.Config;

public interface IServerConfigService
{
    /// <summary>
    /// Chế độ hoạt động hiện tại: true = Gọi API Backend thật; false = Chạy Offline Mock Data 0ms
    /// </summary>
    bool IsLiveApiMode { get; set; }

    /// <summary>
    /// Địa chỉ máy chủ Backend (mặc định: http://10.0.2.2:5018 trên Android, http://localhost:5018 trên Windows hoặc URL Tunnel)
    /// </summary>
    string ServerBaseUrl { get; set; }

    /// <summary>
    /// Chuỗi mô tả trạng thái kết nối hiển thị trên giao diện
    /// </summary>
    string ConnectionStatus { get; }

    /// <summary>
    /// Sự kiện phát ra khi người dùng thay đổi chế độ hoặc URL máy chủ
    /// </summary>
    event EventHandler? OnConfigChanged;

    /// <summary>
    /// Gửi request kiểm tra sức khỏe (/api/health) tới máy chủ với Timeout ngắn (3 giây)
    /// </summary>
    Task<bool> PingServerAsync(string? url = null);

    /// <summary>
    /// Quy trình kiểm tra kết nối khi mở app: Thử kết nối -> Retry 3 lần x 15s nếu fail -> Fallback sang Offline Mock
    /// </summary>
    Task StartConnectionCheckWithRetryAsync(Action<string>? onToastCallback = null);

    /// <summary>
    /// Khôi phục cấu hình về giá trị mặc định của hệ thống
    /// </summary>
    void ResetToDefaults();
}
