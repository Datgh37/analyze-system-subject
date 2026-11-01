using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Dịch vụ quản lý danh mục thuê bao định kỳ và động cơ quét bẫy chi tiêu thông minh (UC-03.09)
public class SubscriptionService : ISubscriptionService
{
    private readonly IApplicationDbContext _context;

    public SubscriptionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriptionDto>> GetSubscriptionsAsync(Guid userId, SubscriptionStatus? status = null, CancellationToken ct = default)
    {
        var query = _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.Wallet)
            .Include(s => s.Category)
            .Where(s => s.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        var list = await query
            .OrderBy(s => s.NextBillingDate)
            .ToListAsync(ct);

        return list.Select(MapToDto).ToList();
    }

    public async Task<SubscriptionDto?> GetSubscriptionByIdAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var item = await _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.Wallet)
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, ct);

        return item == null ? null : MapToDto(item);
    }

    public async Task<SubscriptionDto> CreateSubscriptionAsync(Guid userId, CreateSubscriptionDto request, CancellationToken ct = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.UserId == null || c.UserId == userId), ct);

        if (category == null)
            throw new ArgumentException("Danh mục chi tiêu không tồn tại hoặc không hợp lệ.", nameof(request.CategoryId));

        if (request.WalletId.HasValue && request.WalletId.Value != Guid.Empty)
        {
            var walletExists = await _context.Wallets
                .AnyAsync(w => w.Id == request.WalletId.Value && w.UserId == userId, ct);

            if (!walletExists)
                throw new ArgumentException("Ví thanh toán liên kết không tồn tại hoặc không thuộc quyền sở hữu.", nameof(request.WalletId));
        }

        var subscription = new Subscription(
            userId,
            request.Name,
            request.Amount,
            request.BillingCycle,
            request.NextBillingDate,
            request.CategoryId,
            request.WalletId,
            request.Status,
            request.TrialEndDate,
            request.AutoRenew,
            request.RemindDaysBefore,
            request.Notes);

        await _context.Subscriptions.AddAsync(subscription, ct);
        await _context.SaveChangesAsync(ct);

        return await GetSubscriptionByIdAsync(userId, subscription.Id, ct)
               ?? MapToDto(subscription);
    }

    public async Task<SubscriptionDto> UpdateSubscriptionAsync(Guid userId, Guid id, UpdateSubscriptionDto request, CancellationToken ct = default)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, ct);

        if (subscription == null)
            throw new KeyNotFoundException("Không tìm thấy thuê bao dịch vụ yêu cầu.");

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.UserId == null || c.UserId == userId), ct);

        if (category == null)
            throw new ArgumentException("Danh mục chi tiêu không tồn tại hoặc không hợp lệ.", nameof(request.CategoryId));

        if (request.WalletId.HasValue && request.WalletId.Value != Guid.Empty)
        {
            var walletExists = await _context.Wallets
                .AnyAsync(w => w.Id == request.WalletId.Value && w.UserId == userId, ct);

            if (!walletExists)
                throw new ArgumentException("Ví thanh toán liên kết không tồn tại hoặc không thuộc quyền sở hữu.", nameof(request.WalletId));
        }

        subscription.UpdateDetails(
            request.Name,
            request.Amount,
            request.BillingCycle,
            request.NextBillingDate,
            request.CategoryId,
            request.WalletId,
            request.Status,
            request.TrialEndDate,
            request.AutoRenew,
            request.RemindDaysBefore,
            request.Notes);

        await _context.SaveChangesAsync(ct);

        return await GetSubscriptionByIdAsync(userId, subscription.Id, ct)
               ?? MapToDto(subscription);
    }

    public async Task<bool> ArchiveSubscriptionAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, ct);

        if (subscription == null)
            throw new KeyNotFoundException("Không tìm thấy thuê bao dịch vụ yêu cầu.");

        subscription.Cancel();
        _context.Subscriptions.Remove(subscription); // Soft-delete handled by BaseEntity / DbContext
        await _context.SaveChangesAsync(ct);

        return true;
    }

    public async Task<SubscriptionAuditReportDto> AuditSubscriptionsAsync(Guid userId, CancellationToken ct = default)
    {
        // 1. Lấy danh sách thuê bao người dùng đang quản lý
        var managed = await GetSubscriptionsAsync(userId, null, ct);
        var activeManaged = managed.Where(m => m.Status == SubscriptionStatus.Active || m.Status == SubscriptionStatus.FreeTrial).ToList();

        // 2. Lấy dữ liệu chi tiêu trong 90 ngày gần nhất
        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
        var expenses = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.TransactionType == TransactionType.Expense && t.TransactionDate >= ninetyDaysAgo)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);

        var candidates = new List<SubscriptionCandidateDto>();

        // 3. Tầng 1: Khớp danh mục dịch vụ số thông dụng (Catalog Matching)
        var knownCatalogs = new[]
        {
            ("Netflix", 260_000m, "Dịch vụ xem phim trực tuyến"),
            ("Spotify", 59_000m, "Dịch vụ nghe nhạc trực tuyến"),
            ("iCloud", 19_000m, "Lưu trữ đám mây Apple"),
            ("YouTube Premium", 79_000m, "Thuê bao xem video không quảng cáo"),
            ("ChatGPT Plus", 500_000m, "Trợ lý trí tuệ nhân tạo OpenAI"),
            ("Adobe Creative Cloud", 750_000m, "Bộ công cụ sáng tạo đồ họa"),
            ("VieON", 69_000m, "Truyền hình trực tuyến Việt Nam"),
            ("FPT Play", 88_000m, "Truyền hình & Thể thao FPT"),
            ("Canva Pro", 149_000m, "Công cụ thiết kế nhanh Canva"),
            ("Microsoft 365", 149_000m, "Bộ công cụ văn phòng Microsoft")
        };

        foreach (var (svcName, defaultPrice, desc) in knownCatalogs)
        {
            // Kiểm tra xem dịch vụ này đã nằm trong danh sách đang quản lý chưa
            bool alreadyManaged = managed.Any(m => m.Name.Contains(svcName, StringComparison.OrdinalIgnoreCase));
            if (alreadyManaged) continue;

            var matches = expenses
                .Where(e => (e.Note != null && e.Note.Contains(svcName, StringComparison.OrdinalIgnoreCase))
                         || (e.Category != null && e.Category.Name.Contains(svcName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (matches.Count > 0)
            {
                var latest = matches[0];
                var amount = latest.Amount > 0 ? latest.Amount : defaultPrice;
                var annualCost = amount * 12m;

                candidates.Add(new SubscriptionCandidateDto
                {
                    ServiceName = svcName,
                    CategoryId = latest.CategoryId,
                    CategoryName = latest.Category?.Name ?? "Dịch vụ định kỳ",
                    SuggestedAmount = amount,
                    SuggestedBillingCycle = SubscriptionBillingCycle.Monthly,
                    OccurrencesCount = matches.Count,
                    LastChargedDate = latest.TransactionDate,
                    DetectionSource = "CatalogMatch",
                    AdviceMessage = $"Cú hích Thaler: Bạn đang tiêu tốn {annualCost:N0} đ mỗi năm cho '{svcName}'. Hãy kiểm tra xem bạn có thực sự sử dụng dịch vụ này hàng tuần hay không để tránh rò rỉ tài chính."
                });
            }
        }

        // 4. Tầng 2: Heuristic Recurring Pattern Clustering (Phát hiện các khoản cùng số tiền lặp lại chu kỳ)
        var groupedExpenses = expenses
            .Where(e => e.Amount >= 50_000m)
            .GroupBy(e => new { e.CategoryId, e.Amount })
            .Where(g => g.Count() >= 2)
            .ToList();

        foreach (var group in groupedExpenses)
        {
            var ordered = group.OrderByDescending(e => e.TransactionDate).ToList();
            var latest = ordered[0];

            // Nếu candidate này đã được match ở tầng 1 thì bỏ qua
            bool alreadyInCandidates = candidates.Any(c => c.CategoryId == group.Key.CategoryId && c.SuggestedAmount == group.Key.Amount);
            bool alreadyManaged = managed.Any(m => m.CategoryId == group.Key.CategoryId && m.Amount == group.Key.Amount);

            if (!alreadyInCandidates && !alreadyManaged)
            {
                // Kiểm tra khoảng cách ngày trung bình giữa các lần trừ
                var daysDiff = (ordered[0].TransactionDate - ordered[1].TransactionDate).TotalDays;
                if (daysDiff is >= 25 and <= 35)
                {
                    var name = !string.IsNullOrWhiteSpace(latest.Note) ? latest.Note : $"Gói {latest.Category?.Name ?? "Định kỳ"}";
                    var annualCost = group.Key.Amount * 12m;

                    candidates.Add(new SubscriptionCandidateDto
                    {
                        ServiceName = name,
                        CategoryId = latest.CategoryId,
                        CategoryName = latest.Category?.Name ?? "Chi phí lặp lại",
                        SuggestedAmount = group.Key.Amount,
                        SuggestedBillingCycle = SubscriptionBillingCycle.Monthly,
                        OccurrencesCount = ordered.Count,
                        LastChargedDate = latest.TransactionDate,
                        DetectionSource = "RecurringCluster",
                        AdviceMessage = $"Phát hiện giao dịch lặp lại {ordered.Count} lần (~{daysDiff:N0} ngày/lần): Tổng hao tổn ước tính {annualCost:N0} đ/năm."
                    });
                }
            }
        }

        // 5. Tầng 3: Kiểm tra Free Trial sắp hết hạn
        int freeTrialAlerts = managed.Count(m => m.Status == SubscriptionStatus.FreeTrial 
            && m.TrialEndDate.HasValue 
            && m.TrialEndDate.Value <= DateTime.UtcNow.AddDays(3));

        // 6. Tổng hợp các chỉ số tài chính
        decimal totalMonthly = activeManaged.Sum(m => m.MonthlyCost) + candidates.Sum(c => c.SuggestedAmount);
        decimal totalAnnual = totalMonthly * 12m;

        string healthAlert;
        if (candidates.Count > 0 || freeTrialAlerts > 0)
        {
            healthAlert = $"Cảnh báo bẫy thuê bao: Hệ thống phát hiện {candidates.Count} khoản chi lặp lại tiềm ẩn và {freeTrialAlerts} gói dùng thử sắp hết hạn! Tổng dòng tiền rò rỉ ước tính: {totalMonthly:N0} đ/tháng ({totalAnnual:N0} đ/năm).";
        }
        else if (activeManaged.Count > 0)
        {
            healthAlert = $"Bạn đang kiểm soát tốt {activeManaged.Count} thuê bao định kỳ, tiêu tốn {totalMonthly:N0} đ/tháng ({totalAnnual:N0} đ/năm).";
        }
        else
        {
            healthAlert = "Chưa phát hiện bẫy thuê bao hoặc khoản chi lặp lại nào gây thất thoát dòng tiền.";
        }

        return new SubscriptionAuditReportDto
        {
            ActiveSubscriptionsCount = activeManaged.Count,
            TotalMonthlyDrain = totalMonthly,
            TotalAnnualDrain = totalAnnual,
            ZombieSubscriptionsCount = candidates.Count,
            FreeTrialAlertsCount = freeTrialAlerts,
            ManagedSubscriptions = managed,
            DetectedCandidates = candidates,
            FinancialHealthAlert = healthAlert
        };
    }

    public async Task<SubscriptionDto> ConvertCandidateToSubscriptionAsync(Guid userId, CreateSubscriptionDto request, CancellationToken ct = default)
    {
        return await CreateSubscriptionAsync(userId, request, ct);
    }

    public async Task<List<UpcomingRenewalDto>> GetUpcomingRenewalsAsync(Guid userId, int daysAhead = 7, CancellationToken ct = default)
    {
        var targetDate = DateTime.UtcNow.Date.AddDays(daysAhead);
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);

        var list = await _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.Wallet)
            .Where(s => s.UserId == userId 
                     && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.FreeTrial)
                     && s.NextBillingDate.Date <= targetDate
                     && s.NextBillingDate.Date >= yesterday)
            .OrderBy(s => s.NextBillingDate)
            .ToListAsync(ct);

        return list.Select(s => new UpcomingRenewalDto
        {
            SubscriptionId = s.Id,
            Name = s.Name,
            Amount = s.Amount,
            NextBillingDate = s.NextBillingDate,
            DaysLeft = (s.NextBillingDate.Date - DateTime.UtcNow.Date).Days,
            WalletName = s.Wallet?.Name,
            IsFreeTrialEnding = s.Status == SubscriptionStatus.FreeTrial
        }).ToList();
    }

    private static SubscriptionDto MapToDto(Subscription s)
    {
        int daysLeft = (s.NextBillingDate.Date - DateTime.UtcNow.Date).Days;
        bool isExpiringSoon = daysLeft <= s.RemindDaysBefore;

        return new SubscriptionDto
        {
            Id = s.Id,
            Name = s.Name,
            Amount = s.Amount,
            BillingCycle = s.BillingCycle,
            BillingCycleName = s.BillingCycle switch
            {
                SubscriptionBillingCycle.Weekly => "Hàng tuần",
                SubscriptionBillingCycle.Monthly => "Hàng tháng",
                SubscriptionBillingCycle.Quarterly => "Hàng quý",
                SubscriptionBillingCycle.Yearly => "Hàng năm",
                _ => "Hàng tháng"
            },
            NextBillingDate = s.NextBillingDate,
            DaysUntilNextBilling = daysLeft,
            WalletId = s.WalletId,
            WalletName = s.Wallet?.Name,
            CategoryId = s.CategoryId,
            CategoryName = s.Category?.Name ?? "Dịch vụ định kỳ",
            Status = s.Status,
            StatusName = s.Status switch
            {
                SubscriptionStatus.Active => "Đang hoạt động",
                SubscriptionStatus.Cancelled => "Đã hủy gói",
                SubscriptionStatus.Paused => "Tạm ngưng",
                SubscriptionStatus.FreeTrial => "Dùng thử miễn phí",
                _ => "Đang hoạt động"
            },
            TrialEndDate = s.TrialEndDate,
            AutoRenew = s.AutoRenew,
            RemindDaysBefore = s.RemindDaysBefore,
            Notes = s.Notes,
            MonthlyCost = s.MonthlyCost,
            AnnualCost = s.AnnualCost,
            IsExpiringSoon = isExpiringSoon
        };
    }
}

