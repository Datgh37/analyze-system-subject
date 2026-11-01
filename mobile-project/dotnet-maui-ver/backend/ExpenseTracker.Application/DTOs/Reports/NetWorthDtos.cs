namespace ExpenseTracker.Application.DTOs.Reports;

// Aggregate balance sheet breakdown for Net Worth (UC-07.02)
public class NetWorthBreakdownDto
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    
    public AssetBreakdownDto Assets { get; set; } = new();
    public LiabilityBreakdownDto Liabilities { get; set; } = new();

    public decimal DebtToAssetRatio { get; set; } // Percentage: (TotalLiabilities / TotalAssets) * 100
    public string FinancialHealthRating { get; set; } = "Healthy"; // "Healthy", "Caution", "CriticalRisk"
    public decimal SurvivalRunwayMonths { get; set; } // (LiquidCash + EmergencyFund) / MonthlyEssentialExpenses
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}

public class AssetBreakdownDto
{
    public decimal LiquidCash { get; set; } // Cash in active wallets
    public decimal SavingsAndEmergency { get; set; } // Savings goals and emergency fund
    public decimal Total => LiquidCash + SavingsAndEmergency;
}

public class LiabilityBreakdownDto
{
    public decimal DebtsPayable { get; set; } // External borrowing / Debts
    public decimal CreditCardDebt { get; set; } // Credit card balances
    public decimal Total => DebtsPayable + CreditCardDebt;
}

// Historical point-in-time snapshot of Net Worth
public class NetWorthHistoryItemDto
{
    public Guid Id { get; set; }
    public DateTime SnapshotDate { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    public string? Notes { get; set; }
}

// Request to record a manual or scheduled Net Worth snapshot
public class RecordNetWorthSnapshotDto
{
    public string? Notes { get; set; }
}

