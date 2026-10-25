namespace ExpenseTracker.Maui.Models.AiAgent;

public record VoiceIntentCommandRequest(
    string Transcript,
    Guid? CurrentWalletId = null
);

public record VoiceIntentResultModel(
    string DetectedIntent,
    string ActionType,
    string? TargetRoute,
    Dictionary<string, string> PrefillData,
    decimal? ExtractedAmount,
    string? SuggestedCategory,
    Guid? SuggestedWalletId,
    string ConfirmationMessage,
    double Confidence
);
