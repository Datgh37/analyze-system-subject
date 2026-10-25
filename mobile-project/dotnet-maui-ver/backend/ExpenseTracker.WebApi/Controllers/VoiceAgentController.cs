using ExpenseTracker.Application.DTOs.AiAgent;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing Voice-Driven Intent Agent and AppShell action routing (UC-03.11 Enhancement)
[Authorize]
[Route("api/voice-agent")]
public class VoiceAgentController : BaseApiController
{
    private readonly IVoiceIntentRouterService _voiceRouter;

    public VoiceAgentController(IVoiceIntentRouterService voiceRouter)
    {
        _voiceRouter = voiceRouter;
    }

    [HttpPost("parse-intent")]
    [ProducesResponseType(typeof(VoiceIntentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ParseIntent([FromBody] VoiceIntentCommandRequest request, CancellationToken ct)
    {
        var result = await _voiceRouter.RouteVoiceCommandAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }
}
