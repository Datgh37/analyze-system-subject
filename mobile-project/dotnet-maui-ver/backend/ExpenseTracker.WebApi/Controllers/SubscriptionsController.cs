using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// Controller quản lý danh mục thuê bao định kỳ và rà soát bẫy gia hạn ngầm (UC-03.09)
[Authorize]
[Route("api/subscriptions")]
public class SubscriptionsController : BaseApiController
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(
        ISubscriptionService subscriptionService,
        ILogger<SubscriptionsController> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    // Lấy danh sách thuê bao của người dùng (tùy chọn lọc theo trạng thái)
    [HttpGet]
    [ProducesResponseType(typeof(List<SubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscriptions([FromQuery] SubscriptionStatus? status, CancellationToken ct)
    {
        var result = await _subscriptionService.GetSubscriptionsAsync(CurrentUserId, status, ct);
        return Ok(result);
    }

    // Lấy thông tin chi tiết một gói thuê bao
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubscriptionById(Guid id, CancellationToken ct)
    {
        var result = await _subscriptionService.GetSubscriptionByIdAsync(CurrentUserId, id, ct);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy thuê bao dịch vụ yêu cầu." });

        return Ok(result);
    }

    // Tạo mới một thuê bao theo dõi
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto request, CancellationToken ct)
    {
        var result = await _subscriptionService.CreateSubscriptionAsync(CurrentUserId, request, ct);
        return CreatedAtAction(nameof(GetSubscriptionById), new { id = result.Id }, result);
    }

    // Cập nhật thông tin thuê bao
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] UpdateSubscriptionDto request, CancellationToken ct)
    {
        var result = await _subscriptionService.UpdateSubscriptionAsync(CurrentUserId, id, request, ct);
        return Ok(result);
    }

    // Hủy theo dõi / Xóa mềm thuê bao
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveSubscription(Guid id, CancellationToken ct)
    {
        var result = await _subscriptionService.ArchiveSubscriptionAsync(CurrentUserId, id, ct);
        return Ok(result);
    }

    // Kiểm toán toàn diện bẫy thuê bao lặp lại và rò rỉ dòng tiền
    [HttpGet("audit")]
    [ProducesResponseType(typeof(SubscriptionAuditReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AuditSubscriptions(CancellationToken ct)
    {
        var result = await _subscriptionService.AuditSubscriptionsAsync(CurrentUserId, ct);
        return Ok(result);
    }

    // Chuyển đổi ứng viên phát hiện thành thuê bao quản trị chính thức
    [HttpPost("convert")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertCandidate([FromBody] CreateSubscriptionDto request, CancellationToken ct)
    {
        var result = await _subscriptionService.ConvertCandidateToSubscriptionAsync(CurrentUserId, request, ct);
        return Ok(result);
    }

    // Lấy danh sách thuê bao sắp đến hạn thanh toán trong N ngày tới
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(List<UpcomingRenewalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingRenewals([FromQuery] int daysAhead = 7, CancellationToken ct = default)
    {
        var result = await _subscriptionService.GetUpcomingRenewalsAsync(CurrentUserId, daysAhead, ct);
        return Ok(result);
    }
}

