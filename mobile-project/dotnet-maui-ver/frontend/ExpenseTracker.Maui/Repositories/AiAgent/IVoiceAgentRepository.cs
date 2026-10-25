using ExpenseTracker.Maui.Models.AiAgent;

namespace ExpenseTracker.Maui.Repositories.AiAgent;

public interface IVoiceAgentRepository
{
    Task<VoiceIntentResultModel> ParseIntentAsync(VoiceIntentCommandRequest request);
}
