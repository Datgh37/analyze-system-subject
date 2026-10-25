using ExpenseTracker.Maui.Models.ImportExport;
using Refit;

namespace ExpenseTracker.Maui.Repositories.ImportExport;

public interface IDataImportRepository
{
    Task<ImportPreviewResultModel> PreviewSpreadsheetAsync(Stream fileStream, string fileName);
    Task<ImportPreviewResultModel> PreviewUnstructuredTextAsync(string rawText);
    Task<BulkCommitImportResultModel> CommitBulkImportAsync(BulkCommitImportRequestModel request);
}
