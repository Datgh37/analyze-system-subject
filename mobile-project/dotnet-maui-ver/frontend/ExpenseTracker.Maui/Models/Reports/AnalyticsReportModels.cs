namespace ExpenseTracker.Maui.Models.Reports;

// UC-07.01: Income vs Expense Comparison over time
public class IncomeExpenseComparisonModel
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetSurplus { get; set; }
    public decimal GrowthMoM { get; set; }
    public decimal GrowthYoY { get; set; }
    public List<PeriodDataPointModel> Breakdown { get; set; } = new();
}

public class PeriodDataPointModel
{
    public string PeriodLabel { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal NetSurplus { get; set; }
}

// UC-07.02: Net Worth Snapshot and Evolution
public class NetWorthReportModel
{
    public decimal CurrentNetWorth { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public bool IsNegativeNetWorthWarning { get; set; }
    public string? HealthAdvice { get; set; }
    public List<NetWorthSnapshotModel> HistoricalSnapshots { get; set; } = new();
}

public class NetWorthSnapshotModel
{
    public Guid Id { get; set; }
    public DateTime SnapshotDate { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    public string? Notes { get; set; }
}

public class RecordSnapshotRequestModel
{
    public string? Notes { get; set; }
}

// UC-07.03: Net Cashflow and Waterfall Analysis
public class NetCashflowReportModel
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetCashflow { get; set; }
    public decimal SavingsRate { get; set; }
    public string HealthStatus { get; set; } = "Good";
    public string HealthRecommendation { get; set; } = string.Empty;
    public List<WaterfallStepModel> WaterfallSteps { get; set; } = new();
}

public class WaterfallStepModel
{
    public string StepName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal RunningBalance { get; set; }
    public string StepType { get; set; } = "Inflow";
}

// UC-07.04: Expense Structure Analysis (Sub-categories, 50/30/20, Kakeibo)
public class ExpenseStructureReportModel
{
    public decimal TotalExpense { get; set; }
    public List<CategorySpendingShareModel> BySubCategory { get; set; } = new();
    public Expense503020SummaryModel By503020 { get; set; } = new();
    public ExpenseKakeiboSummaryModel ByKakeibo { get; set; } = new();
    public List<string> DominantCategoryAlerts { get; set; } = new();
}

public class CategorySpendingShareModel
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string IconKey { get; set; } = "folder";
    public string ColorHex { get; set; } = "#10B981";
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public bool IsDominantAlert { get; set; }
}

public class Expense503020SummaryModel
{
    public decimal NeedsAmount { get; set; }
    public decimal NeedsPercentage { get; set; }
    public decimal WantsAmount { get; set; }
    public decimal WantsPercentage { get; set; }
    public decimal SavingsDebtAmount { get; set; }
    public decimal SavingsDebtPercentage { get; set; }
}

public class ExpenseKakeiboSummaryModel
{
    public decimal SurvivalAmount { get; set; }
    public decimal SurvivalPercentage { get; set; }
    public decimal OptionalAmount { get; set; }
    public decimal OptionalPercentage { get; set; }
    public decimal CultureAmount { get; set; }
    public decimal CulturePercentage { get; set; }
    public decimal ExtraAmount { get; set; }
    public decimal ExtraPercentage { get; set; }
}

// UC-07.05: Spending Trend & Z-Score Outlier Detection
public class OutlierDetectionReportModel
{
    public int TotalAnalyzedTransactions { get; set; }
    public int OutlierCount { get; set; }
    public List<OutlierTransactionModel> Outliers { get; set; } = new();
    public List<CategorySurgeModel> CategorySurges { get; set; } = new();
}

public class OutlierTransactionModel
{
    public Guid TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Note { get; set; }
    public decimal CategoryMean { get; set; }
    public decimal CategoryStdDev { get; set; }
    public double ZScore { get; set; }
    public string AlertSeverity { get; set; } = "Moderate";
}

public class CategorySurgeModel
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal PreviousAverage { get; set; }
    public decimal CurrentPeriodTotal { get; set; }
    public decimal SurgePercentage { get; set; }
    public string WarningMessage { get; set; } = string.Empty;
}
