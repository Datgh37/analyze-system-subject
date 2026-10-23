using CommunityToolkit.Mvvm.Messaging.Messages;
using ExpenseTracker.Maui.Models.Notifications;

namespace ExpenseTracker.Maui.Services.Realtime.Messages;

public class RealtimeConnectionChangedMessage : ValueChangedMessage<RealtimeConnectionInfo>
{
    public RealtimeConnectionChangedMessage(RealtimeConnectionInfo value) : base(value) { }
}

public record RealtimeConnectionInfo(
    bool IsConnected,
    string StatusText,
    string ColorHex);

public class WalletBalanceUpdatedMessage : ValueChangedMessage<WalletBalanceUpdateInfo>
{
    public WalletBalanceUpdatedMessage(WalletBalanceUpdateInfo value) : base(value) { }
}

public record WalletBalanceUpdateInfo(
    Guid WalletId,
    decimal NewBalance,
    decimal ChangeAmount,
    string Reason,
    DateTime Timestamp);

public class BudgetAlertReceivedMessage : ValueChangedMessage<BudgetAlertInfo>
{
    public BudgetAlertReceivedMessage(BudgetAlertInfo value) : base(value) { }
}

public record BudgetAlertInfo(
    Guid BudgetId,
    string CategoryName,
    decimal SpentAmount,
    decimal LimitAmount,
    decimal PercentUsed,
    string Message,
    DateTime Timestamp);

public class InAppNotificationReceivedMessage : ValueChangedMessage<InAppNotificationModel>
{
    public InAppNotificationReceivedMessage(InAppNotificationModel value) : base(value) { }
}
