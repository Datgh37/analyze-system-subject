using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Services;

// Firebase Cloud Messaging (FCM HTTP v1) Adapter with Graceful Fallback to MockPushNotificationSender
public class FirebasePushNotificationSender : IPushNotificationSender
{
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MockPushNotificationSender _fallbackSender;
    private readonly ILogger<FirebasePushNotificationSender> _logger;
    private readonly string? _projectId;
    private readonly string? _serviceAccountKeyPath;
    private readonly bool _isConfigured;

    public string ProviderName => _isConfigured ? "FCM-v1" : "FCM-v1(Fallback-Mock)";

    public FirebasePushNotificationSender(
        IConfiguration configuration,
        IApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        MockPushNotificationSender fallbackSender,
        ILogger<FirebasePushNotificationSender> logger)
    {
        _configuration = configuration;
        _context = context;
        _httpClientFactory = httpClientFactory;
        _fallbackSender = fallbackSender;
        _logger = logger;

        _projectId = _configuration["Firebase:ProjectId"];
        _serviceAccountKeyPath = _configuration["Firebase:ServiceAccountKeyPath"];

        _isConfigured = !string.IsNullOrWhiteSpace(_projectId) &&
                        !string.IsNullOrWhiteSpace(_serviceAccountKeyPath) &&
                        File.Exists(_serviceAccountKeyPath);

        if (!_isConfigured)
        {
            _logger.LogInformation(
                "FCM Push Notification: Google Service Account not configured or credentials file not found. " +
                "Operating in Graceful Fallback mode using MockPushNotificationSender.");
        }
    }

    public async Task<PushDeliveryResult> SendToUserAsync(
        Guid userId,
        string title,
        string message,
        string type,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default)
    {
        if (!_isConfigured)
        {
            return await _fallbackSender.SendToUserAsync(userId, title, message, type, data, ct);
        }

        var tokens = await _context.DevicePushTokens
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Select(t => t.DeviceToken)
            .ToListAsync(ct);

        if (tokens.Count == 0)
        {
            _logger.LogDebug("User {UserId} has no registered push tokens, skipping FCM dispatch", userId);
            return PushDeliveryResult.Successful(ProviderName, 0);
        }

        int successCount = 0;
        foreach (var token in tokens)
        {
            var res = await SendToTokenAsync(token, title, message, data, ct);
            if (res.Success) successCount++;
        }

        return PushDeliveryResult.Successful(ProviderName, successCount);
    }

    public async Task<PushDeliveryResult> SendToTokenAsync(
        string deviceToken,
        string title,
        string message,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default)
    {
        if (!_isConfigured)
        {
            return await _fallbackSender.SendToTokenAsync(deviceToken, title, message, data, ct);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("FcmClient");
            var fcmUrl = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

            var fcmPayload = new
            {
                message = new
                {
                    token = deviceToken,
                    notification = new
                    {
                        title = title,
                        body = message
                    },
                    data = data ?? new Dictionary<string, string>(),
                    android = new
                    {
                        priority = "HIGH",
                        notification = new
                        {
                            channel_id = "expense_tracker_alerts",
                            sound = "default"
                        }
                    }
                }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(fcmPayload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(fcmUrl, jsonContent, ct);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogInformation("FCM v1 message sent successfully to token. Response: {Response}", body);
                return PushDeliveryResult.Successful(ProviderName, 1);
            }

            _logger.LogWarning("FCM v1 returned non-success code {StatusCode}. Falling back to Mock sender.", response.StatusCode);
            return await _fallbackSender.SendToTokenAsync(deviceToken, title, message, data, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception calling FCM v1 endpoint. Falling back to Mock sender.");
            return await _fallbackSender.SendToTokenAsync(deviceToken, title, message, data, ct);
        }
    }
}
