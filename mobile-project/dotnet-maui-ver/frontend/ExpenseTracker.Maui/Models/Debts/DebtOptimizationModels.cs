namespace ExpenseTracker.Maui.Models.Debts;

// Model giải thích phương thức trả nợ bằng ngôn ngữ bình dân
public class DebtMethodExplanationModel
{
    public string MethodKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string HowItWorks { get; set; } = string.Empty;
    public string BestForWhom { get; set; } = string.Empty;
    public string RealLifeExample { get; set; } = string.Empty;
    public List<string> Pros { get; set; } = new();
    public List<string> Cons { get; set; } = new();
}

// Model tùy chọn trắc nghiệm định hướng hành vi
public class DebtQuizOptionModel
{
    public string OptionKey { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public string TargetStrategy { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}

// Model câu hỏi trắc nghiệm định hướng hành vi
public class DebtQuizQuestionModel
{
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public List<DebtQuizOptionModel> Options { get; set; } = new();
}

// Cẩm nang hướng dẫn bình dân hóa tài chính
public class DebtEducationalGuideModel
{
    public List<DebtMethodExplanationModel> Methods { get; set; } = new();
    public List<DebtQuizQuestionModel> QuizQuestions { get; set; } = new();
    public string SafeBudgetGuideText { get; set; } = string.Empty;
    public List<string> GoldenRules { get; set; } = new();
}

// Gợi ý ngân sách trả thêm an toàn
public class SafeExtraBudgetRecommendationModel
{
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyEssentialExpenses { get; set; }
    public decimal TotalMonthlyDebtMinimum { get; set; }
    public decimal MonthlySurplus { get; set; }
    public decimal ConservativeAmount { get; set; }
    public decimal RecommendedAmount { get; set; }
    public decimal AggressiveAmount { get; set; }
    public decimal MaxSafeAmount { get; set; }
    public string AdviceText { get; set; } = string.Empty;
}

// Item nợ đưa vào mô phỏng
public class DebtSimulationItemModel
{
    public Guid? DebtId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal InterestRateAnnual { get; set; }
    public decimal MinimumPayment { get; set; }
}

// Payload yêu cầu mô phỏng
public class DebtPayoffSimulationRequest
{
    public decimal MonthlyExtraBudget { get; set; }
    public List<DebtSimulationItemModel>? CustomDebts { get; set; }
}

// Chi tiết thanh toán nợ từng tháng
public class DebtMonthlyPaymentDetailModel
{
    public Guid? DebtId { get; set; }
    public string DebtName { get; set; } = string.Empty;
    public decimal BeginningBalance { get; set; }
    public decimal InterestCharged { get; set; }
    public decimal PrincipalPaid { get; set; }
    public decimal EndingBalance { get; set; }
    public bool IsPaidOffThisMonth { get; set; }
}

// Dòng thời gian mô phỏng theo tháng
public class DebtPayoffTimelineMonthModel
{
    public int Month { get; set; }
    public DateTime ProjectedDate { get; set; }
    public decimal TotalInterestPaid { get; set; }
    public decimal TotalPrincipalPaid { get; set; }
    public decimal RemainingTotalBalance { get; set; }
    public List<string> PaidOffDebtsThisMonth { get; set; } = new();
    public List<DebtMonthlyPaymentDetailModel> Details { get; set; } = new();
}

// Kết quả mô phỏng chiến lược
public class DebtStrategySimulationResultModel
{
    public string Strategy { get; set; } = string.Empty;
    public string StrategyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TotalMonths { get; set; }
    public DateTime ProjectedPayoffDate { get; set; }
    public decimal TotalInterestPaid { get; set; }
    public decimal TotalPrincipalPaid { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public List<string> PayoffOrder { get; set; } = new();
    public bool IsDebtFree { get; set; }
    public bool HasNegativeAmortization { get; set; }
    public string? WarningMessage { get; set; }
    public List<DebtPayoffTimelineMonthModel> Timeline { get; set; } = new();
}

// Kết quả so sánh hai chiến lược
public class DebtPayoffComparisonModel
{
    public DebtStrategySimulationResultModel Snowball { get; set; } = new();
    public DebtStrategySimulationResultModel Avalanche { get; set; } = new();
    public decimal InterestSavingsWithAvalanche { get; set; }
    public int MonthsSavingsWithAvalanche { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public string RecommendedStrategy { get; set; } = string.Empty;
    public string LaymanRecommendationText { get; set; } = string.Empty;
}

