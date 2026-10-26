using System.Security.Claims;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseTracker.WebApi.Hubs;

[Authorize]
public class FinancialHub : Hub
{
    private readonly IWalletService _walletService;
    private readonly ILogger<FinancialHub> _logger;

    public FinancialHub(
        IWalletService walletService,
        ILogger<FinancialHub> logger)
    {
        _walletService = walletService;
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

    // Bi-directional full duplex client invocation: Request real-time balance for a given wallet
    public async Task RequestLiveBalanceAsync(Guid walletId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            _logger.LogWarning("SignalR: RequestLiveBalanceAsync rejected for unauthenticated connection {ConnectionId}", Context.ConnectionId);
            return;
        }

        var walletResult = await _walletService.GetWalletByIdAsync(userId.Value, walletId);
        if (walletResult.IsSuccess && walletResult.Value != null)
        {
            var dto = new BalanceUpdateNotificationDto(
                WalletId: walletResult.Value.Id,
                NewBalance: walletResult.Value.Balance,
                ChangeAmount: 0m,
                Reason: "Live balance inquiry requested by client",
                Timestamp: DateTime.UtcNow);

            await Clients.Caller.SendAsync("ReceiveBalanceUpdate", dto);
            _logger.LogInformation("SignalR: Dispatched live balance {Balance} for wallet {WalletId} to caller {ConnectionId}",
                walletResult.Value.Balance, walletId, Context.ConnectionId);
        }
        else
        {
            _logger.LogWarning("SignalR: Wallet {WalletId} not found or unauthorized for user {UserId}", walletId, userId.Value);
        }
    }

    // Bi-directional client invocation: Ask all active devices of this user to refresh wallets
    public async Task BroadcastWalletRefreshAsync()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return;
        }

        var userGroup = $"user_{userId.Value}";
        await Clients.Group(userGroup).SendAsync("ReceiveWalletRefreshNeeded");
        _logger.LogInformation("SignalR: Broadcasted ReceiveWalletRefreshNeeded to group {Group}", userGroup);
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
