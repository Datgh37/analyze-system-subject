using System.Text.Json;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _context;
    private readonly IFinancialRealtimeNotifier _realtimeNotifier;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IApplicationDbContext context,
        IFinancialRealtimeNotifier realtimeNotifier,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _realtimeNotifier = realtimeNotifier;
        _logger = logger;
    }

    public async Task RegisterPushTokenAsync(Guid userId, RegisterPushTokenRequest request, CancellationToken cancellationToken = default)
    {
        var existingToken = await _context.DevicePushTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.DeviceToken == request.DeviceToken, cancellationToken);

        if (existingToken != null)
        {
            existingToken.RecordActivity();
        }
        else
        {
            var newToken = new DevicePushToken(
                userId,
                request.DeviceToken,
                request.Platform ?? "Android",
                request.DeviceId);

            _context.DevicePushTokens.Add(newToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Registered push token for user {UserId} on platform {Platform}", userId, request.Platform);
    }

    public async Task<List<InAppNotificationDto>> GetNotificationsAsync(Guid userId, bool? unreadOnly = null, int take = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.InAppNotifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (unreadOnly == true)
        {
            query = query.Where(n => !n.IsRead);
        }

        var list = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .Select(n => new InAppNotificationDto(
                n.Id,
                n.Title,
                n.Message,
                n.Type,
                n.PayloadJson,
                n.IsRead,
                n.ReadAt,
                n.CreatedAt))
            .ToListAsync(cancellationToken);

        return list;
    }

    public async Task<bool> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _context.InAppNotifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);

        if (notification == null) return false;

        notification.MarkAsRead();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _context.InAppNotifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<InAppNotificationDto> CreateAndSendNotificationAsync(
        Guid userId,
        string title,
        string message,
        string type,
        object? payload = null,
        CancellationToken cancellationToken = default)
    {
        string? payloadJson = payload != null ? JsonSerializer.Serialize(payload) : null;

        var entity = new InAppNotification(userId, title, message, type, payloadJson);
        _context.InAppNotifications.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new InAppNotificationDto(
            entity.Id,
            entity.Title,
            entity.Message,
            entity.Type,
            entity.PayloadJson,
            entity.IsRead,
            entity.ReadAt,
            entity.CreatedAt);

        // Broadcast realtime over SignalR
        try
        {
            await _realtimeNotifier.NotifyInAppAsync(userId, dto, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast realtime in-app notification to user {UserId}", userId);
        }

        return dto;
    }
}
