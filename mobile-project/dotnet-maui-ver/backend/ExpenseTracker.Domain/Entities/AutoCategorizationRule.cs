using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Smart categorization rule entity matching transaction notes/receipt items to categories automatically (UC-04.07)
public class AutoCategorizationRule : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string KeywordPattern { get; private set; } = string.Empty;
    public Guid TargetCategoryId { get; private set; }
    public Category TargetCategory { get; private set; } = null!;

    public int PriorityOrder { get; private set; }
    public bool IsRegex { get; private set; }
    public bool IsCaseSensitive { get; private set; }

    // Parameterless constructor for EF Core materialization
    private AutoCategorizationRule() { }

    public AutoCategorizationRule(
        Guid userId,
        string keywordPattern,
        Guid targetCategoryId,
        int priorityOrder = 0,
        bool isRegex = false,
        bool isCaseSensitive = false)
    {
        if (string.IsNullOrWhiteSpace(keywordPattern) || keywordPattern.Trim().Length < 2)
            throw new ArgumentException("Vui lòng nhập từ khóa nhận diện hợp lệ (tối thiểu 2 ký tự).", nameof(keywordPattern));

        if (targetCategoryId == Guid.Empty)
            throw new ArgumentException("Danh mục đích không được để trống.", nameof(targetCategoryId));

        if (priorityOrder < 0)
            throw new ArgumentException("Thứ tự ưu tiên không được âm.", nameof(priorityOrder));

        UserId = userId;
        KeywordPattern = keywordPattern.Trim();
        TargetCategoryId = targetCategoryId;
        PriorityOrder = priorityOrder;
        IsRegex = isRegex;
        IsCaseSensitive = isCaseSensitive;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string keywordPattern,
        Guid targetCategoryId,
        int priorityOrder,
        bool isRegex,
        bool isCaseSensitive)
    {
        if (string.IsNullOrWhiteSpace(keywordPattern) || keywordPattern.Trim().Length < 2)
            throw new ArgumentException("Vui lòng nhập từ khóa nhận diện hợp lệ (tối thiểu 2 ký tự).", nameof(keywordPattern));

        if (targetCategoryId == Guid.Empty)
            throw new ArgumentException("Danh mục đích không được để trống.", nameof(targetCategoryId));

        if (priorityOrder < 0)
            throw new ArgumentException("Thứ tự ưu tiên không được âm.", nameof(priorityOrder));

        KeywordPattern = keywordPattern.Trim();
        TargetCategoryId = targetCategoryId;
        PriorityOrder = priorityOrder;
        IsRegex = isRegex;
        IsCaseSensitive = isCaseSensitive;
        UpdatedAt = DateTime.UtcNow;
    }
}
