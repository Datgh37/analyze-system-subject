namespace ExpenseTracker.Maui.Models.ImportExport;

public class ParsedImportRowModel
{
    public int RowIndex { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = "Expense";
    public string CategoryName { get; set; } = string.Empty;
    public string? Note { get; set; }
    public Guid? MatchedCategoryId { get; set; }
    public bool IsValid { get; set; }
    public string? ValidationMessage { get; set; }
    public bool IsSelected { get; set; } = true;
}

public class ImportPreviewResultModel
{
    public string SourceType { get; set; } = "StructuredExcel";
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ParsedImportRowModel> Rows { get; set; } = new();
    public string SummaryMessage { get; set; } = string.Empty;
}

public record BulkCommitImportRequestModel(
    Guid WalletId,
    List<ParsedImportRowModel> SelectedRows
);

public record BulkCommitImportResultModel(
    int SuccessCount,
    decimal TotalCommittedAmount,
    Guid WalletId,
    string Message
);
