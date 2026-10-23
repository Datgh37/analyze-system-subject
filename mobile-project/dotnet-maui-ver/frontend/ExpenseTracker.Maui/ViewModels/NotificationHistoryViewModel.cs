using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Models.Notifications;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Realtime.Messages;

namespace ExpenseTracker.Maui.ViewModels;

public partial class NotificationHistoryViewModel : BaseViewModel
{
    private readonly INotificationRepository _notificationRepository;

    [ObservableProperty]
    private ObservableCollection<InAppNotificationModel> _notifications = new();

    [ObservableProperty]
    private int _unreadCount;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _filterMode = "All"; // "All" or "Unread"

    [ObservableProperty]
    private string? _statusMessage;

    public NotificationHistoryViewModel(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
        Title = "Lịch Sử Thông Báo";

        // Listen for realtime push notifications delivered over SignalR
        WeakReferenceMessenger.Default.Register<InAppNotificationReceivedMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Notifications.Insert(0, m.Value);
                if (!m.Value.IsRead)
                {
                    UnreadCount++;
                }
                StatusMessage = $"Vừa nhận thông báo mới: {m.Value.Title}";
            });
        });

        // Listen for budget alert events
        WeakReferenceMessenger.Default.Register<BudgetAlertReceivedMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var newNotif = new InAppNotificationModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Cảnh báo ngân sách thời gian thực",
                    Message = m.Value.Message,
                    Type = "BudgetAlert",
                    PayloadJson = "{\"route\":\"///BudgetsPage\"}",
                    IsRead = false,
                    CreatedAt = m.Value.Timestamp
                };
                Notifications.Insert(0, newNotif);
                UnreadCount++;
            });
        });
    }

    [RelayCommand]
    public async Task LoadNotificationsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            bool? unreadOnly = FilterMode == "Unread" ? true : null;
            var list = await _notificationRepository.GetNotificationsAsync(unreadOnly);

            Notifications.Clear();
            foreach (var item in list)
            {
                Notifications.Add(item);
            }

            UnreadCount = Notifications.Count(n => !n.IsRead);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải thông báo: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task MarkAsReadAsync(InAppNotificationModel? notification)
    {
        if (notification == null || notification.IsRead) return;

        try
        {
            var success = await _notificationRepository.MarkAsReadAsync(notification.Id);
            if (success)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                UnreadCount = Math.Max(0, UnreadCount - 1);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi cập nhật trạng thái: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task MarkAllAsReadAsync()
    {
        if (IsBusy || UnreadCount == 0) return;

        try
        {
            IsBusy = true;
            var success = await _notificationRepository.MarkAllAsReadAsync();
            if (success)
            {
                foreach (var item in Notifications)
                {
                    item.IsRead = true;
                    item.ReadAt = DateTime.UtcNow;
                }
                UnreadCount = 0;
                StatusMessage = "Đã đánh dấu tất cả thông báo là đã đọc.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi đánh dấu đã đọc: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task OpenNotificationDetailAsync(InAppNotificationModel? notification)
    {
        if (notification == null) return;

        // Auto mark as read on tap
        if (!notification.IsRead)
        {
            await MarkAsReadAsync(notification);
        }

        // Deep linking navigation based on payload JSON
        if (!string.IsNullOrEmpty(notification.PayloadJson) && Shell.Current != null)
        {
            try
            {
                using var doc = JsonDocument.Parse(notification.PayloadJson);
                var root = doc.RootElement;

                string? route = null;
                if (root.TryGetProperty("route", out var routeProp))
                {
                    route = routeProp.GetString();
                }

                if (!string.IsNullOrEmpty(route))
                {
                    await Shell.Current.GoToAsync(route);
                    return;
                }
            }
            catch
            {
                // Fallback to default route based on type
            }
        }

        // Fallback default routing based on type
        if (Shell.Current != null)
        {
            var fallbackRoute = notification.Type switch
            {
                "BudgetAlert" => "///BudgetsPage",
                "BalanceUpdate" => "///WalletsPage",
                "AiReceiptProcessed" => "///ExpensesPage",
                _ => null
            };

            if (!string.IsNullOrEmpty(fallbackRoute))
            {
                await Shell.Current.GoToAsync(fallbackRoute);
            }
        }
    }

    [RelayCommand]
    public async Task SendTestNotificationAsync(string type)
    {
        try
        {
            IsBusy = true;
            var (title, msg, route) = type switch
            {
                "budget" => ("⚠️ Cảnh báo ngân sách", "Bạn đã tiêu 90% hạn mức danh mục Ăn uống!", "///BudgetsPage"),
                "balance" => ("💰 Biến động số dư", "Ví Tiền mặt vừa nhận +2.000.000 VND tiền thưởng.", "///WalletsPage"),
                _ => ("🧾 AI OCR Thành công", "Hóa đơn Co.opmart đã được bóc tách xong.", "///ExpensesPage")
            };

            var result = await _notificationRepository.SendTestNotificationAsync(title, msg, type == "budget" ? "BudgetAlert" : (type == "balance" ? "BalanceUpdate" : "AiReceiptProcessed"), route);
            if (result != null)
            {
                Notifications.Insert(0, result);
                UnreadCount++;
                StatusMessage = "Đã gửi thông báo thử nghiệm thành công!";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi gửi test: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SetFilterAsync(string mode)
    {
        if (FilterMode == mode) return;
        FilterMode = mode;
        await LoadNotificationsAsync();
    }
}
