using ExpenseTracker.Application.DTOs.Receipts;
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing Multimodal AI Vision OCR receipt parsing and ledger confirmation (UC-03.07/03.08)
[Authorize]
[Route("api/receipts")]
public class ReceiptsController : BaseApiController
{
    private readonly IReceiptOcrService _receiptOcrService;

    public ReceiptsController(IReceiptOcrService receiptOcrService)
    {
        _receiptOcrService = receiptOcrService;
    }

    [HttpPost("parse")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ReceiptParsedDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ParseReceipt([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ProblemDetails { Detail = "Vui lòng đính kèm tệp ảnh hoặc PDF hóa đơn hợp lệ." });

        await using var stream = file.OpenReadStream();
        var result = await _receiptOcrService.ParseReceiptAsync(CurrentUserId, stream, file.FileName, ct);
        return HandleResult(result);
    }

    [HttpPost("confirm")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmReceipt([FromBody] ConfirmReceiptRequest request, CancellationToken ct)
    {
        var result = await _receiptOcrService.ConfirmReceiptAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }
}
