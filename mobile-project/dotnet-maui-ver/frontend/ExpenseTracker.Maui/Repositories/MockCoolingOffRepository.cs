using ExpenseTracker.Maui.Models.Behavioral;

namespace ExpenseTracker.Maui.Repositories;

public class MockCoolingOffRepository : ICoolingOffRepository
{
    private readonly List<CoolingOffTransactionModel> _items = new();

    public MockCoolingOffRepository()
    {
        SeedMockData();
    }

    private void SeedMockData()
    {
        _items.Add(new CoolingOffTransactionModel
        {
            Id = Guid.Parse("aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            UserId = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            WalletName = "Ví Chính (Techcombank)",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Thiết Bị Công Nghệ",
            Amount = 4500000m,
            Description = "Tai nghe chống ồn Sony WH-1000XM5",
            CoolingOffUntil = DateTime.UtcNow.AddHours(18).AddMinutes(25),
            Status = CoolingOffStatus.PendingCoolingOff,
            EquivalentWorkHours = 48m,
            ProjectedDsaReduction = 45000m,
            CreatedAt = DateTime.UtcNow.AddHours(-5).AddMinutes(-35)
        });

        _items.Add(new CoolingOffTransactionModel
        {
            Id = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            UserId = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            WalletName = "Ví MoMo",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Thời Trang & Giày Dép",
            Amount = 3200000m,
            Description = "Giày chạy bộ Nike ZoomX Vaporfly",
            CoolingOffUntil = DateTime.UtcNow.AddMinutes(-10), // Expired - ready to confirm or cancel
            Status = CoolingOffStatus.PendingCoolingOff,
            EquivalentWorkHours = 34m,
            ProjectedDsaReduction = 32000m,
            CreatedAt = DateTime.UtcNow.AddHours(-24).AddMinutes(-10)
        });
    }

    public Task<CoolingOffEvaluationResultModel> EvaluateExpenseAsync(CoolingOffEvaluationRequestModel request)
    {
        bool requires = request.Amount >= 3000000m;
        decimal hourlyWage = 93750m; // ~15M monthly / 160h
        decimal workHours = Math.Round(request.Amount / hourlyWage, 1);
        decimal dsaReduction = Math.Round(request.Amount / 15m, 0);
        decimal currentDsa = 280000m;
        decimal dsaAfter = Math.Max(0m, currentDsa - dsaReduction);

        return Task.FromResult(new CoolingOffEvaluationResultModel
        {
            RequiresCoolingOff = requires,
            ThresholdAmount = 3000000m,
            CooldownHours = 24,
            EquivalentWorkHours = workHours,
            ProjectedDsaReduction = dsaReduction,
            CurrentDsa = currentDsa,
            DsaAfter = dsaAfter,
            ImpactAnalysisMessage = requires
                ? $"⚠️ KHOẢNG TRỄ TÂM LÝ 24H: Khoản chi {request.Amount:N0} đ tương đương với {workHours:N1} giờ lao động thực tế ({Math.Round(workHours / 8m, 1)} ngày công)! Hạn mức ngày sẽ giảm từ {currentDsa:N0} đ xuống còn {dsaAfter:N0} đ. Hãy để tâm trí hạ nhiệt 24 giờ trước khi thanh toán."
                : "Khoản chi nằm trong ngưỡng an toàn thông thường."
        });
    }

    public Task<CoolingOffTransactionModel> CreateCoolingOffAsync(CreateCoolingOffRequestModel request)
    {
        var model = new CoolingOffTransactionModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            WalletId = request.WalletId,
            WalletName = "Ví thanh toán",
            CategoryId = request.CategoryId,
            CategoryName = "Mua sắm",
            Amount = request.Amount,
            Description = request.Description,
            CoolingOffUntil = DateTime.UtcNow.AddHours(request.CooldownHours),
            Status = CoolingOffStatus.PendingCoolingOff,
            EquivalentWorkHours = Math.Round(request.Amount / 93750m, 1),
            ProjectedDsaReduction = Math.Round(request.Amount / 15m, 0),
            CreatedAt = DateTime.UtcNow
        };

        _items.Add(model);
        return Task.FromResult(model);
    }

    public Task<List<CoolingOffTransactionModel>> GetPendingCoolingOffAsync()
    {
        return Task.FromResult(_items.Where(i => i.Status == CoolingOffStatus.PendingCoolingOff).ToList());
    }

    public Task<bool> ConfirmPurchaseAsync(Guid id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            item.Status = CoolingOffStatus.Confirmed;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> CancelPurchaseAsync(Guid id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            item.Status = CoolingOffStatus.Cancelled;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> BypassCoolingOffAsync(Guid id, BypassCoolingOffRequestModel request)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            item.Status = CoolingOffStatus.Bypassed;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
