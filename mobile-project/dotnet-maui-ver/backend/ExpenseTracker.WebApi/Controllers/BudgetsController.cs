using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing multi-methodology budgeting, dynamic rebalancing, and Adaptive DSA (UC-05.01 - 05.06)
[Authorize]
[Route("api/budgets")]
public class BudgetsController : BaseApiController
{
    private readonly IBudgetService _budgetService;
    private readonly IAdaptiveDsaService _dsaService;

    public BudgetsController(
        IBudgetService budgetService,
        IAdaptiveDsaService dsaService)
    {
        _budgetService = budgetService;
        _dsaService = dsaService;
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveBudget(CancellationToken ct)
    {
        var result = await _budgetService.GetActiveBudgetAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BudgetSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBudgets(CancellationToken ct)
    {
        var result = await _budgetService.GetBudgetsAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBudgetById(Guid id, CancellationToken ct)
    {
        var result = await _budgetService.GetBudgetByIdAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest request, CancellationToken ct)
    {
        var result = await _budgetService.CreateBudgetAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetRequest request, CancellationToken ct)
    {
        var result = await _budgetService.UpdateBudgetAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/rebalance")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RebalanceBudget(Guid id, [FromBody] RebalanceBudgetRequest request, CancellationToken ct)
    {
        var result = await _budgetService.RebalanceBudgetAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseBudget(Guid id, CancellationToken ct)
    {
        var result = await _budgetService.CloseBudgetAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpGet("active/dsa")]
    [ProducesResponseType(typeof(DsaStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentDsaStatus(CancellationToken ct)
    {
        var result = await _dsaService.GetCurrentDsaStatusAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    [HttpGet("active/velocity")]
    [ProducesResponseType(typeof(VelocityStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpendingVelocity(CancellationToken ct)
    {
        var result = await _dsaService.GetSpendingVelocityAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    [HttpGet("active/progress")]
    [ProducesResponseType(typeof(BudgetProgressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBudgetProgressOverview(CancellationToken ct)
    {
        var result = await _dsaService.GetBudgetProgressOverviewAsync(CurrentUserId, ct);
        return HandleResult(result);
    }
}
