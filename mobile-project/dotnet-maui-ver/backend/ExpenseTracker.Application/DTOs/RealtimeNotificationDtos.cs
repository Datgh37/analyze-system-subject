namespace ExpenseTracker.Application.DTOs;

public record BalanceUpdateNotificationDto(
    Guid WalletId,
    decimal NewBalance,
    decimal ChangeAmount,
    string Reason,
    DateTime Timestamp);

public record BudgetAlertNotificationDto(
    Guid BudgetId,
    string CategoryName,
    decimal SpentAmount,
    decimal LimitAmount,
    decimal PercentUsed,
    string Message,
    DateTime Timestamp);

public record InAppNotificationDto(
    Guid Id,
    string Title,
    string Message,
    string Type,
    string? PayloadJson,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt);

public record RegisterPushTokenRequest(
    string DeviceToken,
    string Platform,
    string? DeviceId);

public record SendTestNotificationRequest(
    string Title,
    string Message,
    string Type,
    string? TargetRoute,
    string? TargetId);
