using ExpenseTracker.Maui.Models.Notifications;
using ExpenseTracker.Maui.Services.Api;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Maui.Repositories;

public class ApiNotificationRepository : INotificationRepository
{
    private readonly INotificationsApi _notificationsApi;
    private readonly ILogger<ApiNotificationRepository> _logger;

    public ApiNotificationRepository(
        INotificationsApi notificationsApi,
        ILogger<ApiNotificationRepository> logger)
    {
        _notificationsApi = notificationsApi;
        _logger = logger;
    }

    public async Task<List<InAppNotificationModel>> GetNotificationsAsync(bool? unreadOnly = null, int take = 50)
    {
        try
        {
            return await _notificationsApi.GetNotificationsAsync(unreadOnly, take);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách thông báo từ API");
            throw;
        }
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        try
        {
            await _notificationsApi.MarkAsReadAsync(notificationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đánh dấu đã đọc thông báo {Id}", notificationId);
            return false;
        }
    }

    public async Task<bool> MarkAllAsReadAsync()
    {
        try
        {
            await _notificationsApi.MarkAllAsReadAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đánh dấu tất cả thông báo đã đọc");
            return false;
        }
    }

    public async Task<bool> RegisterPushTokenAsync(string token, string platform)
    {
        try
        {
            var req = new RegisterPushTokenRequestModel(token, platform, DeviceInfo.Current.Idiom.ToString());
            await _notificationsApi.RegisterPushTokenAsync(req);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký push token với server");
            return false;
        }
    }

    public async Task<InAppNotificationModel?> SendTestNotificationAsync(string title, string message, string type, string? route = null, string? id = null)
    {
        try
        {
            var req = new SendTestNotificationRequestModel(title, message, type, route, id);
            return await _notificationsApi.SendTestNotificationAsync(req);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi thông báo thử nghiệm");
            return null;
        }
    }
}
