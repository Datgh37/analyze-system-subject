using ExpenseTracker.Application.DTOs.Debts;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// Controller quản lý danh mục nợ cá nhân, thanh toán nợ và động cơ tối ưu hóa (UC-06.05, UC-06.06 & UC-06.07)
[Authorize]
[Route("api/debts")]
public class DebtsController : BaseApiController
{
    private readonly IDebtService _debtService;
    private readonly IDebtOptimizerService _debtOptimizerService;
    private readonly ILogger<DebtsController> _logger;

    public DebtsController(
        IDebtService debtService, 
        IDebtOptimizerService debtOptimizerService,
        ILogger<DebtsController> logger)
    {
        _debtService = debtService;
        _debtOptimizerService = debtOptimizerService;
        _logger = logger;
    }

    // Lấy toàn bộ danh sách khoản nợ của người dùng hiện tại
    [HttpGet]
    [ProducesResponseType(typeof(List<DebtDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDebts(CancellationToken ct)
    {
        var result = await _debtService.GetDebtsAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // Lấy thông tin tổng hợp chỉ số nợ (Tổng dư nợ, tổng trả tối thiểu hàng tháng, số khoản active)
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DebtSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDebtSummary(CancellationToken ct)
    {
        var result = await _debtService.GetDebtSummaryAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // Lấy thông tin chi tiết khoản nợ kèm lịch sử các lần trả nợ
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DebtDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDebtById(Guid id, CancellationToken ct)
    {
        var result = await _debtService.GetDebtByIdAsync(id, CurrentUserId, ct);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy khoản nợ được yêu cầu." });
        }
        return Ok(result);
    }

    // Tạo mới một khoản nợ cá nhân
    [HttpPost]
    [ProducesResponseType(typeof(DebtDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDebt([FromBody] CreateDebtDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _debtService.CreateDebtAsync(CurrentUserId, dto, ct);
            return CreatedAtAction(nameof(GetDebtById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Lỗi xác thực khi tạo khoản nợ: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    // Cập nhật thông tin khoản nợ
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DebtDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDebt(Guid id, [FromBody] UpdateDebtDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _debtService.UpdateDebtAsync(id, CurrentUserId, dto, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Lỗi xác thực khi cập nhật khoản nợ {Id}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    // Ghi nhận thanh toán trả nợ (trừ tiền ví liên kết, cập nhật dư nợ)
    [HttpPost("{id:guid}/payments")]
    [ProducesResponseType(typeof(DebtTransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordDebtPaymentDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _debtService.RecordPaymentAsync(id, CurrentUserId, dto, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Vi phạm quy tắc nghiệp vụ khi trả nợ {Id}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Dữ liệu không hợp lệ khi trả nợ {Id}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    // Xóa mềm khoản nợ (Soft delete)
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveDebt(Guid id, CancellationToken ct)
    {
        var success = await _debtService.ArchiveDebtAsync(id, CurrentUserId, ct);
        if (!success)
        {
            return NotFound(new { message = "Không tìm thấy khoản nợ để lưu trữ." });
        }

        return Ok(new { success = true, message = "Khoản nợ đã được lưu trữ an toàn." });
    }

    // --- CÁC ENDPOINT TỐI ƯU HÓA TRẢ NỢ VÀ HƯỚNG DẪN BÌNH DÂN HÓA (UC-06.06) ---

    // Cung cấp cẩm nang hướng dẫn bình dân hóa tài chính, giải thích Snowball vs Avalanche và câu hỏi trắc nghiệm định hướng
    [HttpGet("optimization/guide")]
    [ProducesResponseType(typeof(DebtEducationalGuideDto), StatusCodes.Status200OK)]
    public IActionResult GetEducationalGuide()
    {
        var guide = _debtOptimizerService.GetEducationalGuide();
        return Ok(guide);
    }

    // Gợi ý ngân sách trả thêm an toàn dựa trên tình hình thu chi thực tế của người dùng
    [HttpGet("optimization/safe-budget")]
    [ProducesResponseType(typeof(SafeExtraBudgetRecommendationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSafeBudgetRecommendation(CancellationToken ct)
    {
        var result = await _debtOptimizerService.GetSafeExtraBudgetRecommendationAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // So sánh song song hai chiến lược Snowball vs Avalanche và đưa ra khuyến nghị hành vi đời thường
    [HttpPost("optimization/compare")]
    [ProducesResponseType(typeof(DebtPayoffComparisonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompareStrategies([FromBody] DebtPayoffSimulationRequestDto request, CancellationToken ct)
    {
        if (request.MonthlyExtraBudget < 0)
        {
            return BadRequest(new { message = "Ngân sách trả thêm hàng tháng không thể là số âm." });
        }

        var result = await _debtOptimizerService.CompareStrategiesAsync(CurrentUserId, request, ct);
        return Ok(result);
    }

    // Mô phỏng chi tiết một chiến lược cụ thể (strategy: snowball hoặc avalanche)
    [HttpPost("optimization/simulate")]
    [ProducesResponseType(typeof(DebtStrategySimulationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SimulateStrategy(
        [FromQuery] string strategy, 
        [FromBody] DebtPayoffSimulationRequestDto request, 
        CancellationToken ct)
    {
        if (request.MonthlyExtraBudget < 0)
        {
            return BadRequest(new { message = "Ngân sách trả thêm hàng tháng không thể là số âm." });
        }

        string normalizedStrategy = strategy?.Trim().ToUpperInvariant() ?? "SNOWBALL";
        if (normalizedStrategy != "SNOWBALL" && normalizedStrategy != "AVALANCHE")
        {
            return BadRequest(new { message = "Chiến lược không hợp lệ. Chỉ chấp nhận 'snowball' hoặc 'avalanche'." });
        }

        DebtStrategySimulationResultDto result;
        if (normalizedStrategy == "SNOWBALL")
        {
            result = await _debtOptimizerService.SimulateSnowballAsync(CurrentUserId, request.MonthlyExtraBudget, request.CustomDebts, ct);
        }
        else
        {
            result = await _debtOptimizerService.SimulateAvalancheAsync(CurrentUserId, request.MonthlyExtraBudget, request.CustomDebts, ct);
        }

        return Ok(result);
    }
}

