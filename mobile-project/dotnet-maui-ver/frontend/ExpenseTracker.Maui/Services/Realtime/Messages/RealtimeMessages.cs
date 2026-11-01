using CommunityToolkit.Mvvm.Messaging.Messages;
using ExpenseTracker.Maui.Models.Notifications;

namespace ExpenseTracker.Maui.Services.Realtime.Messages;

public class ServerModeChangedMessage : ValueChangedMessage<bool>
{
    public ServerModeChangedMessage(bool isLiveApiMode) : base(isLiveApiMode) { }
}

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

public class TransactionCreatedMessage : ValueChangedMessage<TransactionCreatedRealtimeInfo>
{
    public TransactionCreatedMessage(TransactionCreatedRealtimeInfo value) : base(value) { }
}

public record TransactionCreatedRealtimeInfo(
    Guid TransactionId,
    Guid WalletId,
    string WalletName,
    Guid CategoryId,
    string CategoryName,
    decimal Amount,
    int TransactionType,
    DateTime TransactionDate,
    string? Note,
    DateTime Timestamp);

public class OcrProgressMessage : ValueChangedMessage<OcrProgressRealtimeInfo>
{
    public OcrProgressMessage(OcrProgressRealtimeInfo value) : base(value) { }
}

public record OcrProgressRealtimeInfo(
    string JobId,
    string StepText,
    int Percent,
    DateTime Timestamp);

public class NudgeAlertReceivedMessage : ValueChangedMessage<NudgeAlertRealtimeInfo>
{
    public NudgeAlertReceivedMessage(NudgeAlertRealtimeInfo value) : base(value) { }
}

public record NudgeAlertRealtimeInfo(
    Guid NudgeId,
    string Title,
    string Message,
    string Severity,
    string? TargetRoute,
    DateTime Timestamp);

public class WalletRefreshNeededMessage : ValueChangedMessage<bool>
{
    public WalletRefreshNeededMessage(bool value = true) : base(value) { }
}

public class TransactionArchivedMessage : ValueChangedMessage<TransactionArchivedRealtimeInfo>
{
    public TransactionArchivedMessage(TransactionArchivedRealtimeInfo value) : base(value) { }
}

public record TransactionArchivedRealtimeInfo(
    Guid TransactionId,
    Guid WalletId,
    decimal RevertedAmount,
    decimal NewWalletBalance,
    int TransactionType,
    string Reason,
    DateTime Timestamp);


