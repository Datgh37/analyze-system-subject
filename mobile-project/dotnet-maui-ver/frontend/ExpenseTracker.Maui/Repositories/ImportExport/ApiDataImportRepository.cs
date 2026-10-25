using ExpenseTracker.Maui.Models.ImportExport;
using ExpenseTracker.Maui.Services.Api;
using Refit;

namespace ExpenseTracker.Maui.Repositories.ImportExport;

public class ApiDataImportRepository : IDataImportRepository
{
    private readonly IDataImportApi _api;

    public ApiDataImportRepository(IDataImportApi api)
    {
        _api = api;
    }

    public async Task<ImportPreviewResultModel> PreviewSpreadsheetAsync(Stream fileStream, string fileName)
    {
        var streamPart = new StreamPart(fileStream, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        return await _api.PreviewSpreadsheetAsync(streamPart);
    }

    public async Task<ImportPreviewResultModel> PreviewUnstructuredTextAsync(string rawText)
    {
        return await _api.PreviewUnstructuredTextAsync(new { Text = rawText });
    }

    public async Task<BulkCommitImportResultModel> CommitBulkImportAsync(BulkCommitImportRequestModel request)
    {
        return await _api.CommitBulkImportAsync(request);
    }
}
