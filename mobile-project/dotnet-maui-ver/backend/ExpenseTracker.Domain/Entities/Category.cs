using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Category entity representing hierarchical financial taxonomy for income and expenses
public class Category : BaseEntity
{
    // Null if system-wide preset category, or specific Guid if custom created by user
    public Guid? UserId { get; private set; }
    public User? User { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public TransactionType TransactionType { get; private set; }

    // Recursive self-referencing relationship for multi-tier category hierarchy
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }
    public ICollection<Category> Children { get; private set; } = new List<Category>();

    // Visual presentation
    public string IconKey { get; private set; } = "folder";
    public string ColorHex { get; private set; } = "#10B981";
    public int DisplayOrder { get; private set; }
    public bool IsSystem { get; private set; }

    // Personal finance budgeting and income classifications
    public IncomeCategoryGroup? IncomeGroup { get; private set; }
    public BudgetRuleMapping? BudgetRuleMapping { get; private set; }

    // Navigation collection
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    // Parameterless constructor for EF Core materialization
    private Category() { }

    public Category(
        string name,
        TransactionType transactionType,
        Guid? userId = null,
        Guid? parentId = null,
        string iconKey = "folder",
        string colorHex = "#10B981",
        int displayOrder = 0,
        bool isSystem = false,
        IncomeCategoryGroup? incomeGroup = null,
        BudgetRuleMapping? budgetRuleMapping = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên danh mục không được để trống.", nameof(name));

        Name = name.Trim();
        TransactionType = transactionType;
        UserId = userId;
        ParentId = parentId;
        IconKey = string.IsNullOrWhiteSpace(iconKey) ? "folder" : iconKey.Trim();
        ColorHex = string.IsNullOrWhiteSpace(colorHex) ? "#10B981" : colorHex.Trim();
        DisplayOrder = displayOrder;
        IsSystem = isSystem;
        IncomeGroup = incomeGroup;
        BudgetRuleMapping = budgetRuleMapping ?? (transactionType == TransactionType.Income ? Enums.BudgetRuleMapping.IncomeSource : Enums.BudgetRuleMapping.Needs);
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        Guid? parentId,
        string iconKey,
        string colorHex,
        int displayOrder,
        IncomeCategoryGroup? incomeGroup,
        BudgetRuleMapping? budgetRuleMapping)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên danh mục không được để trống.", nameof(name));

        if (parentId == Id)
            throw new InvalidOperationException("Danh mục không thể làm cha của chính nó.");

        Name = name.Trim();
        ParentId = parentId;
        IconKey = string.IsNullOrWhiteSpace(iconKey) ? "folder" : iconKey.Trim();
        ColorHex = string.IsNullOrWhiteSpace(colorHex) ? "#10B981" : colorHex.Trim();
        DisplayOrder = displayOrder;
        IncomeGroup = incomeGroup;
        BudgetRuleMapping = budgetRuleMapping;
        UpdatedAt = DateTime.UtcNow;
    }
}
