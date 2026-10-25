namespace ExpenseTracker.Maui.Models.Autonomous;

// Action kind constants for Server-Driven Action Protocol
public static class ActionKindConstants
{
    public const string QuickCommit = "QUICK_COMMIT";
    public const string Navigate = "NAVIGATE";
    public const string SelectChoice = "SELECT_CHOICE";
    public const string Dismiss = "DISMISS";
}

// Action type constants matching backend engine
public static class AutonomousActionTypeConstants
{
    public const string QuickTransactionConfirm = "QUICK_TRANSACTION_CONFIRM";
    public const string ChoiceDisambiguation = "CHOICE_DISAMBIGUATION";
    public const string UnclassifiedNotice = "UNCLASSIFIED_NOTICE";
    public const string BudgetRebalanceSuggestion = "BUDGET_REBALANCE_SUGGESTION";
    public const string SubscriptionAlert = "SUBSCRIPTION_ALERT";
    public const string Committed = "COMMITTED";
}

public record ActionCommandModel(
    string Label,
    string ActionKind,
    string? Endpoint = null,
    string? Route = null,
    Dictionary<string, string>? Payload = null
);

public record ActionChoiceModel(
    string ChoiceId,
    string Label,
    double Probability,
    string FormattedProbability,
    Dictionary<string, string>? Payload = null
);

public record ActionDisplayModel(
    string Title,
    string Subtitle,
    string BadgeText,
    string BadgeColor,
    decimal Amount
);

public record AutonomousActionModel(
    string ActionId,
    string ActionType,
    double ConfidenceScore,
    ActionDisplayModel Display,
    ActionCommandModel? PrimaryAction = null,
    ActionCommandModel? SecondaryAction = null,
    List<ActionChoiceModel>? Choices = null,
    DateTime CreatedAt = default
);

public record AutonomousActionInboxResponseModel(
    int TotalCount,
    List<AutonomousActionModel> Actions
);

public record RawBankNotificationRequestModel(
    string PackageName,
    string Title,
    string Message,
    decimal? DetectedAmount = null,
    DateTime? NotificationTime = null
);

public record QuickCommitActionRequestModel(
    string ActionId,
    string? CategoryName = null,
    decimal? Amount = null,
    string? WalletName = null,
    string? Note = null
);
