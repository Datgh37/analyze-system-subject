using ExpenseTracker.Maui.Models.RecurringIncomes;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of IRecurringIncomeRepository for offline testing and demo
public class MockRecurringIncomeRepository : IRecurringIncomeRepository
{
    private readonly List<RecurringIncomeModel> _items = new();

    public MockRecurringIncomeRepository()
    {
        var userId = Guid.NewGuid();
        var vcbId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var salaryCatId = Guid.Parse("a1111111-1111-1111-1111-111111111111");

        _items.Add(new RecurringIncomeModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            Name: "Lương Công Ty Công Nghệ",
            ExpectedAmount: 18000000m,
            WalletId: vcbId,
            WalletName: "Tài khoản Vietcombank",
            CategoryId: salaryCatId,
            CategoryName: "Tiền lương chính",
            CategoryIcon: "salary",
            CategoryColor: "#10B981",
            Frequency: 3, // Monthly
            DayOfMonth: 5,
            AutoRecord: true,
            IsActive: true,
            NextDueDate: DateTime.Today.AddDays(10),
            LastGeneratedDate: DateTime.Today.AddDays(-20),
            CreatedAt: DateTime.UtcNow.AddMonths(-3)
        ));

        _items.Add(new RecurringIncomeModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            Name: "Tiền Cho Thuê Nhà Trọ",
            ExpectedAmount: 7000000m,
            WalletId: vcbId,
            WalletName: "Tài khoản Vietcombank",
            CategoryId: Guid.Parse("a4444444-4444-4444-4444-444444444444"),
            CategoryName: "Thu nhập phụ khác",
            CategoryIcon: "home",
            CategoryColor: "#059669",
            Frequency: 3, // Monthly
            DayOfMonth: 15,
            AutoRecord: true,
            IsActive: true,
            NextDueDate: DateTime.Today.AddDays(20),
            LastGeneratedDate: DateTime.Today.AddDays(-10),
            CreatedAt: DateTime.UtcNow.AddMonths(-2)
        ));
    }

    public Task<List<RecurringIncomeModel>> GetRecurringIncomesAsync()
    {
        return Task.FromResult(_items.ToList());
    }

    public Task<RecurringIncomeModel> GetRecurringIncomeByIdAsync(Guid id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id)
            ?? throw new KeyNotFoundException($"Khoản thu định kỳ với ID {id} không tồn tại.");
        return Task.FromResult(item);
    }

    public Task<RecurringIncomeModel> CreateRecurringIncomeAsync(CreateRecurringIncomeModel request)
    {
        var newItem = new RecurringIncomeModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Name: request.Name,
            ExpectedAmount: request.ExpectedAmount,
            WalletId: request.WalletId,
            WalletName: "Ví Chỉ Định",
            CategoryId: request.CategoryId,
            CategoryName: "Thu nhập",
            CategoryIcon: "income",
            CategoryColor: "#10B981",
            Frequency: request.Frequency,
            DayOfMonth: request.DayOfMonth,
            AutoRecord: request.AutoRecord,
            IsActive: true,
            NextDueDate: DateTime.Today.AddDays(30),
            LastGeneratedDate: null,
            CreatedAt: DateTime.UtcNow
        );

        _items.Add(newItem);
        return Task.FromResult(newItem);
    }

    public Task<RecurringIncomeModel> UpdateRecurringIncomeAsync(Guid id, UpdateRecurringIncomeModel request)
    {
        var existing = _items.FirstOrDefault(i => i.Id == id)
            ?? throw new KeyNotFoundException($"Khoản thu định kỳ với ID {id} không tồn tại.");

        var index = _items.IndexOf(existing);
        var updated = existing with
        {
            Name = request.Name,
            ExpectedAmount = request.ExpectedAmount,
            WalletId = request.WalletId,
            CategoryId = request.CategoryId,
            Frequency = request.Frequency,
            DayOfMonth = request.DayOfMonth,
            AutoRecord = request.AutoRecord
        };

        _items[index] = updated;
        return Task.FromResult(updated);
    }

    public Task<RecurringIncomeModel> ToggleRecurringIncomeStatusAsync(Guid id)
    {
        var existing = _items.FirstOrDefault(i => i.Id == id)
            ?? throw new KeyNotFoundException($"Khoản thu định kỳ với ID {id} không tồn tại.");

        var index = _items.IndexOf(existing);
        var updated = existing with { IsActive = !existing.IsActive };
        _items[index] = updated;

        return Task.FromResult(updated);
    }

    public Task DeleteRecurringIncomeAsync(Guid id)
    {
        var existing = _items.FirstOrDefault(i => i.Id == id);
        if (existing != null) _items.Remove(existing);
        return Task.CompletedTask;
    }

    public Task<ProcessRecurringIncomeResultModel> ProcessDueRecurringIncomesAsync(ProcessRecurringIncomesModel? request = null)
    {
        var active = _items.Where(i => i.IsActive).ToList();
        var total = active.Sum(i => i.ExpectedAmount);
        var ids = active.Select(_ => Guid.NewGuid()).ToList();

        var result = new ProcessRecurringIncomeResultModel(
            ProcessedCount: active.Count,
            TotalAmountRecorded: total,
            GeneratedTransactionIds: ids,
            Message: $"Đã xử lý thành công {active.Count} khoản thu định kỳ (Tổng cộng: {total:N0} đ)."
        );

        return Task.FromResult(result);
    }
}
