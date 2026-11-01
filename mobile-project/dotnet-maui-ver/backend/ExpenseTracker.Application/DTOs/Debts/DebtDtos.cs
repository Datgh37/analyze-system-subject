namespace ExpenseTracker.Application.DTOs.Debts;

// DTO biểu diễn tóm tắt khoản nợ cá nhân (UC-06.05)
public record DebtDto(
    Guid Id,
    Guid UserId,
    string Name,
    int DebtType,
    string DebtTypeName,
    string CreditorName,
    decimal PrincipalAmount,
    decimal CurrentBalance,
    decimal PaidAmount,
    decimal ProgressPercent,
    decimal InterestRateAnnual,
    decimal MinimumPayment,
    int DueDateMonthly,
    DateTime StartDate,
    DateTime? ExpectedPayoffDate,
    int Status,
    string StatusName,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

// DTO chi tiết khoản nợ kèm danh sách các lần thanh toán (UC-06.05)
public record DebtDetailDto(
    DebtDto Debt,
    List<DebtTransactionDto> Transactions
);

// DTO biểu diễn một lần giao dịch trả nợ (UC-06.07)
public record DebtTransactionDto(
    Guid Id,
    Guid DebtId,
    Guid WalletId,
    string WalletName,
    decimal Amount,
    decimal PrincipalPaid,
    decimal InterestPaid,
    DateTime PaymentDate,
    string? Note,
    DateTime CreatedAt
);

// Payload tạo mới khoản nợ cá nhân (UC-06.05)
public record CreateDebtDto(
    string Name,
    int DebtType,
    string CreditorName,
    decimal PrincipalAmount,
    decimal InterestRateAnnual,
    decimal MinimumPayment,
    int DueDateMonthly,
    DateTime StartDate,
    DateTime? ExpectedPayoffDate = null,
    decimal? CurrentBalance = null,
    string? Notes = null
);

// Payload cập nhật thông tin khoản nợ (UC-06.05)
public record UpdateDebtDto(
    string Name,
    int DebtType,
    string CreditorName,
    decimal InterestRateAnnual,
    decimal MinimumPayment,
    int DueDateMonthly,
    DateTime? ExpectedPayoffDate = null,
    string? Notes = null
);

// Payload ghi nhận thanh toán trả nợ từ một ví cụ thể (UC-06.07)
public record RecordDebtPaymentDto(
    Guid WalletId,
    decimal Amount,
    decimal PrincipalPaid,
    decimal InterestPaid,
    DateTime PaymentDate,
    string? Note = null
);

// DTO tổng quan toàn bộ danh mục nợ của người dùng
public record DebtSummaryDto(
    decimal TotalCurrentDebt,
    decimal TotalInitialPrincipal,
    decimal TotalPaidPrincipal,
    decimal TotalMonthlyMinimumPayment,
    int ActiveDebtsCount,
    int PaidOffDebtsCount
);

// --- DTOs HƯỚNG DẪN GIÁO DỤC TÀI CHÍNH BÌNH DÂN HÓA (UC-06.06) ---

// Chi tiết giải thích một phương pháp trả nợ bằng ngôn ngữ đời thường
public record DebtMethodExplanationDto(
    string MethodKey,
    string Title,
    string SubTitle,
    string HowItWorks,
    string BestForWhom,
    string RealLifeExample,
    List<string> Pros,
    List<string> Cons
);

// Tùy chọn trong câu hỏi trắc nghiệm định hướng hành vi
public record DebtQuizOptionDto(
    string OptionKey,
    string OptionText,
    string TargetStrategy,
    string Explanation
);

// Câu hỏi trắc nghiệm định hướng hành vi
public record DebtQuizQuestionDto(
    string QuestionId,
    string QuestionText,
    List<DebtQuizOptionDto> Options
);

// Trọn bộ nội dung hướng dẫn bình dân hóa cho người dùng không chuyên kinh tế
public record DebtEducationalGuideDto(
    List<DebtMethodExplanationDto> Methods,
    List<DebtQuizQuestionDto> QuizQuestions,
    string SafeBudgetGuideText,
    List<string> GoldenRules
);

// Gợi ý mức ngân sách trả nợ thêm an toàn dựa trên thu chi thực tế của người dùng
public record SafeExtraBudgetRecommendationDto(
    decimal MonthlyIncome,
    decimal MonthlyEssentialExpenses,
    decimal TotalMonthlyDebtMinimum,
    decimal MonthlySurplus,
    decimal ConservativeAmount,
    decimal RecommendedAmount,
    decimal AggressiveAmount,
    decimal MaxSafeAmount,
    string AdviceText
);

// --- DTOs ĐỘNG CƠ TỐI ƯU HÓA TRẢ NỢ (SNOWBALL VS AVALANCHE) ---

// Một khoản nợ đưa vào mô phỏng
public record DebtSimulationItemDto(
    Guid? DebtId,
    string Name,
    decimal Balance,
    decimal InterestRateAnnual,
    decimal MinimumPayment
);

// Yêu cầu mô phỏng và so sánh chiến lược
public record DebtPayoffSimulationRequestDto(
    decimal MonthlyExtraBudget,
    List<DebtSimulationItemDto>? CustomDebts = null
);

// Chi tiết thanh toán từng khoản nợ trong một tháng mô phỏng
public record DebtMonthlyPaymentDetailDto(
    Guid? DebtId,
    string DebtName,
    decimal BeginningBalance,
    decimal InterestCharged,
    decimal PrincipalPaid,
    decimal EndingBalance,
    bool IsPaidOffThisMonth
);

// Dòng thời gian mô phỏng theo tháng
public record DebtPayoffTimelineMonthDto(
    int Month,
    DateTime ProjectedDate,
    decimal TotalInterestPaid,
    decimal TotalPrincipalPaid,
    decimal RemainingTotalBalance,
    List<string> PaidOffDebtsThisMonth,
    List<DebtMonthlyPaymentDetailDto> Details
);

// Kết quả mô phỏng của một chiến lược cụ thể
public record DebtStrategySimulationResultDto(
    string Strategy,
    string StrategyName,
    string Description,
    int TotalMonths,
    DateTime ProjectedPayoffDate,
    decimal TotalInterestPaid,
    decimal TotalPrincipalPaid,
    decimal TotalAmountPaid,
    List<string> PayoffOrder,
    bool IsDebtFree,
    bool HasNegativeAmortization,
    string? WarningMessage,
    List<DebtPayoffTimelineMonthDto> Timeline
);

// Kết quả so sánh trực tiếp giữa Snowball và Avalanche kèm lời khuyên cá nhân hóa
public record DebtPayoffComparisonDto(
    DebtStrategySimulationResultDto Snowball,
    DebtStrategySimulationResultDto Avalanche,
    decimal InterestSavingsWithAvalanche,
    int MonthsSavingsWithAvalanche,
    string Recommendation,
    string RecommendedStrategy,
    string LaymanRecommendationText
);


