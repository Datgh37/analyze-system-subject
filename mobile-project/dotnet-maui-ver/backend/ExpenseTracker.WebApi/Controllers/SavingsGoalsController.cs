using ExpenseTracker.Application.DTOs.SavingsGoals;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing smart savings goals, deposits/withdrawals, feasibility checks, and financial jars (UC-06.01 - 06.05)
[Authorize]
[Route("api/savings-goals")]
[Route("api/savingsgoals")]
public class SavingsGoalsController : BaseApiController
{
    private readonly ISavingsGoalService _goalService;

    public SavingsGoalsController(ISavingsGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SavingsGoalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserGoals(CancellationToken ct)
    {
        var result = await _goalService.GetUserGoalsAsync(CurrentUserId, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGoalById(Guid id, CancellationToken ct)
    {
        var result = await _goalService.GetGoalByIdAsync(CurrentUserId, id, ct);
        if (result == null) return NotFound(new { message = "Không tìm thấy mục tiêu tích lũy." });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGoal([FromBody] CreateSavingsGoalDto request, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.CreateGoalAsync(CurrentUserId, request, ct);
            return CreatedAtAction(nameof(GetGoalById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGoal(Guid id, [FromBody] UpdateSavingsGoalDto request, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.UpdateGoalAsync(CurrentUserId, id, request, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Partially updates savings goal properties via HTTP PATCH (RFC 7396)
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchGoal(Guid id, [FromBody] PatchSavingsGoalDto request, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.PatchGoalAsync(CurrentUserId, id, request, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Partially updates savings goal status (Active, Paused, Completed, Cancelled)
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchGoalStatus(Guid id, [FromBody] PatchSavingsGoalStatusDto request, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.PatchGoalStatusAsync(CurrentUserId, id, request.Status, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGoal(Guid id, CancellationToken ct)
    {
        try
        {
            await _goalService.DeleteGoalAsync(CurrentUserId, id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Deposit money from wallet into savings goal (UC-06.02)
    [HttpPost("{id:guid}/deposit")]
    [ProducesResponseType(typeof(SavingsGoalContributionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] DepositSavingsGoalDto request, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.DepositAsync(CurrentUserId, id, request, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Withdraw money from savings goal with Richard Thaler Nudge (UC-06.03)
    [HttpPost("{id:guid}/withdraw")]
    [ProducesResponseType(typeof(SavingsGoalContributionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] WithdrawSavingsGoalDto request, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.WithdrawAsync(CurrentUserId, id, request, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/contributions")]
    [ProducesResponseType(typeof(List<SavingsGoalContributionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGoalContributions(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.GetGoalContributionsAsync(CurrentUserId, id, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Evaluate financial feasibility (UC-06.01)
    [HttpGet("{id:guid}/feasibility")]
    [ProducesResponseType(typeof(GoalFeasibilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EvaluateFeasibility(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.EvaluateGoalFeasibilityAsync(CurrentUserId, id, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Jars allocation breakdown (UC-06.05)
    [HttpGet("jars-summary")]
    [ProducesResponseType(typeof(JarsSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJarsSummary(CancellationToken ct)
    {
        var result = await _goalService.GetJarsSummaryAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // Configure Auto-deposit schedule (UC-06.04)
    [HttpPost("{id:guid}/auto-deposit")]
    [HttpPatch("{id:guid}/auto-deposit")]
    [HttpPut("{id:guid}/auto-deposit")]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfigureAutoDeposit(Guid id, [FromBody] ConfigureAutoDepositDto request, CancellationToken ct)
    {
        try
        {
            var result = await _goalService.ConfigureAutoDepositAsync(CurrentUserId, id, request, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
