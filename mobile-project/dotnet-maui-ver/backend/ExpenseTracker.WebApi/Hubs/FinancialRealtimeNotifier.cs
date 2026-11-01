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

    public async Task NotifyTransactionCreatedAsync(Guid userId, TransactionCreatedNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveTransactionCreated to group {Group}: {Amount}đ ({CategoryName})",
            groupName, dto.Amount, dto.CategoryName);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveTransactionCreated", dto, cancellationToken);
    }

    public async Task NotifyOcrProgressAsync(Guid userId, OcrProgressNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveOcrProgress to group {Group}: Job {JobId} - {StepText} ({Percent}%)",
            groupName, dto.JobId, dto.StepText, dto.Percent);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveOcrProgress", dto, cancellationToken);
    }

    public async Task NotifyNudgeAlertAsync(Guid userId, NudgeAlertNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveNudgeAlert to group {Group}: {Title} - {Message}",
            groupName, dto.Title, dto.Message);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNudgeAlert", dto, cancellationToken);
    }

    public async Task NotifyTransactionArchivedAsync(Guid userId, TransactionArchivedNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveTransactionArchived to group {Group}: Transaction {TransactionId}, Wallet {WalletId}, Reverted {Amount}đ",
            groupName, dto.TransactionId, dto.WalletId, dto.RevertedAmount);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveTransactionArchived", dto, cancellationToken);
    }

    public async Task NotifyEntityArchivedAsync(Guid userId, EntityArchivedNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var groupName = $"user_{userId}";
        _logger.LogInformation("Broadcasting ReceiveEntityArchived to group {Group}: {EntityType} {EntityId} ({Name})",
            groupName, dto.EntityType, dto.EntityId, dto.Name);

        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveEntityArchived", dto, cancellationToken);
    }
}

