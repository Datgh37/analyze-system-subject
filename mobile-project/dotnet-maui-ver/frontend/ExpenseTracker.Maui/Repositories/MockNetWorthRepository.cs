using ExpenseTracker.Maui.Models.Reports;

namespace ExpenseTracker.Maui.Repositories;

public class MockNetWorthRepository : INetWorthRepository
{
    private NetWorthBreakdownModel _breakdown;
    private readonly List<NetWorthHistoryItemModel> _history;

    public MockNetWorthRepository()
    {
        _breakdown = new NetWorthBreakdownModel
        {
            TotalAssets = 150_000_000m,
            TotalLiabilities = 30_000_000m,
            NetWorth = 120_000_000m,
            Assets = new AssetBreakdownModel
            {
                LiquidCash = 50_000_000m,
                SavingsAndEmergency = 100_000_000m
            },
            Liabilities = new LiabilityBreakdownModel
            {
                DebtsPayable = 25_000_000m,
                CreditCardDebt = 5_000_000m
            },
            DebtToAssetRatio = 20.0m,
            FinancialHealthRating = "Healthy",
            SurvivalRunwayMonths = 20.0m,
            CalculatedAt = DateTime.UtcNow
        };

        _history = new List<NetWorthHistoryItemModel>
        {
            new() { Id = Guid.NewGuid(), SnapshotDate = DateTime.UtcNow.AddMonths(-2), TotalAssets = 130_000_000m, TotalLiabilities = 35_000_000m, NetWorth = 95_000_000m, Notes = "Tháng 8" },
            new() { Id = Guid.NewGuid(), SnapshotDate = DateTime.UtcNow.AddMonths(-1), TotalAssets = 140_000_000m, TotalLiabilities = 32_000_000m, NetWorth = 108_000_000m, Notes = "Tháng 9" },
            new() { Id = Guid.NewGuid(), SnapshotDate = DateTime.UtcNow, TotalAssets = 150_000_000m, TotalLiabilities = 30_000_000m, NetWorth = 120_000_000m, Notes = "Tháng 10" }
        };
    }

    public Task<NetWorthBreakdownModel> GetCurrentNetWorthAsync()
    {
        return Task.FromResult(_breakdown);
    }

    public Task<List<NetWorthHistoryItemModel>> GetNetWorthHistoryAsync(int limit = 12)
    {
        return Task.FromResult(_history.OrderByDescending(h => h.SnapshotDate).Take(limit).ToList());
    }

    public Task<NetWorthHistoryItemModel> RecordSnapshotAsync(RecordNetWorthSnapshotRequest request)
    {
        var item = new NetWorthHistoryItemModel
        {
            Id = Guid.NewGuid(),
            SnapshotDate = DateTime.UtcNow,
            TotalAssets = _breakdown.TotalAssets,
            TotalLiabilities = _breakdown.TotalLiabilities,
            NetWorth = _breakdown.NetWorth,
            Notes = request.Notes
        };
        _history.Add(item);
        return Task.FromResult(item);
    }
}

