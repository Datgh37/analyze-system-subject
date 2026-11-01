using ExpenseTracker.Maui.Models.SmartUtilities;

namespace ExpenseTracker.Maui.Repositories;

public class MockSubscriptionRepository : ISubscriptionRepository
{
    private readonly List<SubscriptionModel> _subscriptions;

    public MockSubscriptionRepository()
    {
        _subscriptions = new List<SubscriptionModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Netflix Premium",
                Amount = 260_000m,
                BillingCycle = 2,
                BillingCycleName = "Hàng tháng",
                NextBillingDate = DateTime.UtcNow.AddDays(5),
                DaysUntilNextBilling = 5,
                CategoryName = "Giải trí",
                Status = 1,
                StatusName = "Đang hoạt động",
                AutoRenew = true,
                RemindDaysBefore = 3,
                MonthlyCost = 260_000m,
                AnnualCost = 3_120_000m,
                IsExpiringSoon = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Apple iCloud 200GB",
                Amount = 59_000m,
                BillingCycle = 2,
                BillingCycleName = "Hàng tháng",
                NextBillingDate = DateTime.UtcNow.AddDays(2),
                DaysUntilNextBilling = 2,
                CategoryName = "Lưu trữ đám mây",
                Status = 1,
                StatusName = "Đang hoạt động",
                AutoRenew = true,
                RemindDaysBefore = 3,
                MonthlyCost = 59_000m,
                AnnualCost = 708_000m,
                IsExpiringSoon = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Spotify Family",
                Amount = 89_000m,
                BillingCycle = 2,
                BillingCycleName = "Hàng tháng",
                NextBillingDate = DateTime.UtcNow.AddDays(15),
                DaysUntilNextBilling = 15,
                CategoryName = "Âm nhạc",
                Status = 1,
                StatusName = "Đang hoạt động",
                AutoRenew = true,
                RemindDaysBefore = 3,
                MonthlyCost = 89_000m,
                AnnualCost = 1_068_000m,
                IsExpiringSoon = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Canva Pro Dùng Thử",
                Amount = 149_000m,
                BillingCycle = 2,
                BillingCycleName = "Hàng tháng",
                NextBillingDate = DateTime.UtcNow.AddDays(2),
                DaysUntilNextBilling = 2,
                CategoryName = "Công cụ thiết kế",
                Status = 4,
                StatusName = "Dùng thử miễn phí",
                TrialEndDate = DateTime.UtcNow.AddDays(2),
                AutoRenew = true,
                RemindDaysBefore = 3,
                MonthlyCost = 149_000m,
                AnnualCost = 1_788_000m,
                IsExpiringSoon = true
            }
        };
    }

    public Task<List<SubscriptionModel>> GetSubscriptionsAsync(int? status = null)
    {
        var list = status.HasValue
            ? _subscriptions.Where(s => s.Status == status.Value).ToList()
            : _subscriptions.ToList();
        return Task.FromResult(list);
    }

    public Task<SubscriptionModel> GetSubscriptionByIdAsync(Guid id)
    {
        var item = _subscriptions.FirstOrDefault(s => s.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy thuê bao.");
        return Task.FromResult(item);
    }

    public Task<SubscriptionModel> CreateSubscriptionAsync(CreateSubscriptionRequest request)
    {
        var model = new SubscriptionModel
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Amount = request.Amount,
            BillingCycle = request.BillingCycle,
            BillingCycleName = request.BillingCycle switch
            {
                1 => "Hàng tuần",
                2 => "Hàng tháng",
                3 => "Hàng quý",
                4 => "Hàng năm",
                _ => "Hàng tháng"
            },
            NextBillingDate = request.NextBillingDate,
            DaysUntilNextBilling = (request.NextBillingDate.Date - DateTime.UtcNow.Date).Days,
            CategoryId = request.CategoryId,
            CategoryName = "Dịch vụ định kỳ",
            WalletId = request.WalletId,
            Status = request.Status,
            StatusName = request.Status == 4 ? "Dùng thử miễn phí" : "Đang hoạt động",
            TrialEndDate = request.TrialEndDate,
            AutoRenew = request.AutoRenew,
            RemindDaysBefore = request.RemindDaysBefore,
            Notes = request.Notes,
            MonthlyCost = request.Amount,
            AnnualCost = request.Amount * 12m,
            IsExpiringSoon = false
        };

        _subscriptions.Add(model);
        return Task.FromResult(model);
    }

    public Task<SubscriptionModel> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request)
    {
        var existing = _subscriptions.FirstOrDefault(s => s.Id == id)
            ?? throw new KeyNotFoundException("Không tìm thấy thuê bao.");

        existing.Name = request.Name;
        existing.Amount = request.Amount;
        existing.BillingCycle = request.BillingCycle;
        existing.NextBillingDate = request.NextBillingDate;
        existing.DaysUntilNextBilling = (request.NextBillingDate.Date - DateTime.UtcNow.Date).Days;
        existing.Status = request.Status;
        existing.TrialEndDate = request.TrialEndDate;
        existing.AutoRenew = request.AutoRenew;
        existing.RemindDaysBefore = request.RemindDaysBefore;
        existing.Notes = request.Notes;
        existing.MonthlyCost = request.Amount;
        existing.AnnualCost = request.Amount * 12m;

        return Task.FromResult(existing);
    }

    public Task<bool> ArchiveSubscriptionAsync(Guid id)
    {
        var existing = _subscriptions.FirstOrDefault(s => s.Id == id);
        if (existing != null)
        {
            existing.Status = 2; // Cancelled
            _subscriptions.Remove(existing);
        }
        return Task.FromResult(true);
    }

    public Task<SubscriptionAuditReportModel> AuditSubscriptionsAsync()
    {
        var active = _subscriptions.Where(s => s.Status == 1 || s.Status == 4).ToList();
        var monthlyDrain = active.Sum(s => s.MonthlyCost);
        var annualDrain = active.Sum(s => s.AnnualCost);

        var report = new SubscriptionAuditReportModel
        {
            ActiveSubscriptionsCount = active.Count,
            TotalMonthlyDrain = monthlyDrain,
            TotalAnnualDrain = annualDrain,
            ZombieSubscriptionsCount = 1,
            FreeTrialAlertsCount = active.Count(s => s.Status == 4),
            ManagedSubscriptions = active,
            DetectedCandidates = new List<SubscriptionCandidateModel>
            {
                new()
                {
                    ServiceName = "ChatGPT Plus",
                    CategoryName = "Trí tuệ nhân tạo",
                    SuggestedAmount = 500_000m,
                    SuggestedBillingCycle = 2,
                    OccurrencesCount = 2,
                    LastChargedDate = DateTime.UtcNow.AddDays(-28),
                    DetectionSource = "CatalogMatch",
                    AdviceMessage = "Cú hích Thaler: Bạn đang chi 6.000.000 đ/năm cho ChatGPT Plus. Hãy kiểm tra mức độ sử dụng."
                }
            },
            FinancialHealthAlert = $"Phát hiện 1 ứng viên bẫy thuê bao lặp lại và 1 gói dùng thử sắp hết hạn! Tổng chi phí: {monthlyDrain + 500_000m:N0} đ/tháng."
        };

        return Task.FromResult(report);
    }

    public Task<SubscriptionModel> ConvertCandidateAsync(CreateSubscriptionRequest request)
    {
        return CreateSubscriptionAsync(request);
    }

    public Task<List<UpcomingRenewalModel>> GetUpcomingRenewalsAsync(int daysAhead = 7)
    {
        var list = _subscriptions
            .Where(s => s.DaysUntilNextBilling <= daysAhead && s.DaysUntilNextBilling >= 0)
            .Select(s => new UpcomingRenewalModel
            {
                SubscriptionId = s.Id,
                Name = s.Name,
                Amount = s.Amount,
                NextBillingDate = s.NextBillingDate,
                DaysLeft = s.DaysUntilNextBilling,
                IsFreeTrialEnding = s.Status == 4
            })
            .ToList();

        return Task.FromResult(list);
    }
}

