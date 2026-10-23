namespace ExpenseTracker.Application.DTOs.SmartUtilities;

// Request to generate VietQR payment payload and QR image URL (UC-03.10)
public record GenerateVietQrRequest(
    string BankBin,
    string AccountNumber,
    string? AccountName = null,
    decimal? Amount = null,
    string? Note = null
);

// Response containing QR image link and standard Napas 247 payload string
public record VietQrResultDto(
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
public record ParseVoiceExpenseRequest(
    string Transcript
);

// Parsed financial transaction fields from voice sentence
public record ParsedVoiceExpenseDto(
    decimal Amount,
    Guid? SuggestedWalletId,
    string? SuggestedWalletName,
    Guid? SuggestedCategoryId,
    string? SuggestedCategoryName,
    string Note,
    string RawTranscript,
    double Confidence
);
