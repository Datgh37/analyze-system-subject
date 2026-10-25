using ExpenseTracker.Application.DTOs.AiAgent;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service to export financial reports in Excel format (Raw Data or AI-Enhanced Executive Report)
public interface IReportExportService
{
    // Generate simple raw transaction data Excel workbook (.xlsx) via ClosedXML
    Task<Result<ExportResultDto>> ExportRawTransactionsExcelAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken ct = default);

    // Generate smart executive report with AI commentary from Financial Advisor (Richard Thaler philosophy)
    Task<Result<ExportResultDto>> ExportSmartExecutiveReportAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken ct = default);
}
