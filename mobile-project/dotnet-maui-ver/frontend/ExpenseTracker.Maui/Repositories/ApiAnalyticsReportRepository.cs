using ExpenseTracker.Maui.Models.Reports;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Live API implementation of IAnalyticsReportRepository delegating to Refit IAnalyticsReportApi
public class ApiAnalyticsReportRepository : IAnalyticsReportRepository
{
    private readonly IAnalyticsReportApi _api;

    public ApiAnalyticsReportRepository(IAnalyticsReportApi api)
    {
        _api = api;
    }

    public Task<IncomeExpenseComparisonModel> GetIncomeExpenseAsync(DateTime from, DateTime to, string groupBy = "month")
        => _api.GetIncomeExpenseAsync(from, to, groupBy);

    public Task<NetWorthReportModel> GetNetWorthAsync(DateTime? from = null, DateTime? to = null)
        => _api.GetNetWorthAsync(from, to);

    public Task<NetWorthSnapshotModel> RecordNetWorthSnapshotAsync(string? notes = null)
        => _api.RecordNetWorthSnapshotAsync(new RecordSnapshotRequestModel { Notes = notes });

    public Task<NetCashflowReportModel> GetCashflowAsync(DateTime from, DateTime to)
        => _api.GetCashflowAsync(from, to);

    public Task<ExpenseStructureReportModel> GetExpenseStructureAsync(DateTime from, DateTime to)
        => _api.GetExpenseStructureAsync(from, to);

    public Task<OutlierDetectionReportModel> GetOutliersAsync(int days = 30, double threshold = 2.0)
        => _api.GetOutliersAsync(days, threshold);
}
