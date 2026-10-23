using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseTracker.WebApi.Hubs;

[Authorize]
public class FinancialHub : Hub
{
    private readonly ILogger<FinancialHub> _logger;

    public FinancialHub(ILogger<FinancialHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            var userGroup = $"user_{userId.Value}";
            await Groups.AddToGroupAsync(Context.ConnectionId, userGroup);
            _logger.LogInformation("SignalR: Client {ConnectionId} connected and joined group {Group}", Context.ConnectionId, userGroup);
        }
        else
        {
            _logger.LogWarning("SignalR: Client {ConnectionId} connected without valid user claim", Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            var userGroup = $"user_{userId.Value}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userGroup);
            _logger.LogInformation("SignalR: Client {ConnectionId} left group {Group}", Context.ConnectionId, userGroup);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task PingAsync()
    {
        await Clients.Caller.SendAsync("ReceivePong", new
        {
            Timestamp = DateTime.UtcNow,
            Message = "FinancialHub is active and responsive"
        });
    }

    private Guid? GetUserId()
    {
        var subClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? Context.User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(subClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}
