using ExpenseTracker.Maui.Models.Categories;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of ICategoryRepository for offline demo and immediate UI preview
public class MockCategoryRepository : ICategoryRepository
{
    private readonly List<CategoryModel> _categories = new();

    public MockCategoryRepository()
    {
        var userId = Guid.NewGuid();

        // --- Income Categories (Type 1) ---
        var salaryId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
        _categories.Add(new CategoryModel(
            Id: salaryId,
            UserId: userId,
            Name: "Tiền lương chính",
            TransactionType: 1,
            ParentId: null,
            ParentName: null,
            IconKey: "salary",
            ColorHex: "#10B981",
            DisplayOrder: 1,
            IsSystem: true,
            IncomeGroup: 1,
            BudgetRuleMapping: null,
            SubCategories: new List<CategoryModel>
            {
                new(Guid.NewGuid(), userId, "Lương cơ bản", 1, salaryId, "Tiền lương chính", "cash", "#059669", 1, false, 1, null),
                new(Guid.NewGuid(), userId, "Làm thêm giờ (OT)", 1, salaryId, "Tiền lương chính", "time", "#047857", 2, false, 1, null)
            }
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("a2222222-2222-2222-2222-222222222222"),
            UserId: userId,
            Name: "Thưởng dự án & KPI",
            TransactionType: 1,
            ParentId: null,
            ParentName: null,
            IconKey: "bonus",
            ColorHex: "#059669",
            DisplayOrder: 2,
            IsSystem: true,
            IncomeGroup: 2,
            BudgetRuleMapping: null
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("a3333333-3333-3333-3333-333333333333"),
            UserId: userId,
            Name: "Lãi tiết kiệm & Đầu tư",
            TransactionType: 1,
            ParentId: null,
            ParentName: null,
            IconKey: "investment",
            ColorHex: "#047857",
            DisplayOrder: 3,
            IsSystem: true,
            IncomeGroup: 3,
            BudgetRuleMapping: null
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("a4444444-4444-4444-4444-444444444444"),
            UserId: userId,
            Name: "Thu nhập phụ khác",
            TransactionType: 1,
            ParentId: null,
            ParentName: null,
            IconKey: "extra_income",
            ColorHex: "#065F46",
            DisplayOrder: 4,
            IsSystem: false,
            IncomeGroup: 4,
            BudgetRuleMapping: null
        ));

        // --- Expense Categories (Type 2) ---
        var foodId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
        _categories.Add(new CategoryModel(
            Id: foodId,
            UserId: userId,
            Name: "Ăn uống & Thực phẩm",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "food",
            ColorHex: "#EF4444",
            DisplayOrder: 1,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 1, // Needs 50%
            SubCategories: new List<CategoryModel>
            {
                new(Guid.NewGuid(), userId, "Bữa chính hàng ngày", 2, foodId, "Ăn uống & Thực phẩm", "meal", "#DC2626", 1, false, null, 1),
                new(Guid.NewGuid(), userId, "Siêu thị & Đi chợ", 2, foodId, "Ăn uống & Thực phẩm", "cart", "#B91C1C", 2, false, null, 1)
            }
        ));

        var housingId = Guid.Parse("b2222222-2222-2222-2222-222222222222");
        _categories.Add(new CategoryModel(
            Id: housingId,
            UserId: userId,
            Name: "Nhà ở & Tiện ích",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "housing",
            ColorHex: "#3B82F6",
            DisplayOrder: 2,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 1, // Needs 50%
            SubCategories: new List<CategoryModel>
            {
                new(Guid.NewGuid(), userId, "Tiền điện & Nước", 2, housingId, "Nhà ở & Tiện ích", "utility", "#2563EB", 1, false, null, 1),
                new(Guid.NewGuid(), userId, "Internet & Truyền hình", 2, housingId, "Nhà ở & Tiện ích", "wifi", "#1D4ED8", 2, false, null, 1)
            }
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("b3333333-3333-3333-3333-333333333333"),
            UserId: userId,
            Name: "Di chuyển (Xăng, Grab)",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "transport",
            ColorHex: "#F59E0B",
            DisplayOrder: 3,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 1 // Needs 50%
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("b4444444-4444-4444-4444-444444444444"),
            UserId: userId,
            Name: "Cà phê & Hẹn hò",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "coffee",
            ColorHex: "#D97706",
            DisplayOrder: 4,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 2 // Wants 30%
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("b5555555-5555-5555-5555-555555555555"),
            UserId: userId,
            Name: "Mua sắm & Đồ dùng",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "shopping",
            ColorHex: "#EC4899",
            DisplayOrder: 5,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 2 // Wants 30%
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("b6666666-6666-6666-6666-666666666666"),
            UserId: userId,
            Name: "Học tập & Sách vở",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "education",
            ColorHex: "#8B5CF6",
            DisplayOrder: 6,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 2 // Wants 30%
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("b7777777-7777-7777-7777-777777777777"),
            UserId: userId,
            Name: "Y tế & Chăm sóc sức khỏe",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "health",
            ColorHex: "#14B8A6",
            DisplayOrder: 7,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 1 // Needs 50%
        ));

        _categories.Add(new CategoryModel(
            Id: Guid.Parse("b8888888-8888-8888-8888-888888888888"),
            UserId: userId,
            Name: "Giải trí & Du lịch",
            TransactionType: 2,
            ParentId: null,
            ParentName: null,
            IconKey: "entertainment",
            ColorHex: "#6366F1",
            DisplayOrder: 8,
            IsSystem: true,
            IncomeGroup: null,
            BudgetRuleMapping: 2 // Wants 30%
        ));
    }

    public Task<List<CategoryModel>> GetCategoriesAsync(int? transactionType = null)
    {
        var result = transactionType.HasValue
            ? _categories.Where(c => c.TransactionType == transactionType.Value).ToList()
            : _categories.ToList();

        return Task.FromResult(result);
    }

    public Task<CategoryModel> GetCategoryByIdAsync(Guid id)
    {
        var cat = _categories.FirstOrDefault(c => c.Id == id)
            ?? throw new KeyNotFoundException($"Danh mục với ID {id} không tồn tại.");
        return Task.FromResult(cat);
    }

    public Task<CategoryModel> CreateCategoryAsync(CreateCategoryModel request)
    {
        var parent = request.ParentId.HasValue
            ? _categories.FirstOrDefault(c => c.Id == request.ParentId.Value)
            : null;

        var newCat = new CategoryModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Name: request.Name,
            TransactionType: request.TransactionType,
            ParentId: request.ParentId,
            ParentName: parent?.Name,
            IconKey: request.IconKey,
            ColorHex: request.ColorHex,
            DisplayOrder: request.DisplayOrder,
            IsSystem: false,
            IncomeGroup: request.IncomeGroup,
            BudgetRuleMapping: request.BudgetRuleMapping
        );

        _categories.Add(newCat);
        return Task.FromResult(newCat);
    }

    public Task<CategoryModel> UpdateCategoryAsync(Guid id, UpdateCategoryModel request)
    {
        var existing = _categories.FirstOrDefault(c => c.Id == id)
            ?? throw new KeyNotFoundException($"Danh mục với ID {id} không tồn tại.");

        var index = _categories.IndexOf(existing);
        var updated = existing with
        {
            Name = request.Name,
            ParentId = request.ParentId,
            IconKey = request.IconKey,
            ColorHex = request.ColorHex,
            DisplayOrder = request.DisplayOrder,
            IncomeGroup = request.IncomeGroup,
            BudgetRuleMapping = request.BudgetRuleMapping
        };

        _categories[index] = updated;
        return Task.FromResult(updated);
    }

    public Task DeleteCategoryAsync(Guid id, Guid? targetCategoryId = null)
    {
        var existing = _categories.FirstOrDefault(c => c.Id == id);
        if (existing != null)
        {
            _categories.Remove(existing);
        }
        return Task.CompletedTask;
    }
}
