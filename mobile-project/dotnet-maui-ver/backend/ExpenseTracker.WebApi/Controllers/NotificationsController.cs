using System.Security.Claims;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost("push-token")]
    public async Task<IActionResult> RegisterPushToken([FromBody] RegisterPushTokenRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        await _notificationService.RegisterPushTokenAsync(userId.Value, request);
        return Ok(new { message = "Push token registered successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool? unreadOnly, [FromQuery] int take = 50)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var list = await _notificationService.GetNotificationsAsync(userId.Value, unreadOnly, take);
        return Ok(list);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var success = await _notificationService.MarkAsReadAsync(userId.Value, id);
        if (!success) return NotFound(new { message = "Notification not found" });

        return Ok(new { message = "Notification marked as read" });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId.Value);
        return Ok(new { message = "All notifications marked as read" });
    }

    [HttpPost("test-push")]
    public async Task<IActionResult> SendTestNotification([FromBody] SendTestNotificationRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        object? payload = null;
        if (!string.IsNullOrEmpty(request.TargetRoute))
        {
            payload = new
            {
                route = request.TargetRoute,
                id = request.TargetId,
                timestamp = DateTime.UtcNow
            };
        }

        var notification = await _notificationService.CreateAndSendNotificationAsync(
            userId.Value,
            request.Title,
            request.Message,
            request.Type ?? "System",
            payload);

        return Ok(notification);
    }

    private Guid? GetCurrentUserId()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(subClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}
