using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

// Controller managing multi-wallet lifecycle, ACID transfers and balance reconciliation
[Authorize]
public class WalletsController : BaseApiController
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    // Retrieves all active wallets for current user
    [HttpGet]
    public async Task<IActionResult> GetWallets(CancellationToken ct)
    {
        var result = await _walletService.GetUserWalletsAsync(CurrentUserId, ct);
        return HandleResult(result);
    }

    // Retrieves wallet details by identifier
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWalletById(Guid id, CancellationToken ct)
    {
        var result = await _walletService.GetWalletByIdAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    // Creates a new financial wallet
    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest request, CancellationToken ct)
    {
        var result = await _walletService.CreateWalletAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    // Updates wallet presentation and metadata
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWallet(Guid id, [FromBody] UpdateWalletRequest request, CancellationToken ct)
    {
        var result = await _walletService.UpdateWalletAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    // Partially updates wallet properties via HTTP PATCH (RFC 7396)
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchWallet(Guid id, [FromBody] PatchWalletRequest request, CancellationToken ct)
    {
        var result = await _walletService.PatchWalletAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    // Soft-deletes a wallet account
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWallet(Guid id, CancellationToken ct)
    {
        var result = await _walletService.DeleteWalletAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    // Executes atomic internal transfer between two wallets
    [HttpPost("transfer")]
    public async Task<IActionResult> TransferMoney([FromBody] TransferMoneyRequest request, CancellationToken ct)
    {
        var result = await _walletService.TransferMoneyAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }

    // Reconciles wallet balance against actual physical/account balance
    [HttpPost("{id:guid}/adjust")]
    public async Task<IActionResult> AdjustBalance(Guid id, [FromBody] AdjustBalanceRequest request, CancellationToken ct)
    {
        var result = await _walletService.AdjustBalanceAsync(CurrentUserId, id, request, ct);
        return HandleResult(result);
    }

    // Retrieves balance reconciliation history for a specific wallet
    [HttpGet("{id:guid}/adjustments")]
    public async Task<IActionResult> GetAdjustmentHistory(Guid id, CancellationToken ct)
    {
        var result = await _walletService.GetAdjustmentHistoryAsync(CurrentUserId, id, ct);
        return HandleResult(result);
    }

    // Extracts balance amount from mobile banking screenshot using OCR
    [HttpPost("scan-balance")]
    public async Task<IActionResult> ScanBalance(
        IFormFile? file,
        [FromServices] IBalanceOcrService ocrService,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ProblemDetails { Detail = "Vui lòng chọn ảnh chụp màn hình số dư ngân hàng." });

        await using var stream = file.OpenReadStream();
        var result = await ocrService.ScanBalanceAsync(stream, file.ContentType, ct);
        return HandleResult(result);
    }
}
