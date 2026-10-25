using ExpenseTracker.Application.DTOs.ImportExport;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

public record UnstructuredTextImportRequest(string Text);

// API endpoints managing Hybrid Multi-source Financial Data Import (UC-07.06)
[Authorize]
[Route("api/data-import")]
public class DataImportController : BaseApiController
{
    private readonly IHybridDataImportService _importService;

    public DataImportController(IHybridDataImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("preview-spreadsheet")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportPreviewResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PreviewSpreadsheet(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ProblemDetails { Detail = "Vui lòng chọn tệp bảng tính Excel (.xlsx) hoặc CSV hợp lệ." });

        await using var stream = file.OpenReadStream();
        var result = await _importService.ParseSpreadsheetImportAsync(CurrentUserId, stream, file.FileName, ct);
        return HandleResult(result);
    }

    [HttpPost("preview-text")]
    [ProducesResponseType(typeof(ImportPreviewResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PreviewUnstructuredText([FromBody] UnstructuredTextImportRequest request, CancellationToken ct)
    {
        var result = await _importService.ParseUnstructuredTextImportAsync(CurrentUserId, request.Text, ct);
        return HandleResult(result);
    }

    [HttpPost("commit-bulk")]
    [ProducesResponseType(typeof(BulkCommitImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CommitBulk([FromBody] BulkCommitImportRequestDto request, CancellationToken ct)
    {
        var result = await _importService.CommitBulkImportAsync(CurrentUserId, request, ct);
        return HandleResult(result);
    }
}
