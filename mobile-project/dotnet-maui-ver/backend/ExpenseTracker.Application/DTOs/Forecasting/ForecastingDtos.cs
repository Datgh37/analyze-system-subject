using System;
using System.Collections.Generic;

namespace ExpenseTracker.Application.DTOs.Forecasting;

// UC-08.01: Time-series expenditure projection DTO
public class ExpenseForecastDto
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal CurrentSpent { get; set; }
    public int DaysPassed { get; set; }
    public int TotalDaysInCycle { get; set; }
    public int RemainingDays { get; set; }
    public int RemainingWeekdays { get; set; }
    public int RemainingWeekends { get; set; }
    public decimal WeekdayDailyVelocity { get; set; }
    public decimal WeekendDailyVelocity { get; set; }
    public decimal EstimatedFutureSpent { get; set; }
    public decimal EstimatedTotalSpent { get; set; }
    public decimal ProjectedDeficitOrSurplus { get; set; }
    public bool IsOverspendProjected { get; set; }
    public string RiskLevel { get; set; } = "Safe"; // Safe (Green), Caution (Yellow), HighRisk (Red)
    public bool InsufficientData { get; set; }
    public string AdvisoryMessage { get; set; } = string.Empty;
    public List<ForecastDailyPointDto> Trajectory { get; set; } = new();
}

public class ForecastDailyPointDto
{
    public DateTime Date { get; set; }
    public decimal? ActualCumulativeSpent { get; set; }
    public decimal? ProjectedCumulativeSpent { get; set; }
    public bool IsWeekend { get; set; }
    public bool IsFuture { get; set; }
}

// UC-08.02: Liquidity depletion & out-of-money date forecast DTO
public class LiquidityDepletionDto
{
    public decimal TotalLiquidFunds { get; set; }
    public decimal AverageDailyVelocity7Days { get; set; }
    public int DaysUntilDepletion { get; set; }
    public DateTime? OutOfMoneyDate { get; set; }
    public DateTime? NextSalaryDate { get; set; }
    public int DeficitDaysBeforeSalary { get; set; }
    public bool IsLiquidityCrisis { get; set; }
    public string LiquidityHealthStatus { get; set; } = "Safe"; // Safe, Stable, Warning, Crisis
    public string AdvisoryMessage { get; set; } = string.Empty;
}

// UC-08.03: Category deficit risk radar DTO
public class CategoryDeficitRiskDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsEssential { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal CurrentSpent { get; set; }
    public decimal DailyVelocity { get; set; }
    public decimal ProjectedSpentEnd { get; set; }
    public decimal RiskRatio { get; set; }
    public DateTime? EstimatedExhaustionDate { get; set; }
    public int? DaysUntilExhaustion { get; set; }
    public string RiskSeverity { get; set; } = "Safe"; // Safe, Caution, HighRisk
    public string Advice { get; set; } = string.Empty;
}

// UC-08.04: What-If simulation request & result DTO
public enum WhatIfScenarioType
{
    LumpSumExpense = 1,
    InstallmentLoan = 2,
    IncomeReduction = 3,
    EmergencyShock = 4
}

public class WhatIfSimulationRequestDto
{
    public WhatIfScenarioType ScenarioType { get; set; } = WhatIfScenarioType.LumpSumExpense;
    public decimal LumpSumAmount { get; set; }
    public decimal MonthlyInstallmentAmount { get; set; }
    public int InstallmentMonths { get; set; } = 12;
    public decimal IncomeReductionPercentage { get; set; } = 20m;
    public decimal EmergencyExpenseAmount { get; set; }
    public decimal EmergencyIncomeDropPercentage { get; set; } = 0m;
}

public class WhatIfSimulationResultDto
{
    public WhatIfScenarioType ScenarioType { get; set; }
    public string ScenarioDescription { get; set; } = string.Empty;
    
    // Impact on Daily Spending Allowance (DSA)
    public decimal DsaBefore { get; set; }
    public decimal DsaAfter { get; set; }
    public decimal DsaChangePercentage { get; set; }
    
    // Impact on Liquidity & Out-of-Money Date
    public DateTime? OutOfMoneyDateBefore { get; set; }
    public DateTime? OutOfMoneyDateAfter { get; set; }
    public int DaysShiftedEarlier { get; set; }
    
    // Impact on Net Worth
    public decimal NetWorthBefore { get; set; }
    public decimal NetWorthAfter { get; set; }
    public decimal NetWorthChangeAmount { get; set; }
    
    // Impact on Debt-to-Income (DTI)
    public decimal DtiBefore { get; set; }
    public decimal DtiAfter { get; set; }
    
    // Impact on Emergency Fund & Survival Runway (UC-08.04 Emergency Shock)
    public decimal EmergencyFundRemainingBefore { get; set; }
    public decimal EmergencyFundRemainingAfter { get; set; }
    public decimal RunwayMonthsBefore { get; set; }
    public decimal RunwayMonthsAfter { get; set; }
    
    public bool IsHighRiskDecision { get; set; }
    public bool IsLiquidDeficitRisk { get; set; } // Exception E1: Lump sum exceeds liquid wallets
    public string ImpactSummaryMessage { get; set; } = string.Empty;
}

// UC-08.04: Alternate Flow A1 - Convert simulated purchase into a savings goal
public class ConvertToSavingsGoalRequestDto
{
    public string GoalName { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public string? Notes { get; set; }
}

public class ConvertToSavingsGoalResultDto
{
    public Guid GoalId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal MonthlyCommitment { get; set; }
    public string Message { get; set; } = string.Empty;
}
