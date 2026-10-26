using ExpenseTracker.Application.DTOs.RecurringIncomes;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing scheduled recurring income streams (UC-02.07)
[Authorize]
[Route("api/recurring-incomes")]
[Route("api/recurringincomes")]
public class RecurringIncomesController : BaseApiController
{
    private readonly IRecurringIncomeService _recurringIncomeService;

    public RecurringIncomesController(IRecurringIncomeService recurringIncomeService)
    {
        _recurringIncomeService = recurringIncomeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RecurringIncomeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecurringIncomes(CancellationToken ct)
    {
        var result = await _recurringIncomeService.GetRecurringIncomesAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecurringIncomeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecurringIncomeById(Guid id, CancellationToken ct)
    {
        var result = await _recurringIncomeService.GetRecurringIncomeByIdAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RecurringIncomeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRecurringIncome([FromBody] CreateRecurringIncomeRequest request, CancellationToken ct)
    {
        var result = await _recurringIncomeService.CreateRecurringIncomeAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RecurringIncomeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRecurringIncome(Guid id, [FromBody] UpdateRecurringIncomeRequest request, CancellationToken ct)
    {
        var result = await _recurringIncomeService.UpdateRecurringIncomeAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/toggle")]
    [ProducesResponseType(typeof(RecurringIncomeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleRecurringIncomeStatus(Guid id, CancellationToken ct)
    {
        var result = await _recurringIncomeService.ToggleRecurringIncomeStatusAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecurringIncome(Guid id, CancellationToken ct)
    {
        var result = await _recurringIncomeService.DeleteRecurringIncomeAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpPost("process-due")]
    [ProducesResponseType(typeof(ProcessRecurringIncomeResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessDueRecurringIncomes([FromBody] ProcessRecurringIncomesRequest? request, CancellationToken ct)
    {
        var result = await _recurringIncomeService.ProcessDueRecurringIncomesAsync(CurrentUserId, request?.AsOfDate, ct);
        return HandleResult(result);
    }
}
