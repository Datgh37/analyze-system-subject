using ExpenseTracker.Application.DTOs.Receipts;
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service interface for receipt AI Vision OCR parsing, math audit, and ledger confirmation (UC-03.07/03.08)
public interface IReceiptOcrService
{
    Task<Result<ReceiptParsedDataDto>> ParseReceiptAsync(Guid userId, Stream fileStream, string fileName, CancellationToken ct = default);
    Task<Result<TransactionDto>> ConfirmReceiptAsync(Guid userId, ConfirmReceiptRequest request, CancellationToken ct = default);
}
