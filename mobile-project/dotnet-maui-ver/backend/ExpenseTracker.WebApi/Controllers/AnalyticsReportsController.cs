using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing multi-dimensional financial reporting and quantitative analytics (UC-07.01 -> UC-07.05)
[Authorize]
[Route("api/analytics/reports")]
public class AnalyticsReportsController : BaseApiController
{
    private readonly IAnalyticsReportService _reportService;
    private readonly IReportExportService _exportService;

    public AnalyticsReportsController(
        IAnalyticsReportService reportService,
        IReportExportService exportService)
    {
        _reportService = reportService;
        _exportService = exportService;
    }

    // UC-07.01: Income vs Expense Comparison over time (MoM, YoY, Group by Day/Month)
    [HttpGet("income-expense")]
    [ProducesResponseType(typeof(IncomeExpenseComparisonDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIncomeExpense(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] string groupBy = "month",
        CancellationToken ct = default)
    {
        var result = await _reportService.GetIncomeExpenseComparisonAsync(CurrentUserId, from, to, groupBy, ct);
        return HandleResult(result);
    }

    // UC-07.02: Net Worth Snapshot and Evolution
    [HttpGet("net-worth")]
    [ProducesResponseType(typeof(NetWorthReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNetWorth(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct = default)
    {
        var result = await _reportService.GetNetWorthReportAsync(CurrentUserId, from, to, ct);
        return HandleResult(result);
    }

    // UC-07.02: Trigger Net Worth Snapshot
    [HttpPost("net-worth/snapshot")]
    [ProducesResponseType(typeof(NetWorthSnapshotDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RecordNetWorthSnapshot(
        [FromBody] RecordSnapshotRequestDto? request,
        CancellationToken ct = default)
    {
        var result = await _reportService.RecordNetWorthSnapshotAsync(CurrentUserId, request?.Notes, ct);
        return HandleResult(result);
    }

    // UC-07.03: Net Cashflow and Waterfall Analysis
    [HttpGet("cashflow")]
    [ProducesResponseType(typeof(NetCashflowReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCashflow(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _reportService.GetNetCashflowReportAsync(CurrentUserId, from, to, ct);
        return HandleResult(result);
    }

    // UC-07.04: Expense Structure Analysis (Sub-categories, 50/30/20, Kakeibo)
    [HttpGet("expense-structure")]
    [ProducesResponseType(typeof(ExpenseStructureReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenseStructure(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _reportService.GetExpenseStructureReportAsync(CurrentUserId, from, to, ct);
        return HandleResult(result);
    }

    // UC-07.05: Spending Trend & Z-Score Outlier Detection
    [HttpGet("outliers")]
    [ProducesResponseType(typeof(OutlierDetectionReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutliers(
        [FromQuery] int days = 30,
        [FromQuery] double threshold = 2.0,
        CancellationToken ct = default)
    {
        var result = await _reportService.DetectSpendingOutliersAsync(CurrentUserId, days, threshold, ct);
        return HandleResult(result);
    }

    // UC-07.01 A1: Export financial transactions to Excel (Raw or Smart Executive with AI Commentary)
    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] bool smart = false,
        CancellationToken ct = default)
    {
        var result = smart
            ? await _exportService.ExportSmartExecutiveReportAsync(CurrentUserId, year, month, ct)
            : await _exportService.ExportRawTransactionsExcelAsync(CurrentUserId, year, month, ct);

        if (result.IsFailure || result.Value == null)
            return HandleResult(result);

        return File(result.Value.FileBytes, result.Value.ContentType, result.Value.FileName);
    }
}
