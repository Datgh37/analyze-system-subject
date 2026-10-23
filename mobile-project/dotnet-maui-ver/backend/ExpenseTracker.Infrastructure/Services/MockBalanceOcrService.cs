using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Infrastructure.Services;

// Mock OCR service parsing account balance screenshots for offline development and testing
public class MockBalanceOcrService : IBalanceOcrService
{
    public Task<Result<OcrBalanceScanResponse>> ScanBalanceAsync(Stream imageStream, string contentType, CancellationToken ct = default)
    {
        if (imageStream == null || imageStream.Length == 0)
            return Task.FromResult(Result<OcrBalanceScanResponse>.Failure("Tệp ảnh không hợp lệ hoặc rỗng."));

        // Simulated heuristic recognition of mobile banking screen
        var mockResponse = new OcrBalanceScanResponse(
            DetectedBalance: 12500000m,
            Confidence: 0.96,
            DetectedBankName: "Vietcombank Digital",
            DetectedAccountNumber: "0071001234567",
            RawText: "Số dư khả dụng: 12,500,000 VND\nTài khoản nguồn: 0071001234567\nNgân hàng TMCP Ngoại thương Việt Nam"
        );

        return Task.FromResult(Result<OcrBalanceScanResponse>.Success(mockResponse));
    }
}
