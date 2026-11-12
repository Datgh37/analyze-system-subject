using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Models.Notifications;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Realtime.Messages;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Maui.UnitTests;

public class NotificationHistoryViewModelTests
{
    private readonly Mock<INotificationRepository> _mockRepo;
    private readonly NotificationHistoryViewModel _viewModel;

    public NotificationHistoryViewModelTests()
    {
        _mockRepo = new Mock<INotificationRepository>();
        _viewModel = new NotificationHistoryViewModel(_mockRepo.Object);
    }

    [Fact]
    public async Task LoadNotificationsAsync_WhenRepositoryReturnsItems_ShouldPopulateNotificationsAndUnreadCount()
    {
        // Arrange
        var notifications = new List<InAppNotificationModel>
        {
            new() { Id = Guid.NewGuid(), Title = "Cảnh báo 1", Message = "Vượt hạn mức", Type = "BudgetAlert", IsRead = false, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Title = "Số dư 2", Message = "Cộng tiền", Type = "BalanceUpdate", IsRead = true, CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
            new() { Id = Guid.NewGuid(), Title = "Hóa đơn 3", Message = "AI OCR xong", Type = "AiReceiptProcessed", IsRead = false, CreatedAt = DateTime.UtcNow.AddMinutes(-5) }
        };

        _mockRepo.Setup(r => r.GetNotificationsAsync(null, 50)).ReturnsAsync(notifications);

        // Act
        await _viewModel.LoadNotificationsAsync();

        // Assert
        _viewModel.Notifications.Should().HaveCount(3);
        _viewModel.UnreadCount.Should().Be(2);
        _viewModel.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task LoadNotificationsAsync_WhenRepositoryThrows_ShouldSetErrorMessage()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetNotificationsAsync(null, 50)).ThrowsAsync(new HttpRequestException("Network failure"));

        // Act
        await _viewModel.LoadNotificationsAsync();

        // Assert
        _viewModel.Notifications.Should().BeEmpty();
        _viewModel.ErrorMessage.Should().Contain("Network failure");
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldUpdateNotificationStatusAndDecrementUnreadCount()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var targetNotification = new InAppNotificationModel
        {
            Id = targetId,
            Title = "Thông báo chưa đọc",
            Message = "Nội dung",
            IsRead = false
        };

        _viewModel.Notifications.Add(targetNotification);
        _viewModel.UnreadCount = 1;

        _mockRepo.Setup(r => r.MarkAsReadAsync(targetId)).ReturnsAsync(true);

        // Act
        await _viewModel.MarkAsReadAsync(targetNotification);

        // Assert
        targetNotification.IsRead.Should().BeTrue();
        targetNotification.ReadAt.Should().NotBeNull();
        _viewModel.UnreadCount.Should().Be(0);
        _mockRepo.Verify(r => r.MarkAsReadAsync(targetId), Times.Once);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_ShouldMarkAllReadAndResetUnreadCount()
    {
        // Arrange
        var notif1 = new InAppNotificationModel { Id = Guid.NewGuid(), Title = "N1", IsRead = false };
        var notif2 = new InAppNotificationModel { Id = Guid.NewGuid(), Title = "N2", IsRead = false };
        _viewModel.Notifications.Add(notif1);
        _viewModel.Notifications.Add(notif2);
        _viewModel.UnreadCount = 2;

        _mockRepo.Setup(r => r.MarkAllAsReadAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.MarkAllAsReadAsync();

        // Assert
        notif1.IsRead.Should().BeTrue();
        notif2.IsRead.Should().BeTrue();
        _viewModel.UnreadCount.Should().Be(0);
        _viewModel.StatusMessage.Should().Contain("đã đọc");
        _mockRepo.Verify(r => r.MarkAllAsReadAsync(), Times.Once);
    }

    [Fact]
    public async Task SendNativeNotificationAsync_WhenTriggered_ShouldCallNativeService()
    {
        // Arrange
        var mockNativeService = new Mock<ExpenseTracker.Maui.Services.Notifications.INativeNotificationService>();
        var viewModel = new NotificationHistoryViewModel(_mockRepo.Object, mockNativeService.Object);

        // Act
        await viewModel.SendNativeNotificationAsync("dsa");

        // Assert
        mockNativeService.Verify(s => s.ShowNotificationAsync(
            It.Is<string>(t => t.Contains("Cảnh Báo") || t.Contains("DSA")),
            It.IsAny<string>(),
            ExpenseTracker.Maui.Services.Notifications.NotificationChannelIds.DsaAlerts,
            It.IsAny<int>(),
            It.Is<string>(r => r == "///BudgetsPage")), Times.Once);

        viewModel.StatusMessage.Should().Contain("Đã phát Native Notification");
    }

    [Fact]
    public async Task ScheduleDailyReminderAsync_WhenTriggered_ShouldCallNativeService()
    {
        // Arrange
        var mockNativeService = new Mock<ExpenseTracker.Maui.Services.Notifications.INativeNotificationService>();
        var viewModel = new NotificationHistoryViewModel(_mockRepo.Object, mockNativeService.Object);

        // Act
        await viewModel.ScheduleDailyReminderAsync();

        // Assert
        mockNativeService.Verify(s => s.ScheduleDailyReminderAsync(21, 0), Times.Once);
        viewModel.StatusMessage.Should().Contain("21:00");
    }

    [Fact]
    public async Task CancelDailyReminderAsync_WhenTriggered_ShouldCallNativeService()
    {
        // Arrange
        var mockNativeService = new Mock<ExpenseTracker.Maui.Services.Notifications.INativeNotificationService>();
        var viewModel = new NotificationHistoryViewModel(_mockRepo.Object, mockNativeService.Object);

        // Act
        await viewModel.CancelDailyReminderAsync();

        // Assert
        mockNativeService.Verify(s => s.CancelDailyReminderAsync(), Times.Once);
        viewModel.StatusMessage.Should().Contain("Đã hủy");
    }
}

