using ExpenseTracker.Maui.Models.Debts;

namespace ExpenseTracker.Maui.Repositories;

// Triển khai Mock In-Memory Repository phục vụ Offline Demo Mode (UC-06.05 & UC-06.07)
public class MockDebtRepository : IDebtRepository
{
    private readonly List<DebtModel> _debts = new();
    private readonly List<DebtTransactionModel> _transactions = new();

    public MockDebtRepository()
    {
        SeedMockData();
    }

    private void SeedMockData()
    {
        var debt1 = new DebtModel
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserId = Guid.Empty,
            Name = "Vay Mua Laptop Macbook Pro",
            DebtType = DebtType.InstallmentLoan,
            DebtTypeName = "Vay tiêu dùng / Trả góp",
            CreditorName = "FE Credit",
            PrincipalAmount = 30_000_000m,
            CurrentBalance = 18_000_000m,
            PaidAmount = 12_000_000m,
            ProgressPercent = 40.0m,
            InterestRateAnnual = 14.5m,
            MinimumPayment = 2_500_000m,
            DueDateMonthly = 15,
            StartDate = DateTime.Today.AddMonths(-6),
            ExpectedPayoffDate = DateTime.Today.AddMonths(6),
            Status = DebtStatus.Active,
            StatusName = "Đang trả nợ",
            Notes = "Gói trả góp 12 tháng tại FPT Shop",
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        var debt2 = new DebtModel
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            UserId = Guid.Empty,
            Name = "Thẻ Tín Dụng VIB Online Plus",
            DebtType = DebtType.CreditCard,
            DebtTypeName = "Thẻ tín dụng",
            CreditorName = "VIB Bank",
            PrincipalAmount = 15_000_000m,
            CurrentBalance = 9_500_000m,
            PaidAmount = 5_500_000m,
            ProgressPercent = 36.7m,
            InterestRateAnnual = 26.0m,
            MinimumPayment = 1_000_000m,
            DueDateMonthly = 25,
            StartDate = DateTime.Today.AddMonths(-3),
            ExpectedPayoffDate = DateTime.Today.AddMonths(4),
            Status = DebtStatus.Active,
            StatusName = "Đang trả nợ",
            Notes = "Chi tiêu gia đình và thanh toán online",
            CreatedAt = DateTime.UtcNow.AddMonths(-3)
        };

        var debt3 = new DebtModel
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            UserId = Guid.Empty,
            Name = "Mượn Tiền Bạn Thân Mua Khóa Học",
            DebtType = DebtType.Personal,
            DebtTypeName = "Vay mượn cá nhân",
            CreditorName = "Nguyễn Văn Tuấn",
            PrincipalAmount = 5_000_000m,
            CurrentBalance = 0m,
            PaidAmount = 5_000_000m,
            ProgressPercent = 100m,
            InterestRateAnnual = 0m,
            MinimumPayment = 0m,
            DueDateMonthly = 1,
            StartDate = DateTime.Today.AddMonths(-5),
            ExpectedPayoffDate = DateTime.Today.AddMonths(-1),
            Status = DebtStatus.PaidOff,
            StatusName = "Đã tất toán",
            Notes = "Đã chuyển khoản trả đủ đầu tháng",
            CreatedAt = DateTime.UtcNow.AddMonths(-5)
        };

        _debts.AddRange(new[] { debt1, debt2, debt3 });

        _transactions.Add(new DebtTransactionModel
        {
            Id = Guid.NewGuid(),
            DebtId = debt1.Id,
            WalletId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            WalletName = "Tài khoản Vietcombank",
            Amount = 2_500_000m,
            PrincipalPaid = 2_000_000m,
            InterestPaid = 500_000m,
            PaymentDate = DateTime.Today.AddDays(-14),
            Note = "Trả nợ kỳ tháng trước",
            CreatedAt = DateTime.UtcNow.AddDays(-14)
        });
    }

    public Task<List<DebtModel>> GetDebtsAsync()
    {
        return Task.FromResult(_debts.OrderBy(d => d.Status).ThenBy(d => d.DueDateMonthly).ToList());
    }

    public Task<DebtDetailModel?> GetDebtByIdAsync(Guid id)
    {
        var debt = _debts.FirstOrDefault(d => d.Id == id);
        if (debt == null) return Task.FromResult<DebtDetailModel?>(null);

        var transactions = _transactions.Where(t => t.DebtId == id).OrderByDescending(t => t.PaymentDate).ToList();
        return Task.FromResult<DebtDetailModel?>(new DebtDetailModel
        {
            Debt = debt,
            Transactions = transactions
        });
    }

    public Task<DebtSummaryModel> GetDebtSummaryAsync()
    {
        var activeDebts = _debts.Where(d => d.Status != DebtStatus.PaidOff).ToList();
        var summary = new DebtSummaryModel
        {
            TotalCurrentDebt = activeDebts.Sum(d => d.CurrentBalance),
            TotalInitialPrincipal = _debts.Sum(d => d.PrincipalAmount),
            TotalPaidPrincipal = _debts.Sum(d => d.PaidAmount),
            TotalMonthlyMinimumPayment = activeDebts.Sum(d => d.MinimumPayment),
            ActiveDebtsCount = activeDebts.Count,
            PaidOffDebtsCount = _debts.Count(d => d.Status == DebtStatus.PaidOff)
        };
        return Task.FromResult(summary);
    }

    public Task<DebtModel> CreateDebtAsync(CreateDebtRequest request)
    {
        var typeName = (DebtType)request.DebtType switch
        {
            DebtType.Personal => "Vay mượn cá nhân",
            DebtType.CreditCard => "Thẻ tín dụng",
            DebtType.InstallmentLoan => "Vay tiêu dùng / Trả góp",
            DebtType.MortgageOrBankLoan => "Vay ngân hàng / Thế chấp",
            _ => "Khác"
        };

        var initialBalance = request.CurrentBalance ?? request.PrincipalAmount;
        var paid = Math.Max(0, request.PrincipalAmount - initialBalance);
        var progress = request.PrincipalAmount > 0 ? (paid / request.PrincipalAmount * 100m) : 0m;

        var debt = new DebtModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty,
            Name = request.Name,
            DebtType = (DebtType)request.DebtType,
            DebtTypeName = typeName,
            CreditorName = request.CreditorName,
            PrincipalAmount = request.PrincipalAmount,
            CurrentBalance = initialBalance,
            PaidAmount = paid,
            ProgressPercent = progress,
            InterestRateAnnual = request.InterestRateAnnual,
            MinimumPayment = request.MinimumPayment,
            DueDateMonthly = request.DueDateMonthly,
            StartDate = request.StartDate,
            ExpectedPayoffDate = request.ExpectedPayoffDate,
            Status = initialBalance == 0 ? DebtStatus.PaidOff : DebtStatus.Active,
            StatusName = initialBalance == 0 ? "Đã tất toán" : "Đang trả nợ",
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _debts.Add(debt);
        return Task.FromResult(debt);
    }

    public Task<DebtModel> UpdateDebtAsync(Guid id, UpdateDebtRequest request)
    {
        var debt = _debts.FirstOrDefault(d => d.Id == id) ?? throw new KeyNotFoundException("Không tìm thấy khoản nợ.");

        var typeName = (DebtType)request.DebtType switch
        {
            DebtType.Personal => "Vay mượn cá nhân",
            DebtType.CreditCard => "Thẻ tín dụng",
            DebtType.InstallmentLoan => "Vay tiêu dùng / Trả góp",
            DebtType.MortgageOrBankLoan => "Vay ngân hàng / Thế chấp",
            _ => "Khác"
        };

        debt.Name = request.Name;
        debt.DebtType = (DebtType)request.DebtType;
        debt.DebtTypeName = typeName;
        debt.CreditorName = request.CreditorName;
        debt.InterestRateAnnual = request.InterestRateAnnual;
        debt.MinimumPayment = request.MinimumPayment;
        debt.DueDateMonthly = request.DueDateMonthly;
        debt.ExpectedPayoffDate = request.ExpectedPayoffDate;
        debt.Notes = request.Notes;
        debt.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(debt);
    }

    public Task<DebtTransactionModel> RecordPaymentAsync(Guid id, RecordDebtPaymentRequest request)
    {
        var debt = _debts.FirstOrDefault(d => d.Id == id) ?? throw new KeyNotFoundException("Không tìm thấy khoản nợ.");

        if (debt.Status == DebtStatus.PaidOff)
            throw new InvalidOperationException("Khoản nợ này đã được tất toán.");

        debt.CurrentBalance = Math.Max(0, debt.CurrentBalance - request.PrincipalPaid);
        debt.PaidAmount = Math.Max(0, debt.PrincipalAmount - debt.CurrentBalance);
        debt.ProgressPercent = debt.PrincipalAmount > 0 ? (debt.PaidAmount / debt.PrincipalAmount * 100m) : 100m;

        if (debt.CurrentBalance == 0)
        {
            debt.Status = DebtStatus.PaidOff;
            debt.StatusName = "Đã tất toán";
        }

        var transaction = new DebtTransactionModel
        {
            Id = Guid.NewGuid(),
            DebtId = id,
            WalletId = request.WalletId,
            WalletName = "Ví Nguồn",
            Amount = request.Amount,
            PrincipalPaid = request.PrincipalPaid,
            InterestPaid = request.InterestPaid,
            PaymentDate = request.PaymentDate,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow
        };

        _transactions.Add(transaction);
        return Task.FromResult(transaction);
    }

    public Task<bool> ArchiveDebtAsync(Guid id)
    {
        var debt = _debts.FirstOrDefault(d => d.Id == id);
        if (debt == null) return Task.FromResult(false);

        _debts.Remove(debt);
        return Task.FromResult(true);
    }

    // --- CÁC PHƯƠNG THỨC MÔ PHỎNG & HƯỚNG DẪN TỐI ƯU HÓA OFFLINE (UC-06.06) ---

    public Task<DebtEducationalGuideModel> GetEducationalGuideAsync()
    {
        var guide = new DebtEducationalGuideModel
        {
            Methods = new List<DebtMethodExplanationModel>
            {
                new DebtMethodExplanationModel
                {
                    MethodKey = "SNOWBALL",
                    Title = "Hòn Tuyết Lăn (Debt Snowball)",
                    SubTitle = "Ưu tiên xóa sạch món nợ nhỏ nhất trước để nhẹ đầu",
                    HowItWorks = "Bạn trả số tiền tối thiểu cho mọi món nợ, dồn toàn bộ tiền dư vào món nợ có số tiền nợ ít nhất. Khi món nợ đó sạch bóng, bạn lấy toàn bộ số tiền hàng tháng của nó dồn sang món tiếp theo (Hiệu ứng hòn tuyết lăn to dần).",
                    BestForWhom = "Phù hợp nhất cho người đang bị stress, hoang mang vì có quá nhiều bên đòi nợ cùng lúc và cần chiến thắng tâm lý sớm để có thêm động lực.",
                    RealLifeExample = "Bạn nợ bạn thân 2 triệu, trả góp 5 triệu, nợ thẻ 20 triệu. Cách này sẽ giúp bạn dọn dẹp sạch khoản 2 triệu ngay tháng đầu, bớt đi 1 người đòi nợ, tâm lý cực kỳ nhẹ nhõm!",
                    Pros = new List<string>
                    {
                        "Tạo cảm giác chiến thắng nhanh chóng (Quick Wins), giải phóng dopamine giảm stress.",
                        "Giảm nhanh số lượng chủ nợ phải theo dõi hàng tháng.",
                        "Dễ duy trì kỷ luật, hạn chế bỏ cuộc giữa chừng."
                    },
                    Cons = new List<string>
                    {
                        "Có thể trả nhiều tiền lãi hơn một chút cho ngân hàng so với Avalanche nếu các món nợ lớn có lãi suất rất cao."
                    }
                },
                new DebtMethodExplanationModel
                {
                    MethodKey = "AVALANCHE",
                    Title = "Tuyết Lở (Debt Avalanche)",
                    SubTitle = "Ưu tiên triệt tiêu món nợ lãi cao nhất trước để đỡ tốn tiền",
                    HowItWorks = "Bạn sắp xếp các món nợ theo lãi suất từ cao xuống thấp. Bạn trả mức tối thiểu cho tất cả, rồi dồn toàn bộ tiền dư vào món có lãi suất 'cắt cổ' nhất (thường là thẻ tín dụng 25-36%/năm).",
                    BestForWhom = "Phù hợp nhất cho người bình tĩnh, có tính kỷ luật cao, muốn tiết kiệm tối đa từng đồng tiền lãi trả cho ngân hàng.",
                    RealLifeExample = "Bạn có nợ thẻ tín dụng lãi 26%/năm và nợ trả góp lãi 14.5%/năm. Avalanche sẽ dồn toàn lực đè bẹp thẻ tín dụng trước, giúp bạn không bị bòn rút tiền lãi mỗi tháng.",
                    Pros = new List<string>
                    {
                        "Tiết kiệm tối đa số tiền lãi phải trả về mặt toán học.",
                        "Rút ngắn tổng thời gian sạch nợ đến mức tối đa.",
                        "Bảo vệ bạn khỏi cạm bẫy lãi kép của thẻ tín dụng."
                    },
                    Cons = new List<string>
                    {
                        "Cần kiên nhẫn hơn vì món nợ lãi cao có thể có số dư lớn, mất nhiều tháng mới trả xong món đầu tiên."
                    }
                }
            },
            QuizQuestions = new List<DebtQuizQuestionModel>
            {
                new DebtQuizQuestionModel
                {
                    QuestionId = "Q1",
                    QuestionText = "Khi nghĩ đến các khoản nợ hiện tại, điều gì làm bạn bận tâm nhất?",
                    Options = new List<DebtQuizOptionModel>
                    {
                        new DebtQuizOptionModel
                        {
                            OptionKey = "STRESS",
                            OptionText = "Tôi bị mệt mỏi và áp lực vì có quá nhiều bên đòi nợ, tôi muốn xóa bớt danh sách nợ càng sớm càng tốt.",
                            TargetStrategy = "SNOWBALL",
                            Explanation = "Tâm lý thoải mái là ưu tiên số 1 của bạn lúc này. Hãy chọn Snowball để tận hưởng niềm vui xóa sổ từng khoản nợ một!"
                        },
                        new DebtQuizOptionModel
                        {
                            OptionKey = "SAVINGS",
                            OptionText = "Tôi tiếc tiền và xót xa khi thấy mỗi tháng bị trừ quá nhiều tiền lãi vô lý cho ngân hàng.",
                            TargetStrategy = "AVALANCHE",
                            Explanation = "Bạn là người thực tế và coi trọng từng đồng tiền mồ hôi nước mắt. Hãy chọn Avalanche để chặn đứng việc tiền lãi phát sinh!"
                        }
                    }
                }
            },
            SafeBudgetGuideText = "Ngân sách trả thêm an toàn là số tiền thặng dư sau khi lấy Thu nhập trừ đi Chi tiêu thiết yếu và Tiền trả nợ tối thiểu. Chúng tôi khuyên bạn chỉ nên trích 30% đến 50% thặng dư này để trả nợ thêm, phần còn lại giữ phòng thân.",
            GoldenRules = new List<string>
            {
                "Quy tắc 1: Luôn trả đủ số tiền tối thiểu (Minimum Payment) đúng hạn để không bị phạt và dính nợ xấu CIC.",
                "Quy tắc 2: Đừng dồn toàn bộ 100% tiền dư vào trả nợ, luôn giữ lại một khoản tiền phòng thân nhỏ (quỹ khẩn cấp).",
                "Quy tắc 3: Sau khi trả hết một món nợ, hãy dồn toàn bộ số tiền đó sang món nợ kế tiếp (Hiệu ứng hòn tuyết lăn)."
            }
        };

        return Task.FromResult(guide);
    }

    public Task<SafeExtraBudgetRecommendationModel> GetSafeBudgetRecommendationAsync()
    {
        decimal income = 22_000_000m;
        decimal expenses = 11_000_000m;
        decimal debtMin = _debts.Where(d => d.Status == DebtStatus.Active).Sum(d => d.MinimumPayment);
        decimal surplus = Math.Max(0, income - expenses - debtMin);

        var model = new SafeExtraBudgetRecommendationModel
        {
            MonthlyIncome = income,
            MonthlyEssentialExpenses = expenses,
            TotalMonthlyDebtMinimum = debtMin,
            MonthlySurplus = surplus,
            ConservativeAmount = Math.Round(surplus * 0.10m / 50000m) * 50000m,
            RecommendedAmount = Math.Round(surplus * 0.30m / 50000m) * 50000m,
            AggressiveAmount = Math.Round(surplus * 0.50m / 50000m) * 50000m,
            MaxSafeAmount = Math.Round(surplus * 0.70m / 50000m) * 50000m,
            AdviceText = $"Dựa trên thu chi thực tế của bạn (thặng dư ước tính {surplus:N0}đ/tháng), mức trả nợ thêm khuyên dùng là {Math.Round(surplus * 0.30m / 50000m) * 50000m:N0}đ/tháng để vừa sạch nợ nhanh vừa an toàn tài chính."
        };

        return Task.FromResult(model);
    }

    public Task<DebtPayoffComparisonModel> CompareStrategiesAsync(decimal extraBudget, List<DebtSimulationItemModel>? customDebts = null)
    {
        var snowball = SimulateLocal(extraBudget, "SNOWBALL", customDebts);
        var avalanche = SimulateLocal(extraBudget, "AVALANCHE", customDebts);

        decimal interestSavings = Math.Max(0m, snowball.TotalInterestPaid - avalanche.TotalInterestPaid);
        int monthsSavings = Math.Max(0, snowball.TotalMonths - avalanche.TotalMonths);

        string recommendation;
        string recommendedStrategy;
        string laymanAdvice;

        if (interestSavings >= 1_000_000m)
        {
            recommendedStrategy = "AVALANCHE";
            recommendation = $"Avalanche là chiến lược tối ưu nhất. Bạn sẽ tiết kiệm được {interestSavings:N0}đ tiền lãi và về đích sớm hơn {monthsSavings} tháng so với Snowball.";
            laymanAdvice = $"Bạn nên chọn Tuyết Lở (Avalanche)! Cách này giúp bạn tiết kiệm được tới {interestSavings:N0}đ tiền lãi trả cho ngân hàng. Hãy tập trung dồn tiền đè bẹp các khoản có lãi suất cao nhất trước nhé!";
        }
        else
        {
            recommendedStrategy = "SNOWBALL";
            recommendation = $"Chênh lệch tiền lãi giữa hai phương pháp không đáng kể ({interestSavings:N0}đ). Khuyên dùng Snowball để giải phóng các khoản nợ nhỏ sớm nhất và tạo động lực tinh thần.";
            laymanAdvice = $"Bạn nên chọn Hòn Tuyết Lăn (Snowball)! Khoản tiền lãi tiết kiệm được giữa 2 cách chỉ chênh lệch khoảng {interestSavings:N0}đ. Chọn Snowball sẽ giúp bạn nhanh chóng xóa sổ dứt điểm các khoản nợ nhỏ trước, bớt đi người đòi nợ và cảm thấy nhẹ đầu hơn rất nhiều!";
        }

        var comparison = new DebtPayoffComparisonModel
        {
            Snowball = snowball,
            Avalanche = avalanche,
            InterestSavingsWithAvalanche = interestSavings,
            MonthsSavingsWithAvalanche = monthsSavings,
            Recommendation = recommendation,
            RecommendedStrategy = recommendedStrategy,
            LaymanRecommendationText = laymanAdvice
        };

        return Task.FromResult(comparison);
    }

    public Task<DebtStrategySimulationResultModel> SimulateStrategyAsync(string strategy, decimal extraBudget, List<DebtSimulationItemModel>? customDebts = null)
    {
        var result = SimulateLocal(extraBudget, strategy, customDebts);
        return Task.FromResult(result);
    }

    private DebtStrategySimulationResultModel SimulateLocal(decimal extraBudget, string strategy, List<DebtSimulationItemModel>? customDebts)
    {
        var items = new List<DebtSimulationItemModel>();
        if (customDebts != null && customDebts.Count > 0)
        {
            items.AddRange(customDebts);
        }
        else
        {
            items.AddRange(_debts.Where(d => d.Status == DebtStatus.Active && d.CurrentBalance > 0).Select(d => new DebtSimulationItemModel
            {
                DebtId = d.Id,
                Name = d.Name,
                Balance = d.CurrentBalance,
                InterestRateAnnual = d.InterestRateAnnual,
                MinimumPayment = d.MinimumPayment
            }));
        }

        if (items.Count == 0)
        {
            return new DebtStrategySimulationResultModel
            {
                Strategy = strategy,
                StrategyName = strategy == "SNOWBALL" ? "Hòn Tuyết Lăn" : "Tuyết Lở",
                TotalMonths = 0,
                IsDebtFree = true
            };
        }

        // Sắp xếp thứ tự ưu tiên
        if (strategy == "SNOWBALL")
            items = items.OrderBy(i => i.Balance).ToList();
        else
            items = items.OrderByDescending(i => i.InterestRateAnnual).ToList();

        decimal totalInterest = 0m;
        decimal totalPrincipal = 0m;
        int month = 0;
        var payoffOrder = new List<string>();
        decimal rollover = 0m;

        while (month < 360)
        {
            var active = items.Where(i => i.Balance > 0.01m).ToList();
            if (active.Count == 0) break;

            month++;
            decimal monthInterest = 0m;
            decimal monthPrincipal = 0m;

            // 1. Tính lãi tháng
            foreach (var item in active)
            {
                decimal monthlyRate = (item.InterestRateAnnual / 100m) / 12m;
                decimal interest = Math.Round(item.Balance * monthlyRate, 0, MidpointRounding.AwayFromZero);
                item.Balance += interest;
                monthInterest += interest;
                totalInterest += interest;
            }

            // 2. Trả min payment
            decimal extra = Math.Max(0m, extraBudget) + rollover;
            foreach (var item in active)
            {
                decimal pay = Math.Min(item.Balance, item.MinimumPayment);
                item.Balance -= pay;
                monthPrincipal += pay;
            }

            // 3. Dồn extra vào khoản ưu tiên
            DebtSimulationItemModel? target;
            if (strategy == "SNOWBALL")
                target = active.Where(i => i.Balance > 0.01m).OrderBy(i => i.Balance).FirstOrDefault();
            else
                target = active.Where(i => i.Balance > 0.01m).OrderByDescending(i => i.InterestRateAnnual).FirstOrDefault();

            if (target != null && extra > 0)
            {
                decimal extraPay = Math.Min(target.Balance, extra);
                target.Balance -= extraPay;
                monthPrincipal += extraPay;
            }

            totalPrincipal += monthPrincipal;

            // Kiểm tra tất toán
            foreach (var item in active)
            {
                if (item.Balance <= 0.01m && !payoffOrder.Contains(item.Name))
                {
                    item.Balance = 0m;
                    payoffOrder.Add(item.Name);
                    rollover += item.MinimumPayment;
                }
            }
        }

        return new DebtStrategySimulationResultModel
        {
            Strategy = strategy,
            StrategyName = strategy == "SNOWBALL" ? "Hòn Tuyết Lăn (Snowball)" : "Tuyết Lở (Avalanche)",
            Description = strategy == "SNOWBALL" 
                ? "Ưu tiên xóa sạch món nợ nhỏ nhất trước để giải phóng áp lực tâm lý." 
                : "Ưu tiên triệt tiêu món nợ lãi suất cao nhất trước để tiết kiệm tối đa tiền lãi.",
            TotalMonths = month,
            ProjectedPayoffDate = DateTime.UtcNow.AddMonths(month),
            TotalInterestPaid = Math.Round(totalInterest, 0),
            TotalPrincipalPaid = Math.Round(totalPrincipal, 0),
            TotalAmountPaid = Math.Round(totalInterest + totalPrincipal, 0),
            PayoffOrder = payoffOrder,
            IsDebtFree = items.All(i => i.Balance <= 0.01m),
            HasNegativeAmortization = false
        };
    }
}

