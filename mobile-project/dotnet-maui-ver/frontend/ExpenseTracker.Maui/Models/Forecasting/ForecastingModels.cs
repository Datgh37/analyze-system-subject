namespace ExpenseTracker.Maui.Models.Forecasting;

// UC-08.01: Daily forecast trajectory point
public class DailyForecastPointModel
{
    public DateTime Date { get; set; }
    public string DayLabel { get; set; } = string.Empty;
    public bool IsWeekend { get; set; }
    public decimal DailySpent { get; set; }
    public decimal CumulativeSpent { get; set; }
    public decimal DailyBudgetLimit { get; set; }
    public decimal CumulativeBudgetLimit { get; set; }
    public bool IsOverBudget { get; set; }
}

// UC-08.01: Comprehensive expenditure forecast model
public class ExpenseForecastModel
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal CurrentSpent { get; set; }
    public int DaysPassed { get; set; }
    public int TotalDaysInCycle { get; set; }
    public int RemainingDays { get; set; }
    public decimal WeekdayDailyVelocity { get; set; }
    public decimal WeekendDailyVelocity { get; set; }
    public decimal EstimatedFutureSpent { get; set; }
    public decimal EstimatedTotalSpent { get; set; }
    public decimal ProjectedOverspendAmount { get; set; }
    public DateTime? ExhaustionDate { get; set; }
    public int? DaysUntilExhaustion { get; set; }
    public string RiskLevel { get; set; } = "Safe"; // Safe, Warning, HighRisk
    public string AdvisoryMessage { get; set; } = string.Empty;
    public bool InsufficientData { get; set; }
    public List<DailyForecastPointModel> Trajectory { get; set; } = new();
}

// UC-08.02: Liquidity depletion and salary gap projection
public class LiquidityDepletionModel
{
    public decimal TotalLiquidFunds { get; set; }
    public decimal Recent7DaysDailyVelocity { get; set; }
    public int DaysUntilDepletion { get; set; }
    public DateTime OutOfMoneyDate { get; set; }
    public DateTime? NextSalaryDate { get; set; }
    public int? DaysUntilNextSalary { get; set; }
    public int? DeficitDaysBeforeSalary { get; set; }
    public bool IsLiquidityCrisis { get; set; }
    public string LiquidityHealthStatus { get; set; } = "Safe"; // Safe, Caution, Crisis
    public string AdvisoryMessage { get; set; } = string.Empty;
}

// UC-08.03: Category deficit risk radar model
public class CategoryDeficitRiskModel
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

// UC-08.04: What-If simulation types
public enum WhatIfScenarioType
{
    LumpSumExpense = 1,
    InstallmentLoan = 2,
    IncomeReduction = 3
}

// UC-08.04: Simulation request parameters
public class WhatIfSimulationRequestModel
{
    public WhatIfScenarioType ScenarioType { get; set; } = WhatIfScenarioType.LumpSumExpense;
    public decimal LumpSumAmount { get; set; } = 5000000m;
    public decimal MonthlyInstallmentAmount { get; set; } = 2500000m;
    public int InstallmentMonths { get; set; } = 12;
    public decimal IncomeReductionPercentage { get; set; } = 20m;
}

// UC-08.04: Simulation result comparison
public class WhatIfSimulationResultModel
{
    public WhatIfScenarioType ScenarioType { get; set; }
    public string ScenarioDescription { get; set; } = string.Empty;
    public decimal DsaBefore { get; set; }
    public decimal DsaAfter { get; set; }
    public decimal DsaChangePercentage { get; set; }
    public DateTime OutOfMoneyDateBefore { get; set; }
    public DateTime OutOfMoneyDateAfter { get; set; }
    public int DaysShiftedEarlier { get; set; }
    public decimal NetWorthBefore { get; set; }
    public decimal NetWorthAfter { get; set; }
    public decimal NetWorthChangeAmount { get; set; }
    public decimal DtiBefore { get; set; }
    public decimal DtiAfter { get; set; }
    public bool IsHighRiskDecision { get; set; }
    public string ImpactSummaryMessage { get; set; } = string.Empty;
}
