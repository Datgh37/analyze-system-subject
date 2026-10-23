using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

public interface IAnalyticsReportService
{
    // UC-07.01: Income vs Expense Comparison over time
    Task<Result<IncomeExpenseComparisonDto>> GetIncomeExpenseComparisonAsync(
        Guid userId,
        DateTime fromDate,
        DateTime toDate,
        string groupBy = "month",
        CancellationToken cancellationToken = default);

    // UC-07.02: Net Worth Snapshot and Evolution
    Task<Result<NetWorthReportDto>> GetNetWorthReportAsync(
        Guid userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    // UC-07.02: Trigger Net Worth Snapshot
    Task<Result<NetWorthSnapshotDto>> RecordNetWorthSnapshotAsync(
        Guid userId,
        string? notes = null,
        CancellationToken cancellationToken = default);

    // UC-07.03: Net Cashflow and Waterfall Analysis
    Task<Result<NetCashflowReportDto>> GetNetCashflowReportAsync(
        Guid userId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    // UC-07.04: Expense Structure Analysis (Sub-categories, 50/30/20, Kakeibo)
    Task<Result<ExpenseStructureReportDto>> GetExpenseStructureReportAsync(
        Guid userId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    // UC-07.05: Spending Trend & Z-Score Outlier Detection
    Task<Result<OutlierDetectionReportDto>> DetectSpendingOutliersAsync(
        Guid userId,
        int days = 30,
        double threshold = 2.0,
        CancellationToken cancellationToken = default);
}
