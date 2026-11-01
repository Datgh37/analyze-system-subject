namespace ExpenseTracker.Maui.Models.Reports;

public class NetWorthBreakdownModel
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    
    public AssetBreakdownModel Assets { get; set; } = new();
    public LiabilityBreakdownModel Liabilities { get; set; } = new();

    public decimal DebtToAssetRatio { get; set; }
    public string FinancialHealthRating { get; set; } = "Healthy";
    public decimal SurvivalRunwayMonths { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}

public class AssetBreakdownModel
{
    public decimal LiquidCash { get; set; }
    public decimal SavingsAndEmergency { get; set; }
    public decimal Total => LiquidCash + SavingsAndEmergency;
}

public class LiabilityBreakdownModel
{
    public decimal DebtsPayable { get; set; }
    public decimal CreditCardDebt { get; set; }
    public decimal Total => DebtsPayable + CreditCardDebt;
}

public class NetWorthHistoryItemModel
{
    public Guid Id { get; set; }
    public DateTime SnapshotDate { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    public string? Notes { get; set; }
}

public class RecordNetWorthSnapshotRequest
{
    public string? Notes { get; set; }
}

