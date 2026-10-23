using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Nudges;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing Friction-by-Design Cooling-Off Period for impulse buying control (UC-09.05)
[Authorize]
[Route("api/cooling-off")]
public class CoolingOffController : BaseApiController
{
    private readonly ICoolingOffService _coolingOffService;

    public CoolingOffController(ICoolingOffService coolingOffService)
    {
        _coolingOffService = coolingOffService;
    }

    // UC-09.05: Pre-flight check to evaluate if expense requires a 24h cooling-off period & working hours equivalent
    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(CoolingOffEvaluationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EvaluateExpense([FromBody] CoolingOffEvaluationRequestDto request, CancellationToken ct = default)
    {
        var result = await _coolingOffService.EvaluateExpenseForCoolingOffAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    // UC-09.05: Create pending cooling-off transaction (Locks 24h timer, does NOT deduct wallet)
    [HttpPost]
    [ProducesResponseType(typeof(CoolingOffTransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCoolingOffTransaction([FromBody] CreateCoolingOffRequestDto request, CancellationToken ct = default)
    {
        var result = await _coolingOffService.CreateCoolingOffTransactionAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    // UC-09.05: Get list of active cooling-off items for user
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<CoolingOffTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingCoolingOffItems(CancellationToken ct = default)
    {
        var result = await _coolingOffService.GetPendingCoolingOffTransactionsAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    // UC-09.05: Confirm purchase after the cooling-off period has expired (Deducts wallet funds)
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmPurchase(Guid id, CancellationToken ct = default)
    {
        var result = await _coolingOffService.ConfirmPurchaseAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    // UC-09.05: Cancel purchase to defeat impulse buying and save money
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelPurchase(Guid id, CancellationToken ct = default)
    {
        var result = await _coolingOffService.CancelPurchaseAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    // UC-09.05 A1: Emergency bypass with user justification
    [HttpPost("{id:guid}/bypass")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BypassCoolingOff(Guid id, [FromBody] BypassCoolingOffRequestDto request, CancellationToken ct = default)
    {
        var result = await _coolingOffService.BypassCoolingOffAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }
}
