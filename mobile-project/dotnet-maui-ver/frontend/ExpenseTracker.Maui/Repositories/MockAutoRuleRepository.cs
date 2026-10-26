using ExpenseTracker.Maui.Models.AutoRules;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of IAutoRuleRepository for offline testing and demo
public class MockAutoRuleRepository : IAutoRuleRepository
{
    private readonly List<AutoRuleModel> _rules = new();

    public MockAutoRuleRepository()
    {
        var userId = Guid.NewGuid();

        _rules.Add(new AutoRuleModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            KeywordPattern: "WinMart, Co.opmart, Bách Hóa Xanh, Lotte",
            TargetCategoryId: Guid.Parse("b1111111-1111-1111-1111-111111111111"),
            TargetCategoryName: "Ăn uống & Thực phẩm",
            PriorityOrder: 1,
            IsRegex: false,
            IsCaseSensitive: false,
            CreatedAt: DateTime.UtcNow.AddDays(-20)
        ));

        _rules.Add(new AutoRuleModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            KeywordPattern: "Grab, Be, Gojek, Xăng, Petrolimex",
            TargetCategoryId: Guid.Parse("b3333333-3333-3333-3333-333333333333"),
            TargetCategoryName: "Di chuyển (Xăng, Grab)",
            PriorityOrder: 2,
            IsRegex: false,
            IsCaseSensitive: false,
            CreatedAt: DateTime.UtcNow.AddDays(-15)
        ));

        _rules.Add(new AutoRuleModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            KeywordPattern: "Highlands, Phúc Long, Starbucks, The Coffee House",
            TargetCategoryId: Guid.Parse("b4444444-4444-4444-4444-444444444444"),
            TargetCategoryName: "Cà phê & Hẹn hò",
            PriorityOrder: 3,
            IsRegex: false,
            IsCaseSensitive: false,
            CreatedAt: DateTime.UtcNow.AddDays(-10)
        ));

        _rules.Add(new AutoRuleModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            KeywordPattern: "Shopee, Lazada, TikTok Shop, Tiki",
            TargetCategoryId: Guid.Parse("b5555555-5555-5555-5555-555555555555"),
            TargetCategoryName: "Mua sắm & Đồ dùng",
            PriorityOrder: 4,
            IsRegex: false,
            IsCaseSensitive: false,
            CreatedAt: DateTime.UtcNow.AddDays(-8)
        ));

        _rules.Add(new AutoRuleModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            KeywordPattern: "Tiền điện, EVN, Tiền nước, Internet, FPT, Viettel",
            TargetCategoryId: Guid.Parse("b2222222-2222-2222-2222-222222222222"),
            TargetCategoryName: "Nhà ở & Tiện ích",
            PriorityOrder: 5,
            IsRegex: false,
            IsCaseSensitive: false,
            CreatedAt: DateTime.UtcNow.AddDays(-5)
        ));
    }

    public Task<List<AutoRuleModel>> GetRulesAsync()
    {
        return Task.FromResult(_rules.OrderBy(r => r.PriorityOrder).ToList());
    }

    public Task<AutoRuleModel> GetRuleByIdAsync(Guid id)
    {
        var rule = _rules.FirstOrDefault(r => r.Id == id)
            ?? throw new KeyNotFoundException($"Quy tắc với ID {id} không tồn tại.");
        return Task.FromResult(rule);
    }

    public Task<AutoRuleModel> CreateRuleAsync(CreateAutoRuleModel request)
    {
        var newRule = new AutoRuleModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            KeywordPattern: request.KeywordPattern,
            TargetCategoryId: request.TargetCategoryId,
            TargetCategoryName: "Danh mục",
            PriorityOrder: request.PriorityOrder,
            IsRegex: request.IsRegex,
            IsCaseSensitive: request.IsCaseSensitive,
            CreatedAt: DateTime.UtcNow
        );

        _rules.Add(newRule);
        return Task.FromResult(newRule);
    }

    public Task<AutoRuleModel> UpdateRuleAsync(Guid id, UpdateAutoRuleModel request)
    {
        var existing = _rules.FirstOrDefault(r => r.Id == id)
            ?? throw new KeyNotFoundException($"Quy tắc với ID {id} không tồn tại.");

        var index = _rules.IndexOf(existing);
        var updated = existing with
        {
            KeywordPattern = request.KeywordPattern,
            TargetCategoryId = request.TargetCategoryId,
            PriorityOrder = request.PriorityOrder,
            IsRegex = request.IsRegex,
            IsCaseSensitive = request.IsCaseSensitive
        };

        _rules[index] = updated;
        return Task.FromResult(updated);
    }

    public Task DeleteRuleAsync(Guid id)
    {
        var existing = _rules.FirstOrDefault(r => r.Id == id);
        if (existing != null) _rules.Remove(existing);
        return Task.CompletedTask;
    }

    public Task<MatchCategoryModel> MatchCategoryAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Task.FromResult(new MatchCategoryModel(null, null, null));

        var lower = text.ToLowerInvariant();
        foreach (var rule in _rules.OrderBy(r => r.PriorityOrder))
        {
            var keywords = rule.KeywordPattern.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var kw in keywords)
            {
                if (lower.Contains(kw.ToLowerInvariant()))
                {
                    return Task.FromResult(new MatchCategoryModel(rule.TargetCategoryId, rule.TargetCategoryName, kw));
                }
            }
        }

        return Task.FromResult(new MatchCategoryModel(null, null, null));
    }
}
