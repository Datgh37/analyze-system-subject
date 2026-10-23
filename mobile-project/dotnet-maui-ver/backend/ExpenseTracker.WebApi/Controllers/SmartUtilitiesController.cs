using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints for smart financial utilities: Subscription Trap Detector, VietQR Napas 247, and Voice STT (UC-03.09 - 03.11)
[Authorize]
[Route("api/smart")]
public class SmartUtilitiesController : BaseApiController
{
    private readonly ISubscriptionDetectorService _subscriptionService;
    private readonly IVietQrService _vietQrService;
    private readonly IVoiceExpenseParserService _voiceService;

    public SmartUtilitiesController(
        ISubscriptionDetectorService subscriptionService,
        IVietQrService vietQrService,
        IVoiceExpenseParserService voiceService)
    {
        _subscriptionService = subscriptionService;
        _vietQrService = vietQrService;
        _voiceService = voiceService;
    }

    [HttpGet("subscriptions")]
    [ProducesResponseType(typeof(SubscriptionAnalysisResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> DetectSubscriptions(CancellationToken ct)
    {
        var result = await _subscriptionService.DetectSubscriptionsAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    [HttpPost("vietqr")]
    [ProducesResponseType(typeof(VietQrResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateVietQr([FromBody] GenerateVietQrRequest request, CancellationToken ct)
    {
        var result = await _vietQrService.GenerateVietQrAsync(request, ct);
        return HandleResult(result);
    }

    [HttpPost("voice-parse")]
    [ProducesResponseType(typeof(ParsedVoiceExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ParseVoiceExpense([FromBody] ParseVoiceExpenseRequest request, CancellationToken ct)
    {
        var result = await _voiceService.ParseVoiceExpenseAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }
}
