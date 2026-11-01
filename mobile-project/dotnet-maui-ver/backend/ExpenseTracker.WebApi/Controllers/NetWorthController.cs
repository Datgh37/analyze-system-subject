using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// Controller báo cáo và thống kê giá trị tài sản ròng đa chiều (UC-07.02)
[Authorize]
[Route("api/net-worth")]
public class NetWorthController : BaseApiController
{
    private readonly INetWorthService _netWorthService;
    private readonly ILogger<NetWorthController> _logger;

    public NetWorthController(
        INetWorthService netWorthService,
        ILogger<NetWorthController> logger)
    {
        _netWorthService = netWorthService;
        _logger = logger;
    }

    // Lấy bảng phân tích giá trị tài sản ròng thời gian thực (Assets - Liabilities, Debt-to-Asset, Runway)
    [HttpGet("current")]
    [ProducesResponseType(typeof(NetWorthBreakdownDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentNetWorth(CancellationToken ct)
    {
        var result = await _netWorthService.GetCurrentNetWorthAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // Lấy lịch sử biến động tài sản ròng theo thời gian
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<NetWorthHistoryItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNetWorthHistory([FromQuery] int limit = 12, CancellationToken ct = default)
    {
        var result = await _netWorthService.GetNetWorthHistoryAsync(CurrentUserId, limit, ct);
        return Ok(result);
    }

    // Ghi nhận một điểm ảnh (Snapshot) tài sản ròng thủ công hoặc định kỳ
    [HttpPost("snapshot")]
    [ProducesResponseType(typeof(NetWorthHistoryItemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RecordSnapshot([FromBody] RecordNetWorthSnapshotDto? request, CancellationToken ct)
    {
        var result = await _netWorthService.RecordSnapshotAsync(CurrentUserId, request, ct);
        return Ok(result);
    }
}

