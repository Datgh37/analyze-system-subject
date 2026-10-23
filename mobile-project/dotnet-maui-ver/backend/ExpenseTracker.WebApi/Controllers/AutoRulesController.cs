using ExpenseTracker.Application.DTOs.AutoRules;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing auto-categorization smart keyword rules (UC-04.07)
[Authorize]
[Route("api/auto-rules")]
public class AutoRulesController : BaseApiController
{
    private readonly IAutoCategorizationService _autoRuleService;

    public AutoRulesController(IAutoCategorizationService autoRuleService)
    {
        _autoRuleService = autoRuleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<AutoRuleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRules(CancellationToken ct)
    {
        var result = await _autoRuleService.GetRulesAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AutoRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRuleById(Guid id, CancellationToken ct)
    {
        var result = await _autoRuleService.GetRuleByIdAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AutoRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRule([FromBody] CreateAutoRuleRequest request, CancellationToken ct)
    {
        var result = await _autoRuleService.CreateRuleAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AutoRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRule(Guid id, [FromBody] UpdateAutoRuleRequest request, CancellationToken ct)
    {
        var result = await _autoRuleService.UpdateRuleAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRule(Guid id, CancellationToken ct)
    {
        var result = await _autoRuleService.DeleteRuleAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    [HttpGet("match")]
    [ProducesResponseType(typeof(MatchCategoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MatchCategory([FromQuery] string text, CancellationToken ct)
    {
        var result = await _autoRuleService.MatchCategoryAsync(CurrentUserId, text, ct);
        return HandleResult(result);
    }
}
