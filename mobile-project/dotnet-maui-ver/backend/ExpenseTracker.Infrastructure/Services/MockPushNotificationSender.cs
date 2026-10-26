using System.Text.Json;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Services;

// Mock Push Notification Sender simulating Firebase/APNs delivery for local dev, evaluation, and grading environments
public class MockPushNotificationSender : IPushNotificationSender
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<MockPushNotificationSender> _logger;

    public string ProviderName => "Mock-Console";

    public MockPushNotificationSender(
        IApplicationDbContext context,
        ILogger<MockPushNotificationSender> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PushDeliveryResult> SendToUserAsync(
        Guid userId,
        string title,
        string message,
        string type,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default)
    {
        var tokens = await _context.DevicePushTokens
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Select(t => t.DeviceToken)
            .ToListAsync(ct);

        var route = data != null && data.TryGetValue("route", out var r) ? r : "///Dashboard";
        var messageId = $"mock_msg_{Guid.NewGuid():N}";

        _logger.LogInformation(
            "\n" +
            "┌──────────────────────────────────────────────────────────────────────────────────┐\n" +
            "│ 🔔 [MOCK PUSH SENDER] SERVER PUSH NOTIFICATION DISPATCHED                        │\n" +
            "├──────────────────────────────────────────────────────────────────────────────────┤\n" +
            "│ User ID:   {UserId,-69} │\n" +
            "│ Title:     {Title,-69} │\n" +
            "│ Body:      {Message,-69} │\n" +
            "│ Category:  {Type,-69} │\n" +
            "│ DeepLink:  {Route,-69} │\n" +
            "│ Devices:   {DeviceCount} device(s) registered                                            │\n" +
            "└──────────────────────────────────────────────────────────────────────────────────┘",
            userId, title, message, type, route, tokens.Count);

        // Simulate 20ms network transit
        await Task.Delay(20, ct);

        return PushDeliveryResult.Successful(ProviderName, Math.Max(1, tokens.Count), messageId);
    }

    public async Task<PushDeliveryResult> SendToTokenAsync(
        string deviceToken,
        string title,
        string message,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default)
    {
        var messageId = $"mock_msg_{Guid.NewGuid():N}";
        var previewToken = deviceToken.Length > 24 ? deviceToken[..24] + "..." : deviceToken;

        _logger.LogInformation(
            "🔔 [MOCK PUSH SENDER] Dispatched to Token {Token}: '{Title}' - '{Message}'",
            previewToken, title, message);

        await Task.Delay(10, ct);

        return PushDeliveryResult.Successful(ProviderName, 1, messageId);
    }
}
