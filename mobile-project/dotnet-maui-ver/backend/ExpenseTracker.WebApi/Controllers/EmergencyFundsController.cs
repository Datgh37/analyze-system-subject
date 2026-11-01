using ExpenseTracker.Application.DTOs.EmergencyFund;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// Controller quản lý Quỹ Dự Phòng Khẩn Cấp và ma sát tâm lý rút tiền (UC-06.04)
[Authorize]
[Route("api/emergency-funds")]
public class EmergencyFundsController : BaseApiController
{
    private readonly IEmergencyFundService _emergencyFundService;
    private readonly ILogger<EmergencyFundsController> _logger;

    public EmergencyFundsController(
        IEmergencyFundService emergencyFundService,
        ILogger<EmergencyFundsController> logger)
    {
        _emergencyFundService = emergencyFundService;
        _logger = logger;
    }

    // Lấy khuyến nghị quy mô quỹ khẩn cấp 3-6-12 tháng dựa trên chi phí thiết yếu lịch sử
    [HttpGet("recommendation")]
    [ProducesResponseType(typeof(EmergencyFundRecommendationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecommendation(CancellationToken ct)
    {
        var result = await _emergencyFundService.GetRecommendationAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // Lấy trạng thái hiện tại của Quỹ Khẩn Cấp và số tháng sinh tồn dự phòng được
    [HttpGet("status")]
    [ProducesResponseType(typeof(EmergencyFundStatusDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken ct)
    {
        var result = await _emergencyFundService.GetStatusAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // Thiết lập hoặc cập nhật mục tiêu Quỹ Khẩn Cấp
    [HttpPost("setup")]
    [ProducesResponseType(typeof(EmergencyFundStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetupFund([FromBody] SetupEmergencyFundDto request, CancellationToken ct)
    {
        var result = await _emergencyFundService.SetupFundAsync(CurrentUserId, request, ct);
        return Ok(result);
    }

    // Nạp tiền tích lũy vào Quỹ Khẩn Cấp từ một ví nguồn
    [HttpPost("deposit")]
    [ProducesResponseType(typeof(EmergencyFundStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deposit([FromBody] DepositEmergencyFundDto request, CancellationToken ct)
    {
        var result = await _emergencyFundService.DepositAsync(CurrentUserId, request, ct);
        return Ok(result);
    }

    // Rút tiền từ Quỹ Khẩn Cấp kèm ma sát nhận thức và lý do khẩn cấp (Richard Thaler Nudge)
    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(EmergencyFundStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawEmergencyFundDto request, CancellationToken ct)
    {
        var result = await _emergencyFundService.WithdrawAsync(CurrentUserId, request, ct);
        return Ok(result);
    }
}

