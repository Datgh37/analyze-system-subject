using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Nudges;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing Richard Thaler Behavioral Nudges, XAI Rationales and Dynamic Rebalancing (UC-09.01 -> UC-09.04)
[Authorize]
[Route("api/nudges")]
public class NudgesController : BaseApiController
{
    private readonly INudgeEngineService _nudgeService;

    public NudgesController(INudgeEngineService nudgeService)
    {
        _nudgeService = nudgeService;
    }

    // UC-09.01: Get active behavioral nudges
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<FinancialNudgeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveNudges(CancellationToken ct = default)
    {
        var result = await _nudgeService.GetActiveNudgesAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    // UC-09.02: Explainable AI (XAI) transparent rationale
    [HttpGet("{id:guid}/rationale")]
    [ProducesResponseType(typeof(XaiRationaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNudgeRationale(Guid id, CancellationToken ct = default)
    {
        var result = await _nudgeService.GetNudgeRationaleAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    // UC-09.03: Propose Dynamic Budget Rebalance
    [HttpGet("rebalance-proposal")]
    [ProducesResponseType(typeof(DynamicRebalanceProposalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRebalanceProposal([FromQuery] Guid? deficitCategoryId = null, CancellationToken ct = default)
    {
        var result = await _nudgeService.GenerateRebalanceProposalAsync(CurrentUserId, deficitCategoryId, ct);
        return HandleResult(result);
    }

    // UC-09.04: Human-in-the-loop approval of nudge and recommendation
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveNudge(Guid id, CancellationToken ct = default)
    {
        var result = await _nudgeService.ApproveNudgeAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    // UC-09.04: User dismisses recommendation
    [HttpPost("{id:guid}/dismiss")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DismissNudge(Guid id, CancellationToken ct = default)
    {
        var result = await _nudgeService.DismissNudgeAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }
}
