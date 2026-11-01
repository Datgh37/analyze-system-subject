using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Forecasting;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Domain application service interface for financial forecasting, depletion radars and What-If sandbox (UC-08.01 -> UC-08.04)
public interface IForecastingService
{
    // UC-08.01: Time-series expenditure projection with weekday/weekend decomposition
    Task<Result<ExpenseForecastDto>> GetExpenditureForecastAsync(Guid userId, CancellationToken ct = default);

    // UC-08.02: Liquidity depletion & out-of-money date forecast vs next salary date
    Task<Result<LiquidityDepletionDto>> GetLiquidityDepletionForecastAsync(Guid userId, CancellationToken ct = default);

    // UC-08.03: Category deficit risk radar ranking high-risk essential categories
    Task<Result<List<CategoryDeficitRiskDto>>> GetCategoryDeficitRisksAsync(Guid userId, CancellationToken ct = default);

    // UC-08.04: Interactive What-If simulation sandbox (In-memory, zero DB mutation)
    Task<Result<WhatIfSimulationResultDto>> SimulateWhatIfScenarioAsync(Guid userId, WhatIfSimulationRequestDto request, CancellationToken ct = default);

    // UC-08.04: Alternate Flow A1 - Convert simulated purchase into a savings goal
    Task<Result<ConvertToSavingsGoalResultDto>> ConvertWhatIfToSavingsGoalAsync(Guid userId, ConvertToSavingsGoalRequestDto request, CancellationToken ct = default);
}
