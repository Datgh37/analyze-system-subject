using ExpenseTracker.Maui.Models.Reports;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for financial analytics and reporting (Clean MVVM Architecture - B3)
public interface IAnalyticsReportRepository
{
    Task<IncomeExpenseComparisonModel> GetIncomeExpenseAsync(DateTime from, DateTime to, string groupBy = "month");
    Task<NetWorthReportModel> GetNetWorthAsync(DateTime? from = null, DateTime? to = null);
    Task<NetWorthSnapshotModel> RecordNetWorthSnapshotAsync(string? notes = null);
    Task<NetCashflowReportModel> GetCashflowAsync(DateTime from, DateTime to);
    Task<ExpenseStructureReportModel> GetExpenseStructureAsync(DateTime from, DateTime to);
    Task<OutlierDetectionReportModel> GetOutliersAsync(int days = 30, double threshold = 2.0);
}
