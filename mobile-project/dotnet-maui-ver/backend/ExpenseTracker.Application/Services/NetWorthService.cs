using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Application Service calculating real-time personal balance sheet and net worth health indicators (UC-07.02)
public class NetWorthService : INetWorthService
{
    private readonly IApplicationDbContext _context;

    public NetWorthService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NetWorthBreakdownDto> GetCurrentNetWorthAsync(Guid userId, CancellationToken ct = default)
    {
        // 1. Wallets breakdown
        var wallets = await _context.Wallets
            .Where(w => w.UserId == userId && w.Status == WalletStatus.Active)
            .ToListAsync(ct);

        decimal liquidCash = 0m;
        decimal creditCardDebt = 0m;

        foreach (var wallet in wallets)
        {
            if (wallet.WalletType == WalletType.CreditCard)
            {
                if (wallet.Balance < 0)
                {
                    creditCardDebt += Math.Abs(wallet.Balance);
                }
                else
                {
                    liquidCash += wallet.Balance;
                }
            }
            else
            {
                if (wallet.Balance > 0)
                {
                    liquidCash += wallet.Balance;
                }
            }
        }

        // 2. Savings & Emergency goals breakdown
        var savingsTotal = await _context.SavingsGoals
            .Where(g => g.UserId == userId && g.Status != SavingsGoalStatus.Cancelled)
            .SumAsync(g => (decimal?)g.CurrentAmount, ct) ?? 0m;

        // 3. Debts payable breakdown
        var debtsPayable = await _context.Debts
            .Where(d => d.UserId == userId && d.Status != DebtStatus.PaidOff)
            .SumAsync(d => (decimal?)d.CurrentBalance, ct) ?? 0m;

        decimal totalAssets = liquidCash + savingsTotal;
        decimal totalLiabilities = debtsPayable + creditCardDebt;
        decimal netWorth = totalAssets - totalLiabilities;

        // 4. Financial health metrics
        decimal debtToAssetRatio = 0m;
        if (totalAssets > 0)
        {
            debtToAssetRatio = Math.Round((totalLiabilities / totalAssets) * 100m, 2);
        }
        else if (totalLiabilities > 0)
        {
            debtToAssetRatio = 100m;
        }

        string healthRating;
        if (netWorth < 0 || debtToAssetRatio > 50m)
        {
            healthRating = "CriticalRisk";
        }
        else if (debtToAssetRatio >= 30m)
        {
            healthRating = "Caution";
        }
        else
        {
            healthRating = "Healthy";
        }

        // 5. Survival runway (months) based on recent needs
        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
        var recentNeeds = await _context.Transactions
            .Where(t => t.UserId == userId &&
                        t.TransactionType == TransactionType.Expense &&
                        t.TransactionDate >= ninetyDaysAgo &&
                        (t.Category.BudgetRuleMapping == BudgetRuleMapping.Needs ||
                         t.Category.Name.Contains("Ăn uống") ||
                         t.Category.Name.Contains("Nhà") ||
                         t.Category.Name.Contains("Điện") ||
                         t.Category.Name.Contains("Y tế")))
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

        decimal monthlyEssential = recentNeeds > 0 ? Math.Round(recentNeeds / 3m, 0) : 5_000_000m;
        decimal survivalRunwayMonths = monthlyEssential > 0 ? Math.Round(totalAssets / monthlyEssential, 1) : 0m;

        return new NetWorthBreakdownDto
        {
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            NetWorth = netWorth,
            Assets = new AssetBreakdownDto
            {
                LiquidCash = liquidCash,
                SavingsAndEmergency = savingsTotal
            },
            Liabilities = new LiabilityBreakdownDto
            {
                DebtsPayable = debtsPayable,
                CreditCardDebt = creditCardDebt
            },
            DebtToAssetRatio = debtToAssetRatio,
            FinancialHealthRating = healthRating,
            SurvivalRunwayMonths = survivalRunwayMonths,
            CalculatedAt = DateTime.UtcNow
        };
    }

    public async Task<List<NetWorthHistoryItemDto>> GetNetWorthHistoryAsync(Guid userId, int limit = 12, CancellationToken ct = default)
    {
        var snapshots = await _context.NetWorthSnapshots
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SnapshotDate)
            .Take(limit)
            .Select(s => new NetWorthHistoryItemDto
            {
                Id = s.Id,
                SnapshotDate = s.SnapshotDate,
                TotalAssets = s.TotalAssets,
                TotalLiabilities = s.TotalLiabilities,
                NetWorth = s.NetWorth,
                Notes = s.Notes
            })
            .ToListAsync(ct);

        return snapshots;
    }

    public async Task<NetWorthHistoryItemDto> RecordSnapshotAsync(Guid userId, RecordNetWorthSnapshotDto? request = null, CancellationToken ct = default)
    {
        var current = await GetCurrentNetWorthAsync(userId, ct);
        var today = DateTime.UtcNow.Date;

        var existingSnapshot = await _context.NetWorthSnapshots
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SnapshotDate.Date == today, ct);

        if (existingSnapshot != null)
        {
            existingSnapshot.Recalculate(current.TotalAssets, current.TotalLiabilities, request?.Notes);
        }
        else
        {
            existingSnapshot = new NetWorthSnapshot(
                userId,
                DateTime.UtcNow,
                current.TotalAssets,
                current.TotalLiabilities,
                request?.Notes);

            await _context.NetWorthSnapshots.AddAsync(existingSnapshot, ct);
        }

        await _context.SaveChangesAsync(ct);

        return new NetWorthHistoryItemDto
        {
            Id = existingSnapshot.Id,
            SnapshotDate = existingSnapshot.SnapshotDate,
            TotalAssets = existingSnapshot.TotalAssets,
            TotalLiabilities = existingSnapshot.TotalLiabilities,
            NetWorth = existingSnapshot.NetWorth,
            Notes = existingSnapshot.Notes
        };
    }
}
