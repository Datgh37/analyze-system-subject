namespace ExpenseTracker.Application.DTOs.SmartUtilities;

// Subscription detected from recurring expense patterns (UC-03.09)
public record DetectedSubscriptionDto(
    string ServiceName,
    Guid CategoryId,
    string CategoryName,
    decimal RecurringAmount,
    decimal EstimatedAnnualCost,
    int OccurrencesCount,
    DateTime LastChargedDate,
    string AdviceMessage
);

// Summary result of subscription trap audit
public record SubscriptionAnalysisResultDto(
    List<DetectedSubscriptionDto> Subscriptions,
    decimal TotalMonthlyDrain,
    decimal TotalAnnualDrain,
    string FinancialHealthAlert
);
