using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface IFinancialRealtimeNotifier
{
    Task NotifyBalanceUpdateAsync(Guid userId, BalanceUpdateNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyBudgetAlertAsync(Guid userId, BudgetAlertNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyInAppAsync(Guid userId, InAppNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyTransactionCreatedAsync(Guid userId, TransactionCreatedNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyOcrProgressAsync(Guid userId, OcrProgressNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyNudgeAlertAsync(Guid userId, NudgeAlertNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyTransactionArchivedAsync(Guid userId, TransactionArchivedNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyEntityArchivedAsync(Guid userId, EntityArchivedNotificationDto dto, CancellationToken cancellationToken = default);
}

