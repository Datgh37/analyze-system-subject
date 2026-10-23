using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Forecasting;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing predictive financial forecasting, liquidity exhaustion and What-If sandbox (UC-08.01 -> UC-08.04)
[Authorize]
[Route("api/forecasting")]
public class ForecastingController : BaseApiController
{
    private readonly IForecastingService _forecastingService;

    public ForecastingController(IForecastingService forecastingService)
    {
        _forecastingService = forecastingService;
    }

    // UC-08.01: Time-series expenditure projection with weekday/weekend effect
    [HttpGet("expenditure")]
    [ProducesResponseType(typeof(ExpenseForecastDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExpenditureForecast(CancellationToken ct = default)
    {
        var result = await _forecastingService.GetExpenditureForecastAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    // UC-08.02: Liquidity depletion & out-of-money date forecast vs next salary date
    [HttpGet("liquidity-depletion")]
    [ProducesResponseType(typeof(LiquidityDepletionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLiquidityDepletionForecast(CancellationToken ct = default)
    {
        var result = await _forecastingService.GetLiquidityDepletionForecastAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    // UC-08.03: Category deficit risk radar ranking high-risk essential categories
    [HttpGet("category-risks")]
    [ProducesResponseType(typeof(List<CategoryDeficitRiskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategoryDeficitRisks(CancellationToken ct = default)
    {
        var result = await _forecastingService.GetCategoryDeficitRisksAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    // UC-08.04: Interactive What-If simulation sandbox (In-memory, zero DB mutation)
    [HttpPost("what-if")]
    [ProducesResponseType(typeof(WhatIfSimulationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SimulateWhatIf([FromBody] WhatIfSimulationRequestDto request, CancellationToken ct = default)
    {
        var result = await _forecastingService.SimulateWhatIfScenarioAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }
}
