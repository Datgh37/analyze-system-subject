using ExpenseTracker.Application.DTOs.AiAgent;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service to classify and route voice transcripts into structured app actions or navigation intents
public interface IVoiceIntentRouterService
{
    Task<Result<VoiceIntentResultDto>> RouteVoiceCommandAsync(
        Guid userId,
        VoiceIntentCommandRequest request,
        CancellationToken ct = default);
}
