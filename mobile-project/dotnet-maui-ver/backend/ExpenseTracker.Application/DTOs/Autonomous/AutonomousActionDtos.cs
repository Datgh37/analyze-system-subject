namespace ExpenseTracker.Application.DTOs.Autonomous;

// Data contracts for Server-Driven Action Protocol (Chặng 10.5 / PLAN-47)
// Built with Jev-inspired probabilistic decision gating (Confidence Thresholds)

public record AutonomousActionDto(
    string ActionId,
    string ActionType,                  // Constants from AutonomousActionTypes
    double ConfidenceScore,              // Probability score (0.0 - 1.0)
    ActionDisplayDto Display,           // Non-intrusive UI presentation details
    ActionCommandDto? PrimaryAction,    // 1-Tap quick save or primary action
    ActionCommandDto? SecondaryAction,  // Edit, navigate or dismiss action
    List<ActionChoiceDto>? Choices,     // Populated when in Jev Choice state (0.50 <= Confidence < 0.85)
    DateTime CreatedAt
);

public record ActionDisplayDto(
    string Title,                       // e.g. "Khoản chi Shopee 185.000 đ"
    string Subtitle,                    // e.g. "Tài khoản ACB • 23:15"
    string BadgeText,                   // e.g. "Mua sắm", "Ăn uống", "Bẫy thuê bao"
    string BadgeColor,                  // Hex color e.g. "#F59E0B"
    decimal? Amount = null              // Monetary value if applicable
);

public record ActionCommandDto(
    string Label,                       // e.g. "Lưu nhanh", "Đổi", "Bỏ qua"
    string ActionKind,                  // "QUICK_COMMIT", "NAVIGATE", "DISMISS", "SELECT_CHOICE"
    string? Endpoint = null,            // e.g. "/api/actions/quick-commit"
    string? Route = null,               // e.g. "AddTransactionPage?amount=185000&source=ACB"
    Dictionary<string, string>? Payload = null
);

public record ActionChoiceDto(
    string ChoiceId,
    string Label,                       // e.g. "Ăn uống"
    double Probability,                 // e.g. 0.65
    string FormattedProbability,        // e.g. "65%"
    Dictionary<string, string>? Payload = null
);

// Request payload when user confirms a 1-tap quick action
public record QuickCommitActionRequest(
    string ActionId,
    string? CategoryName = null,
    decimal? Amount = null,
    string? WalletName = null,
    string? Note = null
);

// Request payload when capturing an incoming bank/e-wallet notification
public record RawBankNotificationRequest(
    string PackageName,                 // e.g. "com.acb.mobile", "com.mservice.momotransfer"
    string Title,                       // Notification title e.g. "ACB ONE"
    string Message,                     // Notification body e.g. "TK 123xxx GD -185,000VND tai SHOPEE"
    decimal? DetectedAmount = null,
    DateTime? NotificationTime = null
);

// Response containing inbox of pending autonomous actions
public record AutonomousActionInboxResponse(
    int TotalCount,
    List<AutonomousActionDto> Actions
);

// Standard Constants for Autonomous Actions
public static class AutonomousActionTypes
{
    public const string QuickTransactionConfirm = "QUICK_TRANSACTION_CONFIRM";
    public const string ChoiceDisambiguation = "CHOICE_DISAMBIGUATION";
    public const string BudgetWarningNotice = "BUDGET_WARNING_NOTICE";
    public const string SubscriptionAlert = "SUBSCRIPTION_ALERT";
    public const string UnclassifiedNotice = "UNCLASSIFIED_NOTICE";
}

public static class ActionKinds
{
    public const string QuickCommit = "QUICK_COMMIT";
    public const string Navigate = "NAVIGATE";
    public const string Dismiss = "DISMISS";
    public const string SelectChoice = "SELECT_CHOICE";
}
