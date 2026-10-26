using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating NotificationService and Server Push Notification Adapter (FCM/Mock)
public class NotificationServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IFinancialRealtimeNotifier> _realtimeNotifierMock;
    private readonly Mock<IPushNotificationSender> _pushSenderMock;
    private readonly Mock<ILogger<NotificationService>> _loggerMock;
    private readonly NotificationService _notificationService;
    private readonly Guid _userId = Guid.NewGuid();

    public NotificationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _realtimeNotifierMock = new Mock<IFinancialRealtimeNotifier>();
        _pushSenderMock = new Mock<IPushNotificationSender>();
        _loggerMock = new Mock<ILogger<NotificationService>>();

        _notificationService = new NotificationService(
            _dbContext,
            _realtimeNotifierMock.Object,
            _loggerMock.Object,
            _pushSenderMock.Object);
    }

    [Fact]
    public async Task RegisterPushTokenAsync_WhenNewToken_ShouldPersistDevicePushToken()
    {
        // Arrange
        var request = new RegisterPushTokenRequest(
            DeviceToken: "fcm_token_sample_12345",
            Platform: "Android",
            DeviceId: "Pixel_8a_Emulator");

        // Act
        await _notificationService.RegisterPushTokenAsync(_userId, request);

        // Assert
        var saved = await _dbContext.DevicePushTokens.FirstOrDefaultAsync(t => t.UserId == _userId);
        saved.Should().NotBeNull();
        saved!.DeviceToken.Should().Be("fcm_token_sample_12345");
        saved.Platform.Should().Be("Android");
        saved.DeviceId.Should().Be("Pixel_8a_Emulator");
    }

    [Fact]
    public async Task RegisterPushTokenAsync_WhenTokenExists_ShouldUpdateActivityWithoutDuplicate()
    {
        // Arrange
        var request = new RegisterPushTokenRequest(
            DeviceToken: "fcm_token_sample_12345",
            Platform: "Android",
            DeviceId: "Pixel_8a_Emulator");

        await _notificationService.RegisterPushTokenAsync(_userId, request);

        // Act: Register again
        await _notificationService.RegisterPushTokenAsync(_userId, request);

        // Assert: Count should still be 1
        var count = await _dbContext.DevicePushTokens.CountAsync(t => t.UserId == _userId);
        count.Should().Be(1);
    }

    [Fact]
    public async Task CreateAndSendNotificationAsync_ShouldPersistAndInvokeBothRealtimeAndPushAdapters()
    {
        // Arrange
        var title = "Cảnh báo hạn mức";
        var message = "Bạn đã chi tiêu đạt 85% hạn mức danh mục Ăn uống.";
        var type = "BudgetAlert";
        var payload = new { route = "///BudgetsPage", categoryId = Guid.NewGuid() };

        // Act
        var result = await _notificationService.CreateAndSendNotificationAsync(_userId, title, message, type, payload);

        // Assert: Persisted to database
        result.Should().NotBeNull();
        result.Title.Should().Be(title);
        result.Message.Should().Be(message);
        result.IsRead.Should().BeFalse();

        var inDb = await _dbContext.InAppNotifications.FirstOrDefaultAsync(n => n.Id == result.Id);
        inDb.Should().NotBeNull();

        // Assert: Realtime SignalR invoked
        _realtimeNotifierMock.Verify(r => r.NotifyInAppAsync(
            _userId,
            It.Is<InAppNotificationDto>(dto => dto.Id == result.Id && dto.Title == title),
            It.IsAny<CancellationToken>()), Times.Once);

        // Assert: Server Push Notification Sender invoked
        _pushSenderMock.Verify(p => p.SendToUserAsync(
            _userId,
            title,
            message,
            type,
            It.Is<Dictionary<string, string>>(d => d != null && d.ContainsKey("route")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldUpdateNotificationStatus()
    {
        // Arrange
        var created = await _notificationService.CreateAndSendNotificationAsync(
            _userId, "Tiêu đề", "Nội dung", "Info");

        // Act
        var marked = await _notificationService.MarkAsReadAsync(_userId, created.Id);

        // Assert
        marked.Should().BeTrue();
        var inDb = await _dbContext.InAppNotifications.FindAsync(created.Id);
        inDb!.IsRead.Should().BeTrue();
        inDb.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task MockPushNotificationSender_ShouldSimulateDeliveryAndReturnSuccess()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<MockPushNotificationSender>>();
        var mockSender = new MockPushNotificationSender(_dbContext, loggerMock.Object);

        var token = new DevicePushToken(_userId, "test_device_token_xyz", "Android");
        _dbContext.DevicePushTokens.Add(token);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await mockSender.SendToUserAsync(
            _userId,
            "Cảnh báo số dư",
            "Số dư ví Tiền Mặt còn dưới 100.000 đ",
            "Warning",
            new Dictionary<string, string> { { "route", "///WalletsPage" } });

        // Assert
        result.Success.Should().BeTrue();
        result.Provider.Should().Be("Mock-Console");
        result.DeliveredCount.Should().Be(1);
    }
}
