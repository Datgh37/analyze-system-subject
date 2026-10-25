namespace ExpenseTracker.Application.DTOs.AiAgent;

// Commentary from AI Financial Advisor (Richard Thaler philosophy)
public record ExecutiveCommentaryDto(
    string MonthYear,
    string FinancialHealthStatus,       // "Xuất sắc (Thặng dư cao)", "Ổn định", "Cần lưu tâm"
    string CommentaryText,              // 3-5 sentences empathetic advice
    string HighlightAchievement,        // 1 positive accomplishment (e.g. Saved 25%)
    string EmpathyRecommendation,       // 1 gentle action suggestion
    decimal SavingsRate,
    decimal NetWorthChange,
    decimal TotalIncome,
    decimal TotalExpense
);

// Request for exporting report with AI commentary
public record SmartExportRequestDto(
    int Year,
    int Month,
    bool IncludeAiCommentary = true,
    string Format = "excel"             // "excel" or "pdf"
);

// Generic export file result (binary payload)
public record ExportResultDto(
    byte[] FileBytes,
    string ContentType,
    string FileName
);
