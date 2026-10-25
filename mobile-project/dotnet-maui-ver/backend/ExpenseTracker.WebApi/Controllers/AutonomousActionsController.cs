using ExpenseTracker.Application.DTOs.Autonomous;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing Server-Driven Action Protocol & Jev Probabilistic Decision Gating (Chặng 10.5 / PLAN-47)
[Authorize]
[Route("api/actions")]
public class AutonomousActionsController : BaseApiController
{
    private readonly IAutonomousActionEngineService _actionEngine;

    public AutonomousActionsController(IAutonomousActionEngineService actionEngine)
    {
        _actionEngine = actionEngine;
    }

    // Retrieve pending actions in user's action inbox
    [HttpGet("inbox")]
    [ProducesResponseType(typeof(AutonomousActionInboxResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInbox(CancellationToken ct = default)
    {
        var result = await _actionEngine.GetPendingActionsAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    // Process raw bank/e-wallet notification captured by Android Native Service
    [HttpPost("from-notification")]
    [ProducesResponseType(typeof(AutonomousActionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessNotification([FromBody] RawBankNotificationRequest request, CancellationToken ct = default)
    {
        var result = await _actionEngine.ProcessRawBankNotificationAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    // 1-tap quick action commit directly into ledger
    [HttpPost("quick-commit")]
    [ProducesResponseType(typeof(AutonomousActionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> QuickCommit([FromBody] QuickCommitActionRequest request, CancellationToken ct = default)
    {
        var result = await _actionEngine.CommitQuickActionAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    // Dismiss an action from inbox
    [HttpPost("{actionId}/dismiss")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DismissAction(string actionId, CancellationToken ct = default)
    {
        var result = await _actionEngine.DismissActionAsync(CurrentUserId, actionId, ct);
        return HandleResult(result);
    }
}
