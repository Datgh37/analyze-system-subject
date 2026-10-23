using ExpenseTracker.Maui.Models.Transactions;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of ITransactionRepository for standalone testing and offline demo
public class MockTransactionRepository : ITransactionRepository
{
    private readonly List<TransactionModel> _transactions = new();

    public MockTransactionRepository()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var foodCatId = Guid.NewGuid();
        var transportCatId = Guid.NewGuid();
        var salaryCatId = Guid.NewGuid();

        _transactions.Add(new TransactionModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            WalletId: walletId,
            WalletName: "Ví Tiền Mặt",
            CategoryId: foodCatId,
            CategoryName: "Ăn uống",
            CategoryIcon: "food",
            CategoryColor: "#EF4444",
            Amount: 45000m,
            TransactionType: 2, // Expense
            TransactionDate: DateTime.Today,
            Note: "Ăn sáng phở bò",
            IsEssential: true,
            IsWindfall: false,
            IsRecurring: false,
            ExcludeFromReport: false,
            CreatedAt: DateTime.UtcNow.AddHours(-4)
        ));

        _transactions.Add(new TransactionModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            WalletId: walletId,
            WalletName: "Ví Tiền Mặt",
            CategoryId: transportCatId,
            CategoryName: "Di chuyển",
            CategoryIcon: "car",
            CategoryColor: "#3B82F6",
            Amount: 60000m,
            TransactionType: 2, // Expense
            TransactionDate: DateTime.Today,
            Note: "Đổ xăng xe máy",
            IsEssential: true,
            IsWindfall: false,
            IsRecurring: false,
            ExcludeFromReport: false,
            CreatedAt: DateTime.UtcNow.AddHours(-2)
        ));

        _transactions.Add(new TransactionModel(
            Id: Guid.NewGuid(),
            UserId: userId,
            WalletId: walletId,
            WalletName: "Ví Tiền Mặt",
            CategoryId: salaryCatId,
            CategoryName: "Tiền lương",
            CategoryIcon: "briefcase",
            CategoryColor: "#10B981",
            Amount: 18000000m,
            TransactionType: 1, // Income
            TransactionDate: DateTime.Today.AddDays(-5),
            Note: "Nhận lương tháng",
            IsEssential: true,
            IsWindfall: false,
            IsRecurring: true,
            ExcludeFromReport: false,
            CreatedAt: DateTime.UtcNow.AddDays(-5)
        ));
    }

    public Task<List<TransactionModel>> GetTransactionsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? walletId = null,
        Guid? categoryId = null,
        int? transactionType = null,
        int page = 1,
        int pageSize = 50)
    {
        var query = _transactions.AsEnumerable();

        if (walletId.HasValue) query = query.Where(t => t.WalletId == walletId.Value);
        if (categoryId.HasValue) query = query.Where(t => t.CategoryId == categoryId.Value);
        if (transactionType.HasValue) query = query.Where(t => t.TransactionType == transactionType.Value);
        if (fromDate.HasValue) query = query.Where(t => t.TransactionDate >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(t => t.TransactionDate <= toDate.Value);

        return Task.FromResult(query.OrderByDescending(t => t.TransactionDate).ToList());
    }

    public Task<TransactionModel> GetTransactionByIdAsync(Guid id)
    {
        var tx = _transactions.FirstOrDefault(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Không tìm thấy giao dịch {id}");
        return Task.FromResult(tx);
    }

    public Task<TransactionModel> CreateIncomeAsync(CreateIncomeModel request)
    {
        var model = new TransactionModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            WalletId: request.WalletId,
            WalletName: "Ví Demo",
            CategoryId: request.CategoryId,
            CategoryName: "Thu nhập",
            CategoryIcon: "cash",
            CategoryColor: "#10B981",
            Amount: request.Amount,
            TransactionType: 1,
            TransactionDate: request.TransactionDate,
            Note: request.Note,
            IsEssential: true,
            IsWindfall: request.IsWindfall,
            IsRecurring: request.IsRecurring,
            ExcludeFromReport: request.ExcludeFromReport,
            CreatedAt: DateTime.UtcNow
        );
        _transactions.Add(model);
        return Task.FromResult(model);
    }

    public Task<TransactionModel> CreateExpenseAsync(CreateExpenseModel request)
    {
        var model = new TransactionModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            WalletId: request.WalletId,
            WalletName: "Ví Demo",
            CategoryId: request.CategoryId,
            CategoryName: "Chi tiêu",
            CategoryIcon: "cart",
            CategoryColor: "#EF4444",
            Amount: request.Amount,
            TransactionType: 2,
            TransactionDate: request.TransactionDate,
            Note: request.Note,
            IsEssential: request.IsEssential,
            IsWindfall: false,
            IsRecurring: request.IsRecurring,
            ExcludeFromReport: request.ExcludeFromReport,
            CreatedAt: DateTime.UtcNow
        );
        _transactions.Add(model);
        return Task.FromResult(model);
    }

    public Task DeleteIncomeAsync(Guid id)
    {
        _transactions.RemoveAll(t => t.Id == id);
        return Task.CompletedTask;
    }

    public Task DeleteExpenseAsync(Guid id)
    {
        _transactions.RemoveAll(t => t.Id == id);
        return Task.CompletedTask;
    }

    public Task<decimal> GetMonthlyIncomeTotalAsync(int month, int year)
    {
        var total = _transactions
            .Where(t => t.TransactionType == 1 && t.TransactionDate.Month == month && t.TransactionDate.Year == year)
            .Sum(t => t.Amount);
        return Task.FromResult(total);
    }

    public Task<decimal> GetMonthlyExpenseTotalAsync(int month, int year)
    {
        var total = _transactions
            .Where(t => t.TransactionType == 2 && t.TransactionDate.Month == month && t.TransactionDate.Year == year)
            .Sum(t => t.Amount);
        return Task.FromResult(total);
    }
}
