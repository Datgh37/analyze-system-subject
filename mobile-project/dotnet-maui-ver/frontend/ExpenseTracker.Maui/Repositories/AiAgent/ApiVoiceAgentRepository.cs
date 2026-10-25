using ExpenseTracker.Maui.Models.AiAgent;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories.AiAgent;

public class ApiVoiceAgentRepository : IVoiceAgentRepository
{
    private readonly IVoiceAgentApi _api;

    public ApiVoiceAgentRepository(IVoiceAgentApi api)
    {
        _api = api;
    }

    public async Task<VoiceIntentResultModel> ParseIntentAsync(VoiceIntentCommandRequest request)
    {
        return await _api.ParseIntentAsync(request);
    }
}
