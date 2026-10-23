using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service interface for detecting recurring subscription traps (UC-03.09)
public interface ISubscriptionDetectorService
{
    Task<Result<SubscriptionAnalysisResultDto>> DetectSubscriptionsAsync(Guid userId, CancellationToken ct = default);
}

// Service interface for VietQR Napas 247 generation (UC-03.10)
public interface IVietQrService
{
    Task<Result<VietQrResultDto>> GenerateVietQrAsync(GenerateVietQrRequest request, CancellationToken ct = default);
}

// Service interface for Vietnamese speech-to-text financial transaction parsing (UC-03.11)
public interface IVoiceExpenseParserService
{
    Task<Result<ParsedVoiceExpenseDto>> ParseVoiceExpenseAsync(Guid userId, ParseVoiceExpenseRequest request, CancellationToken ct = default);
}
