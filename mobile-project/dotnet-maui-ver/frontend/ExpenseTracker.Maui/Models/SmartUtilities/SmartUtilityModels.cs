namespace ExpenseTracker.Maui.Models.SmartUtilities;

// Subscription detected from recurring expense patterns (UC-03.09)
public record DetectedSubscriptionModel(
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
public record SubscriptionAnalysisResultModel(
    List<DetectedSubscriptionModel> Subscriptions,
    decimal TotalMonthlyDrain,
    decimal TotalAnnualDrain,
    string FinancialHealthAlert
);

// Request to generate VietQR payment payload and QR image URL (UC-03.10)
public record GenerateVietQrModel(
    string BankBin,
    string AccountNumber,
    string? AccountName = null,
    decimal? Amount = null,
    string? Note = null
);

// Response containing QR image link and standard Napas 247 payload string
public record VietQrResultModel(
    string QrImageUrl,
    string PayloadString,
    string BankBin,
    string BankName,
    string AccountNumber,
    string? AccountName,
    decimal? Amount,
    string? Note
);

// Request to parse natural language voice sentence (UC-03.11)
public record ParseVoiceExpenseModel(
    string Transcript
);

// Parsed financial transaction fields from voice sentence
public record ParsedVoiceExpenseModel(
    decimal Amount,
    Guid? SuggestedWalletId,
    string? SuggestedWalletName,
    Guid? SuggestedCategoryId,
    string? SuggestedCategoryName,
    string Note,
    string RawTranscript,
    double Confidence
);
