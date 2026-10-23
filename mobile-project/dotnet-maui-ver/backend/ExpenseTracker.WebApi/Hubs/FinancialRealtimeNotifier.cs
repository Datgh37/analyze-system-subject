using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseTracker.WebApi.Hubs;

public class FinancialRealtimeNotifier : IFinancialRealtimeNotifier
{
    private readonly IHubContext<FinancialHub> _hubContext;
    private readonly ILogger<FinancialRealtimeNotifier> _logger;

    public FinancialRealtimeNotifier(
        IHubContext<FinancialHub> hubContext,
        ILogger<FinancialRealtimeNotifier> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyBalanceUpdateAsync(Guid userId, BalanceUpdateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveBalanceUpdate to group {Group}: Wallet {WalletId}, Balance {NewBalance}",
            groupName, dto.WalletId, dto.NewBalance);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveBalanceUpdate", dto, cancellationToken);
    }

    public async Task NotifyBudgetAlertAsync(Guid userId, BudgetAlertNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveBudgetAlert to group {Group}: Category {Category}, {PercentUsed}%",
            groupName, dto.CategoryName, dto.PercentUsed);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveBudgetAlert", dto, cancellationToken);
    }

    public async Task NotifyInAppAsync(Guid userId, InAppNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveNotification to group {Group}: {Title}", groupName, dto.Title);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", dto, cancellationToken);
    }
}
