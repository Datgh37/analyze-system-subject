using ExpenseTracker.Application.DTOs.ImportExport;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service to handle hybrid multi-source financial data import (Structured Excel + Unstructured Text)
public interface IHybridDataImportService
{
    // Parse structured spreadsheet (.xlsx, .csv) via ClosedXML without token cost
    Task<Result<ImportPreviewResultDto>> ParseSpreadsheetImportAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        CancellationToken ct = default);

    // Parse unstructured note / text fragments via Gemini NLP reasoning
    Task<Result<ImportPreviewResultDto>> ParseUnstructuredTextImportAsync(
        Guid userId,
        string rawText,
        CancellationToken ct = default);

    // Commit selected rows from HITL Preview to database atomically
    Task<Result<BulkCommitImportResultDto>> CommitBulkImportAsync(
        Guid userId,
        BulkCommitImportRequestDto request,
        CancellationToken ct = default);
}
