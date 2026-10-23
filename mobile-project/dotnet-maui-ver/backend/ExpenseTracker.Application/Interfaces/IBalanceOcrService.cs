using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service interface extracting balance information from mobile banking screenshots via OCR
public interface IBalanceOcrService
{
    Task<Result<OcrBalanceScanResponse>> ScanBalanceAsync(Stream imageStream, string contentType, CancellationToken ct = default);
}
