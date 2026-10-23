using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing ledger transactions and income recording
[Authorize]
public class TransactionsController : BaseApiController
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionFilterRequest filter, CancellationToken ct)
    {
        var result = await _transactionService.GetTransactionsAsync(CurrentUserId, filter, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionById(Guid id, CancellationToken ct)
    {
        var result = await _transactionService.GetTransactionByIdAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpPost("incomes")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeTransactionRequest request, CancellationToken ct)
    {
        var result = await _transactionService.CreateIncomeAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    [HttpPut("incomes/{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIncome(Guid id, [FromBody] UpdateIncomeTransactionRequest request, CancellationToken ct)
    {
        var result = await _transactionService.UpdateIncomeAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    [HttpDelete("incomes/{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIncome(Guid id, CancellationToken ct)
    {
        var result = await _transactionService.DeleteIncomeAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpGet("incomes/monthly-total")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonthlyIncomeTotal([FromQuery] int month, [FromQuery] int year, CancellationToken ct)
    {
        if (month < 1 || month > 12)
            month = DateTime.UtcNow.Month;

        if (year < 2000 || year > 2100)
            year = DateTime.UtcNow.Year;

        var result = await _transactionService.GetTotalIncomeAmountAsync(CurrentUserId, month, year, ct);
        return HandleResult(result);
    }

    [HttpPost("expenses")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseTransactionRequest request, CancellationToken ct)
    {
        var result = await _transactionService.CreateExpenseAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    [HttpPut("expenses/{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseTransactionRequest request, CancellationToken ct)
    {
        var result = await _transactionService.UpdateExpenseAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    [HttpDelete("expenses/{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpense(Guid id, CancellationToken ct)
    {
        var result = await _transactionService.DeleteExpenseAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpGet("expenses/{id:guid}/details")]
    [ProducesResponseType(typeof(ExpenseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExpenseDetails(Guid id, CancellationToken ct)
    {
        var result = await _transactionService.GetExpenseDetailsAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpGet("expenses/monthly-total")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonthlyExpenseTotal([FromQuery] int month, [FromQuery] int year, CancellationToken ct)
    {
        if (month < 1 || month > 12)
            month = DateTime.UtcNow.Month;

        if (year < 2000 || year > 2100)
            year = DateTime.UtcNow.Year;

        var result = await _transactionService.GetTotalExpenseAmountAsync(CurrentUserId, month, year, ct);
        return HandleResult(result);
    }
}
