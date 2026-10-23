using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface IFinancialRealtimeNotifier
{
    Task NotifyBalanceUpdateAsync(Guid userId, BalanceUpdateNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyBudgetAlertAsync(Guid userId, BudgetAlertNotificationDto dto, CancellationToken cancellationToken = default);
    Task NotifyInAppAsync(Guid userId, InAppNotificationDto dto, CancellationToken cancellationToken = default);
}
