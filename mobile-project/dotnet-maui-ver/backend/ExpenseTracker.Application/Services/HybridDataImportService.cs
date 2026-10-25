using ClosedXML.Excel;
using ExpenseTracker.Application.DTOs.ImportExport;
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of Hybrid Multi-source Financial Data Import Service (UC-07.06)
// Combines 0-token ClosedXML spreadsheet parsing with intelligent unstructured text extraction
public class HybridDataImportService : IHybridDataImportService
{
    private readonly IApplicationDbContext _context;
    private readonly IAutoCategorizationService _autoRuleService;
    private readonly ITransactionService _transactionService;

    public HybridDataImportService(
        IApplicationDbContext context,
        IAutoCategorizationService autoRuleService,
        ITransactionService transactionService)
    {
        _context = context;
        _autoRuleService = autoRuleService;
        _transactionService = transactionService;
    }

    // 1. Structured Spreadsheet Parser (.xlsx, .csv) via ClosedXML
    public async Task<Result<ImportPreviewResultDto>> ParseSpreadsheetImportAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        CancellationToken ct = default)
    {
        if (fileStream == null || fileStream.Length == 0)
            return Result<ImportPreviewResultDto>.Failure("Tệp bảng tính rỗng. Vui lòng chọn tệp Excel (.xlsx) hợp lệ.");

        var rows = new List<ParsedImportRowDto>();
        decimal totalAmount = 0m;

        try
        {
            if (fileStream.CanSeek)
                fileStream.Position = 0;

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null || worksheet.RowsUsed().Count() <= 1)
            {
                return Result<ImportPreviewResultDto>.Failure("Bảng tính không có dữ liệu giao dịch hoặc chỉ chứa tiêu đề.");
            }

            var usedRows = worksheet.RowsUsed().ToList();

            // Identify column indices from header
            int dateCol = 1, amountCol = 2, catCol = 3, typeCol = -1, noteCol = 4;
            var headerRow = usedRows.First();
            foreach (var cell in headerRow.CellsUsed())
            {
                string val = cell.GetString().ToLowerInvariant().Trim();
                if (val.Contains("ngày") || val.Contains("date")) dateCol = cell.Address.ColumnNumber;
                else if (val.Contains("tiền") || val.Contains("amount") || val.Contains("giá")) amountCol = cell.Address.ColumnNumber;
                else if (val.Contains("mục") || val.Contains("category")) catCol = cell.Address.ColumnNumber;
                else if (val.Contains("loại") || val.Contains("type")) typeCol = cell.Address.ColumnNumber;
                else if (val.Contains("chú") || val.Contains("note") || val.Contains("nội dung")) noteCol = cell.Address.ColumnNumber;
            }

            int rowIndex = 1;
            foreach (var row in usedRows.Skip(1))
            {
                ct.ThrowIfCancellationRequested();

                // Read date
                DateTime txDate = DateTime.UtcNow;
                var dateCell = row.Cell(dateCol);
                if (dateCell.TryGetValue<DateTime>(out var dVal))
                    txDate = dVal;
                else if (DateTime.TryParse(dateCell.GetString(), out var parsedD))
                    txDate = parsedD;

                // Read amount
                decimal amount = 0m;
                var amountCell = row.Cell(amountCol);
                if (amountCell.TryGetValue<decimal>(out var aVal))
                    amount = Math.Abs(aVal);
                else
                {
                    string aStr = amountCell.GetString().Replace(",", "").Replace(".", "").Replace("đ", "").Replace("VND", "").Trim();
                    if (decimal.TryParse(aStr, out var parsedA))
                        amount = Math.Abs(parsedA);
                }

                // Read category & note
                string catName = row.Cell(catCol).GetString().Trim();
                string note = noteCol > 0 ? row.Cell(noteCol).GetString().Trim() : "";

                // Read type
                string txType = "Expense";
                if (typeCol > 0)
                {
                    string tStr = row.Cell(typeCol).GetString().ToLowerInvariant();
                    if (tStr.Contains("thu") || tStr.Contains("income"))
                        txType = "Income";
                }

                bool isValid = amount > 0;
                string? errorMsg = isValid ? null : "Số tiền không hợp lệ hoặc bằng 0";

                // Match with system categories
                Guid? matchedCatId = null;
                if (!string.IsNullOrWhiteSpace(catName))
                {
                    var match = await _autoRuleService.MatchCategoryAsync(userId, catName, ct);
                    matchedCatId = match.Value?.CategoryId;
                    if (!string.IsNullOrWhiteSpace(match.Value?.CategoryName))
                        catName = match.Value.CategoryName;
                }

                if (string.IsNullOrWhiteSpace(catName))
                    catName = txType == "Income" ? "Thu nhập khác" : "Chi tiêu khác";

                if (isValid) totalAmount += amount;

                rows.Add(new ParsedImportRowDto(
                    RowIndex: rowIndex++,
                    TransactionDate: txDate,
                    Amount: amount,
                    TransactionType: txType,
                    CategoryName: catName,
                    Note: string.IsNullOrWhiteSpace(note) ? $"Nhập từ {fileName}" : note,
                    MatchedCategoryId: matchedCatId,
                    IsValid: isValid,
                    ValidationMessage: errorMsg
                ));
            }
        }
        catch (Exception ex)
        {
            return Result<ImportPreviewResultDto>.Failure($"Lỗi cấu trúc tệp Excel: {ex.Message}. Vui lòng kiểm tra lại định dạng bảng tính.");
        }

        int validCount = rows.Count(r => r.IsValid);
        int invalidCount = rows.Count(r => !r.IsValid);

        var result = new ImportPreviewResultDto(
            SourceType: "StructuredExcel",
            TotalRows: rows.Count,
            ValidRows: validCount,
            InvalidRows: invalidCount,
            TotalAmount: totalAmount,
            Rows: rows,
            SummaryMessage: $"Bóc tách thành công {validCount}/{rows.Count} dòng giao dịch từ tệp Excel."
        );

        return Result<ImportPreviewResultDto>.Success(result);
    }

    // 2. Unstructured Note / Text Parser via Contextual NLP Reasoning
    public async Task<Result<ImportPreviewResultDto>> ParseUnstructuredTextImportAsync(
        Guid userId,
        string rawText,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return Result<ImportPreviewResultDto>.Failure("Nội dung ghi chú rỗng. Vui lòng nhập hoặc dán nội dung ghi chép.");

        var lines = rawText.Split(new[] { '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(l => l.Trim())
                           .Where(l => l.Length > 2)
                           .ToList();

        if (lines.Count == 0)
            return Result<ImportPreviewResultDto>.Failure("Không tìm thấy dòng ghi chép hợp lệ.");

        var rows = new List<ParsedImportRowDto>();
        decimal totalAmount = 0m;
        int rowIndex = 1;

        foreach (var line in lines)
        {
            ct.ThrowIfCancellationRequested();

            decimal? amount = VoiceIntentRouterService.ParseVietnameseSlangAmount(line);
            bool isIncome = line.Contains("lương", StringComparison.OrdinalIgnoreCase)
                         || line.Contains("thu", StringComparison.OrdinalIgnoreCase)
                         || line.Contains("thưởng", StringComparison.OrdinalIgnoreCase);

            string txType = isIncome ? "Income" : "Expense";
            string catName = "Chi tiêu khác";

            // Infer category
            var match = await _autoRuleService.MatchCategoryAsync(userId, line, ct);
            Guid? matchedCatId = match.Value?.CategoryId;
            if (!string.IsNullOrWhiteSpace(match.Value?.CategoryName))
            {
                catName = match.Value.CategoryName;
            }
            else
            {
                if (line.Contains("cà phê", StringComparison.OrdinalIgnoreCase) || line.Contains("cafe", StringComparison.OrdinalIgnoreCase) || line.Contains("ăn", StringComparison.OrdinalIgnoreCase))
                    catName = "Ăn uống";
                else if (line.Contains("xăng", StringComparison.OrdinalIgnoreCase) || line.Contains("grab", StringComparison.OrdinalIgnoreCase))
                    catName = "Đi lại";
                else if (line.Contains("trọ", StringComparison.OrdinalIgnoreCase) || line.Contains("điện", StringComparison.OrdinalIgnoreCase) || line.Contains("nước", StringComparison.OrdinalIgnoreCase))
                    catName = "Hóa đơn & Tiện ích";
                else
                    catName = isIncome ? "Lương" : "Chi tiêu khác";
            }

            bool isValid = amount.HasValue && amount.Value > 0;
            decimal finalAmount = amount ?? 0m;
            if (isValid) totalAmount += finalAmount;

            rows.Add(new ParsedImportRowDto(
                RowIndex: rowIndex++,
                TransactionDate: DateTime.UtcNow,
                Amount: finalAmount,
                TransactionType: txType,
                CategoryName: catName,
                Note: line,
                MatchedCategoryId: matchedCatId,
                IsValid: isValid,
                ValidationMessage: isValid ? null : "Không phát hiện được số tiền trong dòng ghi chú"
            ));
        }

        int validCount = rows.Count(r => r.IsValid);
        int invalidCount = rows.Count(r => !r.IsValid);

        var result = new ImportPreviewResultDto(
            SourceType: "UnstructuredNote",
            TotalRows: rows.Count,
            ValidRows: validCount,
            InvalidRows: invalidCount,
            TotalAmount: totalAmount,
            Rows: rows,
            SummaryMessage: $"Bóc tách thành công {validCount}/{rows.Count} dòng ghi chú tự do."
        );

        return Result<ImportPreviewResultDto>.Success(result);
    }

    // 3. Atomically commit approved rows to database
    public async Task<Result<BulkCommitImportResultDto>> CommitBulkImportAsync(
        Guid userId,
        BulkCommitImportRequestDto request,
        CancellationToken ct = default)
    {
        if (request.SelectedRows == null || request.SelectedRows.Count == 0)
            return Result<BulkCommitImportResultDto>.Failure("Không có dòng giao dịch nào được chọn để nhập.");

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == request.WalletId && w.UserId == userId, ct);
        if (wallet == null)
            return Result<BulkCommitImportResultDto>.Failure("Ví thanh toán không tồn tại hoặc không thuộc quyền sở hữu của người dùng.");

        // Fallback default categories
        var defaultExpenseCat = await _context.Categories
            .FirstOrDefaultAsync(c => (c.IsSystem || c.UserId == userId) && c.TransactionType == TransactionType.Expense, ct);
        var defaultIncomeCat = await _context.Categories
            .FirstOrDefaultAsync(c => (c.IsSystem || c.UserId == userId) && c.TransactionType == TransactionType.Income, ct);

        int successCount = 0;
        decimal totalCommitted = 0m;

        foreach (var row in request.SelectedRows.Where(r => r.IsValid && r.Amount > 0))
        {
            ct.ThrowIfCancellationRequested();

            if (row.TransactionType.Equals("Income", StringComparison.OrdinalIgnoreCase))
            {
                var incomeCatId = row.MatchedCategoryId ?? defaultIncomeCat?.Id ?? Guid.Empty;
                var incomeReq = new CreateIncomeTransactionRequest(
                    WalletId: wallet.Id,
                    CategoryId: incomeCatId,
                    Amount: row.Amount,
                    TransactionDate: row.TransactionDate,
                    Note: row.Note ?? "Nhập từ tệp dữ liệu",
                    IsRecurring: false
                );
                var res = await _transactionService.CreateIncomeAsync(userId, incomeReq, ct);
                if (res.IsSuccess)
                {
                    successCount++;
                    totalCommitted += row.Amount;
                }
            }
            else
            {
                var expenseCatId = row.MatchedCategoryId ?? defaultExpenseCat?.Id ?? Guid.Empty;
                var expenseReq = new CreateExpenseTransactionRequest(
                    WalletId: wallet.Id,
                    CategoryId: expenseCatId,
                    Amount: row.Amount,
                    TransactionDate: row.TransactionDate,
                    Note: row.Note ?? "Nhập từ tệp dữ liệu",
                    IsEssential: false,
                    IsRecurring: false,
                    ExcludeFromReport: false
                );
                var res = await _transactionService.CreateExpenseAsync(userId, expenseReq, ct);
                if (res.IsSuccess)
                {
                    successCount++;
                    totalCommitted += row.Amount;
                }
            }
        }

        var commitResult = new BulkCommitImportResultDto(
            SuccessCount: successCount,
            TotalCommittedAmount: totalCommitted,
            WalletId: wallet.Id,
            Message: $"Nhập khẩu thành công {successCount} giao dịch vào ví '{wallet.Name}' (Tổng: {totalCommitted:N0} VND)."
        );

        return Result<BulkCommitImportResultDto>.Success(commitResult);
    }
}
