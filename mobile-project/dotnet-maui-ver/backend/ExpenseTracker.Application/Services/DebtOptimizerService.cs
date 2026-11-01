using ExpenseTracker.Application.DTOs.Debts;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Services;

// Dịch vụ động cơ tối ưu hóa trả nợ cá nhân và hướng dẫn bình dân hóa tài chính (UC-06.06)
public class DebtOptimizerService : IDebtOptimizerService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DebtOptimizerService> _logger;

    private const int MaxSimulationMonths = 360; // Giới hạn tối đa 30 năm chống lặp vô hạn
    private const decimal NudgeSavingsThreshold = 1_000_000m; // Ngưỡng 1 triệu đồng để kích hoạt Nudge Avalanche

    public DebtOptimizerService(IApplicationDbContext context, ILogger<DebtOptimizerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Cung cấp nội dung cẩm nang hướng dẫn bình dân hóa tài chính và câu hỏi trắc nghiệm định hướng
    public DebtEducationalGuideDto GetEducationalGuide()
    {
        var methods = new List<DebtMethodExplanationDto>
        {
            new DebtMethodExplanationDto(
                MethodKey: "SNOWBALL",
                Title: "Hòn Tuyết Lăn (Debt Snowball)",
                SubTitle: "Ưu tiên xóa sạch món nợ nhỏ nhất trước để nhẹ đầu",
                HowItWorks: "Giống như nặn một hòn tuyết nhỏ rồi lăn xuống dốc, hòn tuyết sẽ to dần lên. Bạn trả số tiền tối thiểu cho mọi món nợ, nhưng dồn toàn bộ tiền dư vào món nợ có số tiền nợ ít nhất. Khi món nợ đó sạch bóng, bạn lấy toàn bộ số tiền hàng tháng của nó dồn sang món tiếp theo.",
                BestForWhom: "Phù hợp nhất cho người đang bị stress, hoang mang vì có quá nhiều bên đòi nợ cùng lúc (bạn bè, trả góp, người thân...) và cần cảm giác chiến thắng nhỏ sớm để có thêm động lực.",
                RealLifeExample: "Bạn nợ bạn thân 2 triệu, nợ trả góp điện thoại 5 triệu, nợ thẻ tín dụng 20 triệu. Cách này sẽ giúp bạn trả dứt điểm 2 triệu bạn thân ngay tháng đầu, bớt đi 1 người đòi nợ, tâm lý cực kỳ nhẹ nhõm!",
                Pros: new List<string>
                {
                    "Tạo cảm giác chiến thắng nhanh chóng (Quick Wins), giải phóng dopamine giúp giảm stress.",
                    "Giảm nhanh số lượng chủ nợ phải theo dõi hàng tháng.",
                    "Dễ duy trì kỷ luật, hạn chế bỏ cuộc giữa chừng."
                },
                Cons: new List<string>
                {
                    "Có thể phải trả nhiều tiền lãi hơn một chút cho ngân hàng so với Avalanche nếu các món nợ lớn có lãi suất rất cao."
                }
            ),
            new DebtMethodExplanationDto(
                MethodKey: "AVALANCHE",
                Title: "Tuyết Lở (Debt Avalanche)",
                SubTitle: "Ưu tiên triệt tiêu món nợ lãi cao nhất trước để đỡ tốn tiền",
                HowItWorks: "Giống như chặn dòng tuyết lở nguy hiểm nhất trước, bạn sắp xếp các món nợ theo lãi suất từ cao xuống thấp. Bạn trả mức tối thiểu cho tất cả, rồi dồn toàn bộ tiền dư vào món có lãi suất 'cắt cổ' nhất (thường là thẻ tín dụng hoặc nợ tín dụng tiêu dùng 25-36%/năm).",
                BestForWhom: "Phù hợp nhất cho người bình tĩnh, có tính kỷ luật cao, muốn tiết kiệm tối đa từng đồng tiền lãi trả cho ngân hàng.",
                RealLifeExample: "Bạn có nợ thẻ tín dụng lãi 32%/năm và nợ ngân hàng mua xe lãi 9%/năm. Avalanche sẽ dồn toàn lực đè bẹp nợ thẻ tín dụng trước, giúp bạn không bị ngân hàng bòn rút hàng triệu tiền lãi mỗi tháng.",
                Pros: new List<string>
                {
                    "Tiết kiệm tối đa số tiền lãi phải trả về mặt toán học.",
                    "Rút ngắn tổng thời gian sạch nợ đến mức tối đa.",
                    "Bảo vệ bạn khỏi cạm bẫy lãi kép của thẻ tín dụng."
                },
                Cons: new List<string>
                {
                    "Cần kiên nhẫn hơn, vì món nợ lãi cao có thể có số dư lớn, mất nhiều tháng mới trả xong món đầu tiên."
                }
            )
        };

        var quizQuestions = new List<DebtQuizQuestionDto>
        {
            new DebtQuizQuestionDto(
                QuestionId: "Q1_EMOTION_VS_MATH",
                QuestionText: "Khi nghĩ đến các khoản nợ hiện tại, điều gì làm bạn bận tâm nhất?",
                Options: new List<DebtQuizOptionDto>
                {
                    new DebtQuizOptionDto(
                        OptionKey: "OPT_STRESS",
                        OptionText: "Tôi bị mệt mỏi và áp lực vì có quá nhiều bên đòi nợ, tôi muốn xóa bớt danh sách nợ càng sớm càng tốt.",
                        TargetStrategy: "SNOWBALL",
                        Explanation: "Tâm lý thoải mái là ưu tiên số 1 của bạn lúc này. Hãy chọn Snowball để tận hưởng niềm vui xóa sổ từng khoản nợ một!"
                    ),
                    new DebtQuizOptionDto(
                        OptionKey: "OPT_SAVINGS",
                        OptionText: "Tôi tiếc tiền và xót xa khi thấy mỗi tháng bị trừ quá nhiều tiền lãi vô lý cho ngân hàng.",
                        TargetStrategy: "AVALANCHE",
                        Explanation: "Bạn là người thực tế và coi trọng từng đồng tiền mồ hôi nước mắt. Hãy chọn Avalanche để chặn đứng việc tiền lãi phát sinh!"
                    )
                }
            )
        };

        var goldenRules = new List<string>
        {
            "Quy tắc 1: Luôn trả đủ số tiền tối thiểu (Minimum Payment) cho mọi khoản nợ để không bị phạt phí trả chậm và dính nợ xấu CIC.",
            "Quy tắc 2: Đừng dồn toàn bộ 100% tiền dư vào trả nợ, luôn giữ lại một khoản tiền phòng thân nhỏ (quỹ khẩn cấp) để không phải vay mới khi ốm đau.",
            "Quy tắc 3: Sau khi trả hết một món nợ, đừng tiêu xài số tiền đó mà hãy dồn toàn bộ số tiền đó sang món nợ kế tiếp (Hiệu ứng hòn tuyết lăn)."
        };

        return new DebtEducationalGuideDto(
            Methods: methods,
            QuizQuestions: quizQuestions,
            SafeBudgetGuideText: "Ngân sách trả thêm an toàn là số tiền thặng dư sau khi lấy Thu nhập trừ đi Chi tiêu thiết yếu (Ăn uống, thuê nhà) và Tiền trả nợ tối thiểu. Chúng tôi khuyên bạn chỉ nên trích 30% đến tối đa 50% thặng dư này để trả nợ thêm, phần còn lại giữ phòng thân.",
            GoldenRules: goldenRules
        );
    }

    // Tính toán gợi ý ngân sách trả thêm an toàn dựa trên thu chi thực tế của người dùng
    public async Task<SafeExtraBudgetRecommendationDto> GetSafeExtraBudgetRecommendationAsync(
        Guid userId, 
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var thirtyDaysAgo = now.AddDays(-30);

        // 1. Tính tổng thu nhập bình quân gần đây
        var recentIncomes = await _context.Transactions
            .Where(t => t.UserId == userId && t.TransactionType == TransactionType.Income && t.TransactionDate >= thirtyDaysAgo)
            .Select(t => t.Amount)
            .ToListAsync(ct);

        decimal monthlyIncome = recentIncomes.Count > 0 ? recentIncomes.Sum() : 0m;

        // Nếu người dùng có cấu hình RecurringIncome thì kết hợp lấy thu nhập định kỳ
        var recurringIncomes = await _context.RecurringIncomes
            .Where(r => r.UserId == userId && r.IsActive)
            .Select(r => r.ExpectedAmount)
            .ToListAsync(ct);

        if (recurringIncomes.Count > 0 && monthlyIncome == 0m)
        {
            monthlyIncome = recurringIncomes.Sum();
        }

        // Mặc định fallback nếu tài khoản mới chưa có giao dịch
        if (monthlyIncome <= 0m)
        {
            monthlyIncome = 15_000_000m; // Thu nhập bình quân tham chiếu chuẩn
        }

        // 2. Tính chi tiêu thiết yếu gần nhất (Needs)
        var recentExpenses = await _context.Transactions
            .Where(t => t.UserId == userId && t.TransactionType == TransactionType.Expense && t.TransactionDate >= thirtyDaysAgo)
            .Select(t => t.Amount)
            .ToListAsync(ct);

        decimal monthlyExpenses = recentExpenses.Count > 0 ? recentExpenses.Sum() : (monthlyIncome * 0.50m);

        // 3. Tính tổng số tiền trả tối thiểu của các khoản nợ active
        var activeDebts = await _context.Debts
            .Where(d => d.UserId == userId && d.Status != DebtStatus.PaidOff && d.CurrentBalance > 0)
            .ToListAsync(ct);

        decimal totalDebtMinimum = activeDebts.Sum(d => d.MinimumPayment);

        // 4. Tính toán thặng dư
        decimal surplus = monthlyIncome - monthlyExpenses - totalDebtMinimum;

        decimal conservative = 0m;
        decimal recommended = 0m;
        decimal aggressive = 0m;
        decimal maxSafe = 0m;
        string advice;

        if (surplus <= 0)
        {
            advice = "Dòng tiền hàng tháng của bạn đang khá căng thẳng hoặc bị thâm hụt. Lời khuyên an toàn: Hãy chỉ duy trì trả số tiền tối thiểu (Minimum Payment) đúng hạn và tìm cách cắt giảm các chi phí chưa cần thiết trước khi nghĩ đến việc trả thêm nợ.";
        }
        else
        {
            // Làm tròn bội số 50.000đ cho tròn tiền thực tế
            conservative = RoundToMultiple(surplus * 0.10m, 50_000m);
            recommended = RoundToMultiple(surplus * 0.30m, 50_000m);
            aggressive = RoundToMultiple(surplus * 0.50m, 50_000m);
            maxSafe = RoundToMultiple(surplus * 0.70m, 50_000m);

            advice = $"Bạn đang có thặng dư tài chính ước tính khoảng {surplus:N0}đ/tháng sau khi trừ sinh hoạt phí và nợ tối thiểu. Chúng tôi khuyên bạn nên trích {recommended:N0}đ/tháng (mức 30%) để trả nợ thêm, vừa giúp sạch nợ nhanh mà vẫn giữ lại được tiền phòng thân an toàn.";
        }

        return new SafeExtraBudgetRecommendationDto(
            MonthlyIncome: Math.Round(monthlyIncome, 0),
            MonthlyEssentialExpenses: Math.Round(monthlyExpenses, 0),
            TotalMonthlyDebtMinimum: Math.Round(totalDebtMinimum, 0),
            MonthlySurplus: Math.Round(surplus, 0),
            ConservativeAmount: conservative,
            RecommendedAmount: recommended,
            AggressiveAmount: aggressive,
            MaxSafeAmount: maxSafe,
            AdviceText: advice
        );
    }

    // So sánh song song hai chiến lược Snowball vs Avalanche và đưa ra khuyến nghị hành vi đời thường
    public async Task<DebtPayoffComparisonDto> CompareStrategiesAsync(
        Guid userId, 
        DebtPayoffSimulationRequestDto request, 
        CancellationToken ct = default)
    {
        var simulationDebts = await ResolveSimulationDebtsAsync(userId, request.CustomDebts, ct);

        var snowballResult = SimulateCore(simulationDebts, request.MonthlyExtraBudget, "SNOWBALL");
        var avalancheResult = SimulateCore(simulationDebts, request.MonthlyExtraBudget, "AVALANCHE");

        decimal interestSavings = Math.Max(0m, snowballResult.TotalInterestPaid - avalancheResult.TotalInterestPaid);
        int monthsSavings = Math.Max(0, snowballResult.TotalMonths - avalancheResult.TotalMonths);

        string recommendation;
        string recommendedStrategy;
        string laymanAdvice;

        if (snowballResult.HasNegativeAmortization || avalancheResult.HasNegativeAmortization)
        {
            recommendedStrategy = "ALERT_NEGATIVE_AMORTIZATION";
            recommendation = "Cảnh báo khẩn cấp: Số tiền trả nợ hàng tháng không đủ bù tiền lãi phát sinh! Bạn đang rơi vào bẫy nợ Khấu hao âm (Negative Amortization).";
            laymanAdvice = "Nguy hiểm! Số tiền bạn dự định trả mỗi tháng còn ít hơn cả tiền lãi ngân hàng tự động tính thêm. Càng trả thì nợ sẽ càng phình to ra. Bạn bắt buộc phải tăng thêm tiền trả hàng tháng hoặc thương lượng với chủ nợ để hạ lãi suất!";
        }
        else if (!snowballResult.IsDebtFree && !avalancheResult.IsDebtFree)
        {
            recommendedStrategy = "INCREASE_PAYMENT";
            recommendation = "Cần tăng số tiền trả nợ hàng tháng để có thể tất toán nợ trong khung thời gian hợp lý.";
            laymanAdvice = "Với mức chi trả hiện tại, sẽ mất hơn 30 năm mới có thể trả hết nợ. Hãy cố gắng trích thêm ngân sách trả nợ mỗi tháng để về đích sớm hơn.";
        }
        else if (interestSavings >= NudgeSavingsThreshold)
        {
            recommendedStrategy = "AVALANCHE";
            recommendation = $"Avalanche là chiến lược tối ưu nhất. Bạn sẽ tiết kiệm được {interestSavings:N0}đ tiền lãi và về đích sớm hơn {monthsSavings} tháng so với Snowball.";
            laymanAdvice = $"Bạn nên chọn Tuyết Lở (Avalanche)! Cách này giúp bạn tiết kiệm được tới {interestSavings:N0}đ tiền lãi phải trả cho ngân hàng (đủ tiền mua sắm sinh hoạt cho nhiều tuần). Hãy tập trung dồn tiền đè bẹp các khoản có lãi suất cao nhất trước nhé!";
        }
        else
        {
            recommendedStrategy = "SNOWBALL";
            recommendation = $"Chênh lệch tiền lãi giữa hai phương pháp không đáng kể ({interestSavings:N0}đ). Khuyên dùng Snowball để giải phóng các khoản nợ nhỏ sớm nhất và tạo động lực tinh thần.";
            laymanAdvice = $"Bạn nên chọn Hòn Tuyết Lăn (Snowball)! Khoản tiền lãi tiết kiệm được giữa 2 cách chỉ chênh lệch khoảng {interestSavings:N0}đ (không quá lớn). Chọn Snowball sẽ giúp bạn nhanh chóng xóa sổ dứt điểm các khoản nợ nhỏ trước, bớt đi người đòi nợ và cảm thấy nhẹ đầu hơn rất nhiều!";
        }

        return new DebtPayoffComparisonDto(
            Snowball: snowballResult,
            Avalanche: avalancheResult,
            InterestSavingsWithAvalanche: interestSavings,
            MonthsSavingsWithAvalanche: monthsSavings,
            Recommendation: recommendation,
            RecommendedStrategy: recommendedStrategy,
            LaymanRecommendationText: laymanAdvice
        );
    }

    // Mô phỏng chi tiết chiến lược Snowball
    public async Task<DebtStrategySimulationResultDto> SimulateSnowballAsync(
        Guid userId, 
        decimal monthlyExtraBudget, 
        List<DebtSimulationItemDto>? customDebts = null, 
        CancellationToken ct = default)
    {
        var simulationDebts = await ResolveSimulationDebtsAsync(userId, customDebts, ct);
        return SimulateCore(simulationDebts, monthlyExtraBudget, "SNOWBALL");
    }

    // Mô phỏng chi tiết chiến lược Avalanche
    public async Task<DebtStrategySimulationResultDto> SimulateAvalancheAsync(
        Guid userId, 
        decimal monthlyExtraBudget, 
        List<DebtSimulationItemDto>? customDebts = null, 
        CancellationToken ct = default)
    {
        var simulationDebts = await ResolveSimulationDebtsAsync(userId, customDebts, ct);
        return SimulateCore(simulationDebts, monthlyExtraBudget, "AVALANCHE");
    }

    // --- THUẬT TOÁN MÔ PHỎNG LÃI SUẤT DƯ NỢ GIẢM DẦN & TỐI ƯU HÓA ---

    private DebtStrategySimulationResultDto SimulateCore(
        List<DebtSimulationItemDto> initialDebts, 
        decimal extraBudget, 
        string strategy)
    {
        if (initialDebts.Count == 0)
        {
            return new DebtStrategySimulationResultDto(
                Strategy: strategy,
                StrategyName: strategy == "SNOWBALL" ? "Hòn Tuyết Lăn" : "Tuyết Lở",
                Description: "Không có khoản nợ nào cần thanh toán.",
                TotalMonths: 0,
                ProjectedPayoffDate: DateTime.UtcNow,
                TotalInterestPaid: 0m,
                TotalPrincipalPaid: 0m,
                TotalAmountPaid: 0m,
                PayoffOrder: new List<string>(),
                IsDebtFree: true,
                HasNegativeAmortization: false,
                WarningMessage: null,
                Timeline: new List<DebtPayoffTimelineMonthDto>()
            );
        }

        // Tạo bản sao độc lập các khoản nợ để mô phỏng
        var debts = initialDebts.Select(d => new WorkingDebt(
            d.DebtId,
            d.Name,
            d.Balance,
            d.InterestRateAnnual,
            d.MinimumPayment
        )).ToList();

        // Kiểm tra bẫy nợ Khấu hao âm (Negative Amortization) ngay từ tháng đầu
        decimal totalMonthlyInterestMonth1 = debts.Sum(d => Math.Round(d.Balance * ((d.InterestRateAnnual / 100m) / 12m), 0, MidpointRounding.AwayFromZero));
        decimal totalMonthlyPaymentMonth1 = debts.Sum(d => d.MinimumPayment) + Math.Max(0m, extraBudget);

        if (totalMonthlyPaymentMonth1 <= totalMonthlyInterestMonth1)
        {
            return new DebtStrategySimulationResultDto(
                Strategy: strategy,
                StrategyName: strategy == "SNOWBALL" ? "Hòn Tuyết Lăn" : "Tuyết Lở",
                Description: "Cảnh báo: Bẫy nợ Khấu hao âm (Negative Amortization)",
                TotalMonths: MaxSimulationMonths,
                ProjectedPayoffDate: DateTime.UtcNow.AddMonths(MaxSimulationMonths),
                TotalInterestPaid: totalMonthlyInterestMonth1 * MaxSimulationMonths,
                TotalPrincipalPaid: 0m,
                TotalAmountPaid: totalMonthlyPaymentMonth1 * MaxSimulationMonths,
                PayoffOrder: new List<string>(),
                IsDebtFree: false,
                HasNegativeAmortization: true,
                WarningMessage: $"Nguy hiểm: Tổng số tiền chi trả hàng tháng ({totalMonthlyPaymentMonth1:N0}đ) nhỏ hơn hoặc bằng tiền lãi phát sinh ({totalMonthlyInterestMonth1:N0}đ). Dư nợ sẽ phình to vô tận thay vì giảm đi!",
                Timeline: new List<DebtPayoffTimelineMonthDto>()
            );
        }

        // Sắp xếp thứ tự ưu tiên ban đầu
        if (strategy == "SNOWBALL")
        {
            // Snowball: Dư nợ nhỏ nhất lên đầu
            debts = debts.OrderBy(d => d.Balance).ThenByDescending(d => d.InterestRateAnnual).ToList();
        }
        else
        {
            // Avalanche: Lãi suất cao nhất lên đầu
            debts = debts.OrderByDescending(d => d.InterestRateAnnual).ThenBy(d => d.Balance).ToList();
        }

        decimal totalInterestPaid = 0m;
        decimal totalPrincipalPaid = 0m;
        int month = 0;
        var timeline = new List<DebtPayoffTimelineMonthDto>();
        var payoffOrder = new List<string>();
        decimal rolloverMinimumPayment = 0m; // Hiệu ứng hòn tuyết lăn dồn tiền khi có nợ tất toán

        while (month < MaxSimulationMonths)
        {
            var activeDebts = debts.Where(d => d.Balance > 0.01m).ToList();
            if (activeDebts.Count == 0)
            {
                break; // Đã sạch nợ 100%
            }

            month++;
            decimal monthInterest = 0m;
            decimal monthPrincipal = 0m;
            var monthPaidOffDebts = new List<string>();
            var monthDetails = new List<DebtMonthlyPaymentDetailDto>();

            // 1. Tính tiền lãi tháng phát sinh cho từng khoản nợ theo chuẩn dư nợ giảm dần
            foreach (var debt in activeDebts)
            {
                decimal monthlyRate = (debt.InterestRateAnnual / 100m) / 12m;
                decimal interest = Math.Round(debt.Balance * monthlyRate, 0, MidpointRounding.AwayFromZero);
                debt.BeginningBalance = debt.Balance;
                debt.Balance += interest;
                debt.InterestChargedThisMonth = interest;
                monthInterest += interest;
                totalInterestPaid += interest;
            }

            // 2. Trả số tiền tối thiểu (Minimum Payment) cho tất cả các khoản còn nợ
            decimal availableExtra = Math.Max(0m, extraBudget) + rolloverMinimumPayment;

            foreach (var debt in activeDebts)
            {
                decimal payment = Math.Min(debt.Balance, debt.MinimumPayment);
                debt.Balance -= payment;
                debt.PrincipalPaidThisMonth = payment;
                monthPrincipal += payment;
            }

            // 3. Dồn toàn bộ Extra Budget + Rollover vào khoản nợ ưu tiên số 1 của chiến lược
            if (availableExtra > 0m)
            {
                var remainingDebts = activeDebts.Where(d => d.Balance > 0.01m).ToList();
                if (remainingDebts.Count > 0)
                {
                    WorkingDebt targetDebt = strategy == "SNOWBALL"
                        ? remainingDebts.OrderBy(d => d.Balance).ThenByDescending(d => d.InterestRateAnnual).First()
                        : remainingDebts.OrderByDescending(d => d.InterestRateAnnual).ThenBy(d => d.Balance).First();

                    decimal extraPay = Math.Min(targetDebt.Balance, availableExtra);
                    targetDebt.Balance -= extraPay;
                    targetDebt.PrincipalPaidThisMonth += extraPay;
                    monthPrincipal += extraPay;
                }
            }

            totalPrincipalPaid += monthPrincipal;

            // 4. Kiểm tra xem có khoản nợ nào vừa được tất toán trong tháng này không
            foreach (var debt in activeDebts)
            {
                bool isPaidOff = debt.Balance <= 0.01m;
                if (isPaidOff && !payoffOrder.Contains(debt.Name))
                {
                    debt.Balance = 0m;
                    payoffOrder.Add(debt.Name);
                    monthPaidOffDebts.Add(debt.Name);
                    // Hiệu ứng Snowball: Giải phóng Minimum Payment của khoản vừa tất toán để dồn cho các tháng sau
                    rolloverMinimumPayment += debt.MinimumPayment;
                }

                monthDetails.Add(new DebtMonthlyPaymentDetailDto(
                    DebtId: debt.DebtId,
                    DebtName: debt.Name,
                    BeginningBalance: debt.BeginningBalance,
                    InterestCharged: debt.InterestChargedThisMonth,
                    PrincipalPaid: debt.PrincipalPaidThisMonth,
                    EndingBalance: Math.Max(0m, debt.Balance),
                    IsPaidOffThisMonth: isPaidOff && monthPaidOffDebts.Contains(debt.Name)
                ));
            }

            timeline.Add(new DebtPayoffTimelineMonthDto(
                Month: month,
                ProjectedDate: DateTime.UtcNow.AddMonths(month),
                TotalInterestPaid: monthInterest,
                TotalPrincipalPaid: monthPrincipal,
                RemainingTotalBalance: debts.Sum(d => Math.Max(0m, d.Balance)),
                PaidOffDebtsThisMonth: monthPaidOffDebts,
                Details: monthDetails
            ));
        }

        bool isDebtFree = debts.All(d => d.Balance <= 0.01m);

        return new DebtStrategySimulationResultDto(
            Strategy: strategy,
            StrategyName: strategy == "SNOWBALL" ? "Hòn Tuyết Lăn (Snowball)" : "Tuyết Lở (Avalanche)",
            Description: strategy == "SNOWBALL" 
                ? "Ưu tiên xóa sạch món nợ nhỏ nhất trước để giải phóng áp lực tâm lý và tạo động lực duy trì."
                : "Ưu tiên triệt tiêu món nợ có lãi suất cao nhất trước để tiết kiệm tối đa tiền lãi cho ngân hàng.",
            TotalMonths: month,
            ProjectedPayoffDate: DateTime.UtcNow.AddMonths(month),
            TotalInterestPaid: Math.Round(totalInterestPaid, 0),
            TotalPrincipalPaid: Math.Round(totalPrincipalPaid, 0),
            TotalAmountPaid: Math.Round(totalInterestPaid + totalPrincipalPaid, 0),
            PayoffOrder: payoffOrder,
            IsDebtFree: isDebtFree,
            HasNegativeAmortization: false,
            WarningMessage: isDebtFree ? null : $"Khoản nợ chưa thể tất toán sau {MaxSimulationMonths} tháng (30 năm). Hãy cân nhắc tăng thêm số tiền trả mỗi tháng.",
            Timeline: timeline
        );
    }

    private async Task<List<DebtSimulationItemDto>> ResolveSimulationDebtsAsync(
        Guid userId, 
        List<DebtSimulationItemDto>? customDebts, 
        CancellationToken ct)
    {
        if (customDebts != null && customDebts.Count > 0)
        {
            return customDebts.Where(d => d.Balance > 0).ToList();
        }

        var dbDebts = await _context.Debts
            .Where(d => d.UserId == userId && d.Status != DebtStatus.PaidOff && d.CurrentBalance > 0)
            .OrderBy(d => d.CurrentBalance)
            .ToListAsync(ct);

        return dbDebts.Select(d => new DebtSimulationItemDto(
            DebtId: d.Id,
            Name: d.Name,
            Balance: d.CurrentBalance,
            InterestRateAnnual: d.InterestRateAnnual,
            MinimumPayment: d.MinimumPayment
        )).ToList();
    }

    private static decimal RoundToMultiple(decimal value, decimal multiple)
    {
        if (multiple <= 0 || value <= 0) return 0m;
        return Math.Round(value / multiple, MidpointRounding.AwayFromZero) * multiple;
    }

    // Lớp trợ giúp nội bộ lưu trạng thái biến thiên của từng khoản nợ trong quá trình mô phỏng
    private class WorkingDebt
    {
        public Guid? DebtId { get; }
        public string Name { get; }
        public decimal Balance { get; set; }
        public decimal InterestRateAnnual { get; }
        public decimal MinimumPayment { get; }

        public decimal BeginningBalance { get; set; }
        public decimal InterestChargedThisMonth { get; set; }
        public decimal PrincipalPaidThisMonth { get; set; }

        public WorkingDebt(Guid? debtId, string name, decimal balance, decimal interestRateAnnual, decimal minimumPayment)
        {
            DebtId = debtId;
            Name = name;
            Balance = balance;
            InterestRateAnnual = interestRateAnnual;
            MinimumPayment = minimumPayment;
        }
    }
}
