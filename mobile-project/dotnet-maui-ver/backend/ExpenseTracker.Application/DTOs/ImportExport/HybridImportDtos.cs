namespace ExpenseTracker.Application.DTOs.ImportExport;

// A single parsed transaction row from either Excel or Unstructured Text/Note
public record ParsedImportRowDto(
    int RowIndex,
    DateTime TransactionDate,
    decimal Amount,
    string TransactionType,             // "Expense" or "Income"
    string CategoryName,
    string? Note,
    Guid? MatchedCategoryId,
    bool IsValid,
    string? ValidationMessage
);

// Summary result for the Human-In-The-Loop (HITL) Preview screen
public record ImportPreviewResultDto(
    string SourceType,                  // "StructuredExcel" or "UnstructuredNote"
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    decimal TotalAmount,
    List<ParsedImportRowDto> Rows,
    string SummaryMessage
);

// Payload submitted by user after reviewing/editing on Preview screen
public record BulkCommitImportRequestDto(
    Guid WalletId,
    List<ParsedImportRowDto> SelectedRows
);

// Result of committing bulk transactions to ACID database
public record BulkCommitImportResultDto(
    int SuccessCount,
    decimal TotalCommittedAmount,
    Guid WalletId,
    string Message
);
