using ExpenseTracker.Maui.Models.Notifications;

namespace ExpenseTracker.Maui.Repositories;

public class MockNotificationRepository : INotificationRepository
{
    private readonly List<InAppNotificationModel> _notifications = new();

    public MockNotificationRepository(IEnumerable<InAppNotificationModel>? initial = null)
    {
        var initialList = initial?.ToList();
        if (initialList != null && initialList.Count > 0)
        {
            _notifications.AddRange(initialList);
        }
        else
        {
            // Seed mock notifications
            _notifications.AddRange(new[]
            {
                new InAppNotificationModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Cảnh báo hạn mức ngân sách",
                    Message = "Bạn đã sử dụng 85% hạn mức ngân sách danh mục 'Ăn uống' trong tháng này.",
                    Type = "BudgetAlert",
                    PayloadJson = "{\"route\":\"///BudgetsPage\"}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new InAppNotificationModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Biến động số dư ví",
                    Message = "Ví 'Tiền mặt' vừa được cộng +5.000.000 VND từ Thu nhập lương.",
                    Type = "BalanceUpdate",
                    PayloadJson = "{\"route\":\"///WalletsPage\"}",
                    IsRead = true,
                    ReadAt = DateTime.UtcNow.AddHours(-3),
                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                },
                new InAppNotificationModel
                {
                    Id = Guid.NewGuid(),
                    Title = "AI OCR Hoàn tất",
                    Message = "Hóa đơn siêu thị Co.opmart đã được bóc tách thành công 5 mặt hàng.",
                    Type = "AiReceiptProcessed",
                    PayloadJson = "{\"route\":\"///ExpensesPage\"}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                }
            });
        }
    }

    public Task<List<InAppNotificationModel>> GetNotificationsAsync(bool? unreadOnly = null, int take = 50)
    {
        var query = _notifications.AsEnumerable();
        if (unreadOnly == true)
        {
            query = query.Where(n => !n.IsRead);
        }

        var result = query
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        var item = _notifications.FirstOrDefault(n => n.Id == notificationId);
        if (item != null)
        {
            item.IsRead = true;
            item.ReadAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> MarkAllAsReadAsync()
    {
        foreach (var item in _notifications.Where(n => !n.IsRead))
        {
            item.IsRead = true;
            item.ReadAt = DateTime.UtcNow;
        }
        return Task.FromResult(true);
    }

    public Task<bool> RegisterPushTokenAsync(string token, string platform)
    {
        return Task.FromResult(true);
    }

    public Task<InAppNotificationModel?> SendTestNotificationAsync(string title, string message, string type, string? route = null, string? id = null)
    {
        var item = new InAppNotificationModel
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Type = type,
            PayloadJson = route != null ? $"{{\"route\":\"{route}\",\"id\":\"{id}\"}}" : null,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
        _notifications.Insert(0, item);
        return Task.FromResult<InAppNotificationModel?>(item);
    }
}
