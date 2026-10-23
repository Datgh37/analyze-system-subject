using ExpenseTracker.Application.DTOs.Payslips;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// API endpoints managing AI Vision OCR payslip parsing, mathematical audit, and ledger persistence (UC-02.08)
[Authorize]
public class PayslipsController : BaseApiController
{
    private readonly IPayslipOcrService _payslipOcrService;

    public PayslipsController(IPayslipOcrService payslipOcrService)
    {
        _payslipOcrService = payslipOcrService;
    }

    [HttpPost("parse")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PayslipParsedDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ParsePayslip(IFormFile? file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "Vui lòng chọn tệp hình ảnh hoặc tài liệu PDF phiếu lương để bóc tách."
            });
        }

        await using var stream = file.OpenReadStream();
        var result = await _payslipOcrService.ParsePayslipAsync(stream, file.FileName, ct);
        return HandleResult(result);
    }

    [HttpPost("confirm")]
    [ProducesResponseType(typeof(SalarySlipDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmPayslip([FromBody] ConfirmPayslipRequest request, CancellationToken ct)
    {
        var result = await _payslipOcrService.ConfirmAndSavePayslipAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    [HttpGet("transaction/{transactionId:guid}")]
    [ProducesResponseType(typeof(SalarySlipDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayslipDetail(Guid transactionId, CancellationToken ct)
    {
        var result = await _payslipOcrService.GetSalarySlipDetailByTransactionIdAsync(CurrentUserId, transactionId, ct);
        return HandleResult(result);
    }
}
