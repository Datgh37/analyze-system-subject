using ExpenseTracker.Maui.Models.ImportExport;

namespace ExpenseTracker.Maui.Repositories.ImportExport;

public class MockDataImportRepository : IDataImportRepository
{
    public Task<ImportPreviewResultModel> PreviewSpreadsheetAsync(Stream fileStream, string fileName)
    {
        var mockResult = new ImportPreviewResultModel
        {
            SourceType = "StructuredExcel",
            TotalRows = 3,
            ValidRows = 3,
            InvalidRows = 0,
            TotalAmount = 535_000m,
            SummaryMessage = "Bóc tách thành công 3/3 dòng từ bảng tính Excel",
            Rows = new List<ParsedImportRowModel>
            {
                new() { RowIndex = 1, TransactionDate = DateTime.UtcNow.AddDays(-2), Amount = 45000m, TransactionType = "Expense", CategoryName = "Ăn uống", Note = "Ăn trưa cơm văn phòng", IsValid = true, IsSelected = true },
                new() { RowIndex = 2, TransactionDate = DateTime.UtcNow.AddDays(-1), Amount = 40000m, TransactionType = "Expense", CategoryName = "Ăn uống", Note = "Cà phê sáng", IsValid = true, IsSelected = true },
                new() { RowIndex = 3, TransactionDate = DateTime.UtcNow, Amount = 450000m, TransactionType = "Expense", CategoryName = "Hóa đơn & Tiện ích", Note = "Tiền điện EVN tháng 10", IsValid = true, IsSelected = true }
            }
        };

        return Task.FromResult(mockResult);
    }

    public Task<ImportPreviewResultModel> PreviewUnstructuredTextAsync(string rawText)
    {
        var mockResult = new ImportPreviewResultModel
        {
            SourceType = "UnstructuredNote",
            TotalRows = 2,
            ValidRows = 2,
            InvalidRows = 0,
            TotalAmount = 95_000m,
            SummaryMessage = "Bóc tách thành công 2/2 dòng ghi chú tự do",
            Rows = new List<ParsedImportRowModel>
            {
                new() { RowIndex = 1, TransactionDate = DateTime.UtcNow, Amount = 45000m, TransactionType = "Expense", CategoryName = "Ăn uống", Note = "Cà phê Highlands 45k", IsValid = true, IsSelected = true },
                new() { RowIndex = 2, TransactionDate = DateTime.UtcNow, Amount = 50000m, TransactionType = "Expense", CategoryName = "Đi lại", Note = "Đổ xăng xe máy 50 ka", IsValid = true, IsSelected = true }
            }
        };

        return Task.FromResult(mockResult);
    }

    public Task<BulkCommitImportResultModel> CommitBulkImportAsync(BulkCommitImportRequestModel request)
    {
        var result = new BulkCommitImportResultModel(
            SuccessCount: request.SelectedRows.Count,
            TotalCommittedAmount: request.SelectedRows.Sum(r => r.Amount),
            WalletId: request.WalletId,
            Message: $"Nhập khẩu thành công {request.SelectedRows.Count} giao dịch vào ví!"
        );

        return Task.FromResult(result);
    }
}
