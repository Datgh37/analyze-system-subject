namespace ExpenseTracker.Application.DTOs.AiAgent;

// Request to parse a voice transcript into an executable action or navigation intent
public record VoiceIntentCommandRequest(
    string Transcript,
    Guid? CurrentWalletId = null
);

// Structured result returned by Voice Intent Router
public record VoiceIntentResultDto(
    string DetectedIntent,              // "AddTransaction", "NavigateTo", "QueryStatus"
    string ActionType,                  // "Expense", "Income", "Navigation", "Query"
    string? TargetRoute,                // e.g. "CreateCategoryPage", "AddTransactionPage"
    Dictionary<string, string> PrefillData, // Key-value pairs for prefilling UI form
    decimal? ExtractedAmount,           // e.g. 50000m
    string? SuggestedCategory,          // e.g. "Ăn uống", "Đi chợ"
    Guid? SuggestedWalletId,
    string ConfirmationMessage,         // User-friendly feedback message
    double Confidence                   // Confidence score (0.0 - 1.0)
);

// Constants for standard intents
public static class VoiceIntents
{
    public const string AddTransaction = "AddTransaction";
    public const string NavigateTo = "NavigateTo";
    public const string QueryStatus = "QueryStatus";
}
