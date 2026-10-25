using ClosedXML.Excel;
using ExpenseTracker.Application.DTOs.AiAgent;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of Financial Report Export Service (UC-07.01 Alternative Flow A1)
// Generates both Fast Raw Data Excel and AI-Decorated Smart Executive Financial Reports
// Powered by Google Gemini 2.5 Flash NLP acting as Behavioral Finance Advisor (Prof. Richard Thaler)
public class ReportExportService : IReportExportService
{
    private readonly IApplicationDbContext _context;
    private readonly IGeminiApiClient? _geminiClient;

    public ReportExportService(
        IApplicationDbContext context,
        IGeminiApiClient? geminiClient = null)
    {
        _context = context;
        _geminiClient = geminiClient;
    }

    // 1. Raw Transactions Excel Export (Fast, 0 tokens)
    public async Task<Result<ExportResultDto>> ExportRawTransactionsExcelAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken ct = default)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Wallet)
            .Where(t => t.UserId == userId && t.TransactionDate >= startDate && t.TransactionDate < endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);

        decimal totalExpense = transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
        decimal totalIncome = transactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
        decimal netSavings = totalIncome - totalExpense;

        using var workbook = new XLWorkbook();

        // Sheet 1: Tổng quan
        var summarySheet = workbook.Worksheets.Add("TongQuan");
        summarySheet.Cell("A1").Value = $"BÁO CÁO THU CHI THÁNG {month:D2}/{year}";
        summarySheet.Cell("A1").Style.Font.Bold = true;
        summarySheet.Cell("A1").Style.Font.FontSize = 14;
        summarySheet.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#1E3A8A"); // Navy

        summarySheet.Cell("A3").Value = "Chỉ số tài chính";
        summarySheet.Cell("B3").Value = "Số tiền (VND)";
        summarySheet.Row(3).Style.Font.Bold = true;
        summarySheet.Row(3).Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");

        summarySheet.Cell("A4").Value = "Tổng thu nhập:";
        summarySheet.Cell("B4").Value = totalIncome;
        summarySheet.Cell("B4").Style.NumberFormat.Format = "#,##0";

        summarySheet.Cell("A5").Value = "Tổng chi tiêu:";
        summarySheet.Cell("B5").Value = totalExpense;
        summarySheet.Cell("B5").Style.NumberFormat.Format = "#,##0";

        summarySheet.Cell("A6").Value = "Dòng tiền thặng dư:";
        summarySheet.Cell("B6").Value = netSavings;
        summarySheet.Cell("B6").Style.NumberFormat.Format = "#,##0";
        summarySheet.Cell("B6").Style.Font.Bold = true;
        summarySheet.Cell("B6").Style.Font.FontColor = netSavings >= 0 ? XLColor.Green : XLColor.Red;

        summarySheet.Columns().AdjustToContents();

        // Sheet 2: Danh sách giao dịch
        var txSheet = workbook.Worksheets.Add("ChiTietGiaoDich");
        txSheet.Cell("A1").Value = "Ngày";
        txSheet.Cell("B1").Value = "Loại";
        txSheet.Cell("C1").Value = "Danh mục";
        txSheet.Cell("D1").Value = "Số tiền (VND)";
        txSheet.Cell("E1").Value = "Ví thanh toán";
        txSheet.Cell("F1").Value = "Ghi chú";
        txSheet.Row(1).Style.Font.Bold = true;
        txSheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A");
        txSheet.Row(1).Style.Font.FontColor = XLColor.White;

        int rowIdx = 2;
        foreach (var tx in transactions)
        {
            txSheet.Cell(rowIdx, 1).Value = tx.TransactionDate.ToString("yyyy-MM-dd HH:mm");
            txSheet.Cell(rowIdx, 2).Value = tx.TransactionType == TransactionType.Income ? "Thu nhập" : "Chi tiêu";
            txSheet.Cell(rowIdx, 3).Value = tx.Category?.Name ?? "Khác";
            txSheet.Cell(rowIdx, 4).Value = tx.Amount;
            txSheet.Cell(rowIdx, 4).Style.NumberFormat.Format = "#,##0";
            txSheet.Cell(rowIdx, 5).Value = tx.Wallet?.Name ?? "Ví";
            txSheet.Cell(rowIdx, 6).Value = tx.Note;
            rowIdx++;
        }

        txSheet.Columns().AdjustToContents();

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        byte[] bytes = memoryStream.ToArray();

        var result = new ExportResultDto(
            FileBytes: bytes,
            ContentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            FileName: $"BaoCao_TaiChinh_{month:D2}_{year}.xlsx"
        );

        return Result<ExportResultDto>.Success(result);
    }

    // 2. AI-Decorated Smart Executive Financial Report (Richard Thaler Advisor powered by Gemini NLP)
    public async Task<Result<ExportResultDto>> ExportSmartExecutiveReportAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken ct = default)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Wallet)
            .Where(t => t.UserId == userId && t.TransactionDate >= startDate && t.TransactionDate < endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);

        decimal totalExpense = transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
        decimal totalIncome = transactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
        decimal netSavings = totalIncome - totalExpense;
        decimal savingsRate = totalIncome > 0 ? (netSavings / totalIncome) * 100m : 0m;
        string healthStatus = savingsRate >= 20m ? "Xuất sắc (Thặng dư cao)" : (savingsRate >= 0 ? "Ổn định" : "Cần lưu tâm (Thâm hụt nhẹ)");

        // Top 3 Expense Categories for context-aware AI advisory
        var topExpenseCategories = transactions
            .Where(t => t.TransactionType == TransactionType.Expense)
            .GroupBy(t => t.Category?.Name ?? "Khác")
            .OrderByDescending(g => g.Sum(x => x.Amount))
            .Take(3)
            .Select(g => $"{g.Key} ({g.Sum(x => x.Amount):N0} VND)")
            .ToList();
        string topCatSummary = topExpenseCategories.Any() ? string.Join(", ", topExpenseCategories) : "Chưa có chi tiêu";

        // Generate Richard Thaler Empathetic Commentary via Gemini NLP or Safe Fallback
        string commentary = await GenerateThalerAdvisorCommentaryAsync(month, year, totalIncome, totalExpense, netSavings, savingsRate, topCatSummary, ct);

        using var workbook = new XLWorkbook();

        // Sheet 1: ⭐ Nhận định điều hành Cố vấn AI
        var aiSheet = workbook.Worksheets.Add("⭐ TongQuan_CoVanAI");
        aiSheet.Cell("A1").Value = "BẢN NHẬN ĐỊNH ĐIỀU HÀNH TÀI CHÍNH CÁ NHÂN (AI ADVISOR SUMMARY)";
        aiSheet.Cell("A1").Style.Font.Bold = true;
        aiSheet.Cell("A1").Style.Font.FontSize = 14;
        aiSheet.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#1E3A8A");

        aiSheet.Cell("A2").Value = $"Kỳ phân tích: Tháng {month:D2}/{year} | Trợ lý Cố vấn: Richard Thaler Empathetic Engine";
        aiSheet.Cell("A2").Style.Font.Italic = true;
        aiSheet.Cell("A2").Style.Font.FontColor = XLColor.Gray;

        // Executive Box
        aiSheet.Range("A4:F7").Merge();
        var execBox = aiSheet.Cell("A4");
        execBox.Value = $"\" {commentary} \"";
        execBox.Style.Font.Italic = true;
        execBox.Style.Font.FontSize = 11;
        execBox.Style.Alignment.WrapText = true;
        execBox.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFF6FF"); // Light Blue
        execBox.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        execBox.Style.Border.OutsideBorderColor = XLColor.FromHtml("#3B82F6");

        // KPI Cards
        aiSheet.Cell("A9").Value = "CHỈ SỐ TÀI CHÍNH CỐT LÕI";
        aiSheet.Cell("A9").Style.Font.Bold = true;

        aiSheet.Cell("A10").Value = "Tình trạng sức khỏe:";
        aiSheet.Cell("B10").Value = healthStatus;
        aiSheet.Cell("B10").Style.Font.Bold = true;

        aiSheet.Cell("A11").Value = "Tổng thu nhập:";
        aiSheet.Cell("B11").Value = totalIncome;
        aiSheet.Cell("B11").Style.NumberFormat.Format = "#,##0 VND";

        aiSheet.Cell("A12").Value = "Tổng chi tiêu:";
        aiSheet.Cell("B12").Value = totalExpense;
        aiSheet.Cell("B12").Style.NumberFormat.Format = "#,##0 VND";

        aiSheet.Cell("A13").Value = "Tỷ lệ tiết kiệm (Savings Rate):";
        aiSheet.Cell("B13").Value = $"{savingsRate:F1}%";
        aiSheet.Cell("B13").Style.Font.Bold = true;
        aiSheet.Cell("B13").Style.Font.FontColor = savingsRate >= 20m ? XLColor.Green : XLColor.FromHtml("#D97706");

        aiSheet.Columns().AdjustToContents();

        // Sheet 2: Danh sách giao dịch
        var txSheet = workbook.Worksheets.Add("ChiTietGiaoDich");
        txSheet.Cell("A1").Value = "Ngày";
        txSheet.Cell("B1").Value = "Loại";
        txSheet.Cell("C1").Value = "Danh mục";
        txSheet.Cell("D1").Value = "Số tiền (VND)";
        txSheet.Cell("E1").Value = "Ví thanh toán";
        txSheet.Cell("F1").Value = "Ghi chú";
        txSheet.Row(1).Style.Font.Bold = true;
        txSheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A");
        txSheet.Row(1).Style.Font.FontColor = XLColor.White;

        int rowIdx = 2;
        foreach (var tx in transactions)
        {
            txSheet.Cell(rowIdx, 1).Value = tx.TransactionDate.ToString("yyyy-MM-dd HH:mm");
            txSheet.Cell(rowIdx, 2).Value = tx.TransactionType == TransactionType.Income ? "Thu nhập" : "Chi tiêu";
            txSheet.Cell(rowIdx, 3).Value = tx.Category?.Name ?? "Khác";
            txSheet.Cell(rowIdx, 4).Value = tx.Amount;
            txSheet.Cell(rowIdx, 4).Style.NumberFormat.Format = "#,##0";
            txSheet.Cell(rowIdx, 5).Value = tx.Wallet?.Name ?? "Ví";
            txSheet.Cell(rowIdx, 6).Value = tx.Note;
            rowIdx++;
        }

        txSheet.Columns().AdjustToContents();

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        byte[] bytes = memoryStream.ToArray();

        var result = new ExportResultDto(
            FileBytes: bytes,
            ContentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            FileName: $"BaoCao_CoVanAI_{month:D2}_{year}.xlsx"
        );

        return Result<ExportResultDto>.Success(result);
    }

    private async Task<string> GenerateThalerAdvisorCommentaryAsync(
        int month,
        int year,
        decimal totalIncome,
        decimal totalExpense,
        decimal netSavings,
        decimal savingsRate,
        string topCatSummary,
        CancellationToken ct)
    {
        if (_geminiClient != null)
        {
            try
            {
                const string systemPrompt = "Bạn là Giáo sư Richard Thaler, Chuyên gia Kinh tế Hành vi (Nobel Kinh Tế 2017) và Cố vấn Tài chính Cá nhân Cấp cao (Empathetic Behavioral Finance Advisor). "
                    + "Dựa trên số liệu tài chính tháng của người dùng, hãy viết một đoạn nhận định điều hành tài chính cá nhân (khoảng 3-4 câu) với văn phong chuyên nghiệp, ấm áp, đồng cảm, không phán xét. Đưa ra 1 cú hích hành vi (Nudge) cụ thể, nhẹ nhàng giúp người dùng duy trì kỷ luật hoặc giảm bớt chi tiêu ngoài kế hoạch mà không gây ma sát tâm lý.\n"
                    + "YÊU CẦU: Trả về DUY NHẤT một đoạn văn bản ngắn gọn, súc tích (không kèm markdown, không có lời dẫn hoặc dấu ngoặc kép thừa).";

                string userPrompt = $"Dữ liệu tài chính tháng {month:D2}/{year}:\n"
                    + $"- Tổng thu nhập: {totalIncome:N0} VND\n"
                    + $"- Tổng chi tiêu: {totalExpense:N0} VND\n"
                    + $"- Dòng tiền thặng dư/thâm hụt: {netSavings:N0} VND (Tỷ lệ tiết kiệm: {savingsRate:F1}%)\n"
                    + $"- Top danh mục chi nhiều nhất: {topCatSummary}";

                var response = await _geminiClient.GenerateTextAsync(userPrompt, systemPrompt, ct);
                if (response.IsSuccess && !string.IsNullOrWhiteSpace(response.Value))
                {
                    string text = response.Value.Trim(' ', '"', '\r', '\n');
                    if (!string.IsNullOrWhiteSpace(text))
                        return text;
                }
            }
            catch
            {
                // Fallback gracefully to deterministic commentary if API is unreachable
            }
        }

        // Resilient Deterministic Fallback Template
        string achievement = savingsRate >= 20m
            ? $"Bạn đã tích lũy thành công {savingsRate:F1}% thu nhập, vượt chuẩn vàng 20% của quy tắc 50/30/20!"
            : $"Bạn đã kiểm soát tốt các khoản chi thiết yếu trong tháng.";

        string recommendation = savingsRate < 20m
            ? "Tháng tới hãy thử áp dụng nguyên tắc 'Trả cho mình trước': Trích ngay 15-20% vào quỹ tích lũy ngay khi nhận lương để tạo thói quen kỷ luật."
            : "Tiếp tục duy trì nhịp độ chi tiêu đàn hồi hiện tại và cân nhắc chuyển bớt tiền nhàn rỗi sang Quỹ dự phòng khẩn cấp.";

        return $"Tháng {month:D2}/{year}, Bạn đạt tổng thu nhập {totalIncome:N0} VND và chi tiêu {totalExpense:N0} VND. "
            + $"Dòng tiền thặng dư ghi nhận {netSavings:N0} VND (Tỷ lệ tiết kiệm {savingsRate:F1}%). "
            + $"{achievement} {recommendation}";
    }
}
