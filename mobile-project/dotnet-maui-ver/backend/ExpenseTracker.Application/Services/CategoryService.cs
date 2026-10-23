using ExpenseTracker.Application.DTOs.Categories;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Service implementing hierarchical category management and recursive subtree building
public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Retrieves system and user categories built into a recursive hierarchical tree
    public async Task<Result<List<CategoryDto>>> GetCategoriesAsync(Guid userId, TransactionType? transactionType = null, CancellationToken ct = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.IsSystem || c.UserId == userId);

        if (transactionType.HasValue)
        {
            query = query.Where(c => c.TransactionType == transactionType.Value);
        }

        var allCategories = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);

        // Build hierarchical tree: root categories have null ParentId
        var rootCategories = allCategories
            .Where(c => c.ParentId == null)
            .Select(c => MapToTreeDto(c, allCategories))
            .ToList();

        return Result<List<CategoryDto>>.Success(rootCategories);
    }

    // Retrieves single category details
    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(Guid userId, Guid categoryId, CancellationToken ct = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == categoryId && (c.IsSystem || c.UserId == userId), ct);

        if (category == null)
            return Result<CategoryDto>.Failure("Danh mục không tồn tại hoặc bạn không có quyền truy cập.", 404);

        var dto = new CategoryDto(
            category.Id,
            category.UserId,
            category.Name,
            category.TransactionType,
            category.ParentId,
            category.Parent?.Name,
            category.IconKey,
            category.ColorHex,
            category.DisplayOrder,
            category.IsSystem,
            category.IncomeGroup,
            category.BudgetRuleMapping
        );

        return Result<CategoryDto>.Success(dto);
    }

    // Creates custom user category
    public async Task<Result<CategoryDto>> CreateCategoryAsync(Guid userId, CreateCategoryRequest request, CancellationToken ct = default)
    {
        if (request.ParentId.HasValue)
        {
            var parent = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value && (c.IsSystem || c.UserId == userId), ct);

            if (parent == null)
                return Result<CategoryDto>.Failure("Danh mục cha không tồn tại.");

            if (parent.TransactionType != request.TransactionType)
                return Result<CategoryDto>.Failure("Danh mục cha phải cùng loại giao dịch (Thu nhập / Chi tiêu).");

            var parentDepth = await GetDepthAsync(request.ParentId.Value, ct);
            if (parentDepth >= 3)
                return Result<CategoryDto>.Failure("Hệ thống chỉ hỗ trợ tối đa 3 tầng phân cấp danh mục để bảo đảm tính rõ ràng của báo cáo tài chính.");
        }

        var trimmedName = request.Name.Trim();
        var isDuplicate = await _context.Categories
            .AnyAsync(c => (c.UserId == userId || c.IsSystem)
                        && c.ParentId == request.ParentId
                        && c.TransactionType == request.TransactionType
                        && c.Name.ToLower() == trimmedName.ToLower(), ct);

        if (isDuplicate)
            return Result<CategoryDto>.Failure("Tên danh mục này đã tồn tại trong cùng nhóm. Vui lòng chọn tên khác.");

        var category = new Category(
            name: trimmedName,
            transactionType: request.TransactionType,
            userId: userId,
            parentId: request.ParentId,
            iconKey: request.IconKey,
            colorHex: request.ColorHex,
            displayOrder: request.DisplayOrder,
            isSystem: false,
            incomeGroup: request.IncomeGroup,
            budgetRuleMapping: request.BudgetRuleMapping
        );

        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct);

        var dto = new CategoryDto(
            category.Id,
            category.UserId,
            category.Name,
            category.TransactionType,
            category.ParentId,
            null,
            category.IconKey,
            category.ColorHex,
            category.DisplayOrder,
            category.IsSystem,
            category.IncomeGroup,
            category.BudgetRuleMapping
        );

        return Result<CategoryDto>.Success(dto);
    }

    // Updates custom user category
    public async Task<Result<CategoryDto>> UpdateCategoryAsync(Guid userId, Guid categoryId, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId, ct);

        if (category == null)
            return Result<CategoryDto>.Failure("Không tìm thấy danh mục hoặc bạn không có quyền chỉnh sửa danh mục hệ thống.", 404);

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == categoryId)
                return Result<CategoryDto>.Failure("Không thể chọn danh mục này làm danh mục cha vì sẽ tạo ra vòng lặp phân cấp bất hợp lệ.");

            var parent = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value && (c.IsSystem || c.UserId == userId), ct);

            if (parent == null)
                return Result<CategoryDto>.Failure("Danh mục cha không tồn tại.");

            if (parent.TransactionType != category.TransactionType)
                return Result<CategoryDto>.Failure("Danh mục cha phải cùng loại giao dịch.");

            // Check DAG cycle: parent cannot be descendant of categoryId
            if (await HasCycleAsync(categoryId, request.ParentId.Value, ct))
                return Result<CategoryDto>.Failure("Không thể chọn danh mục này làm danh mục cha vì sẽ tạo ra vòng lặp phân cấp bất hợp lệ.");

            // Check max 3-tier hierarchy depth
            var parentDepth = await GetDepthAsync(request.ParentId.Value, ct);
            var subtreeHeight = await GetSubtreeMaxHeightAsync(categoryId, ct);
            if (parentDepth + subtreeHeight > 3)
                return Result<CategoryDto>.Failure("Hệ thống chỉ hỗ trợ tối đa 3 tầng phân cấp danh mục để bảo đảm tính rõ ràng của báo cáo tài chính.");
        }

        var trimmedName = request.Name.Trim();
        var isDuplicate = await _context.Categories
            .AnyAsync(c => c.Id != categoryId
                        && (c.UserId == userId || c.IsSystem)
                        && c.ParentId == request.ParentId
                        && c.TransactionType == category.TransactionType
                        && c.Name.ToLower() == trimmedName.ToLower(), ct);

        if (isDuplicate)
            return Result<CategoryDto>.Failure("Tên danh mục này đã tồn tại trong cùng nhóm. Vui lòng chọn tên khác.");

        category.UpdateDetails(
            name: trimmedName,
            parentId: request.ParentId,
            iconKey: request.IconKey,
            colorHex: request.ColorHex,
            displayOrder: request.DisplayOrder,
            incomeGroup: request.IncomeGroup,
            budgetRuleMapping: request.BudgetRuleMapping
        );

        await _context.SaveChangesAsync(ct);

        var dto = new CategoryDto(
            category.Id,
            category.UserId,
            category.Name,
            category.TransactionType,
            category.ParentId,
            null,
            category.IconKey,
            category.ColorHex,
            category.DisplayOrder,
            category.IsSystem,
            category.IncomeGroup,
            category.BudgetRuleMapping
        );

        return Result<CategoryDto>.Success(dto);
    }

    // Deletes custom user category with optional transaction reassignment
    public async Task<Result<bool>> DeleteCategoryAsync(Guid userId, Guid categoryId, Guid? targetCategoryId = null, CancellationToken ct = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId, ct);

        if (category == null)
            return Result<bool>.Failure("Không tìm thấy danh mục hoặc bạn không thể xóa danh mục mặc định của hệ thống.", 404);

        var hasTransactions = await _context.Transactions.AnyAsync(t => t.CategoryId == categoryId, ct);
        var hasChildren = await _context.Categories.AnyAsync(c => c.ParentId == categoryId, ct);

        if (hasTransactions || hasChildren)
        {
            if (!targetCategoryId.HasValue)
            {
                if (hasTransactions)
                    return Result<bool>.Failure("Danh mục này đang có giao dịch phát sinh. Vui lòng chọn danh mục thay thế để tiếp nhận các giao dịch này.");
                return Result<bool>.Failure("Không thể xóa danh mục đang chứa các danh mục con. Vui lòng xóa hoặc di chuyển các danh mục con trước.");
            }

            if (targetCategoryId.Value == categoryId)
                return Result<bool>.Failure("Vui lòng chọn một danh mục khác để tiếp nhận giao dịch.");

            var targetCat = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == targetCategoryId.Value && (c.IsSystem || c.UserId == userId), ct);

            if (targetCat == null || targetCat.TransactionType != category.TransactionType)
                return Result<bool>.Failure("Danh mục tiếp nhận thay thế không hợp lệ hoặc không cùng loại giao dịch.");

            // Reassign transactions
            var transactions = await _context.Transactions
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync(ct);
            foreach (var t in transactions)
            {
                t.UpdateDetails(t.WalletId, targetCategoryId.Value, t.Amount, t.TransactionDate, t.Note, t.IsEssential, t.IsWindfall, t.IsRecurring, t.ExcludeFromReport);
            }

            // Reassign children
            var children = await _context.Categories
                .Where(c => c.ParentId == categoryId)
                .ToListAsync(ct);
            foreach (var child in children)
            {
                child.UpdateDetails(child.Name, category.ParentId, child.IconKey, child.ColorHex, child.DisplayOrder, child.IncomeGroup, child.BudgetRuleMapping);
            }
        }

        category.MarkAsDeleted();
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    // Recursively maps category and its child branches
    private static CategoryDto MapToTreeDto(Category root, List<Category> allCategories)
    {
        var children = allCategories
            .Where(c => c.ParentId == root.Id)
            .Select(c => MapToTreeDto(c, allCategories))
            .ToList();

        return new CategoryDto(
            root.Id,
            root.UserId,
            root.Name,
            root.TransactionType,
            root.ParentId,
            null,
            root.IconKey,
            root.ColorHex,
            root.DisplayOrder,
            root.IsSystem,
            root.IncomeGroup,
            root.BudgetRuleMapping,
            children.Count > 0 ? children : null
        );
    }

    private async Task<int> GetDepthAsync(Guid categoryId, CancellationToken ct)
    {
        int depth = 1;
        var current = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        while (current?.ParentId != null)
        {
            depth++;
            current = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == current.ParentId.Value, ct);
        }
        return depth;
    }

    private async Task<bool> HasCycleAsync(Guid categoryId, Guid newParentId, CancellationToken ct)
    {
        if (categoryId == newParentId) return true;
        Guid? currentParentId = newParentId;
        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == categoryId) return true;
            var parent = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == currentParentId.Value, ct);
            currentParentId = parent?.ParentId;
        }
        return false;
    }

    private async Task<int> GetSubtreeMaxHeightAsync(Guid categoryId, CancellationToken ct)
    {
        var children = await _context.Categories.AsNoTracking()
            .Where(c => c.ParentId == categoryId)
            .Select(c => c.Id)
            .ToListAsync(ct);

        if (children.Count == 0) return 1;

        int maxChildHeight = 0;
        foreach (var childId in children)
        {
            var childHeight = await GetSubtreeMaxHeightAsync(childId, ct);
            if (childHeight > maxChildHeight) maxChildHeight = childHeight;
        }

        return 1 + maxChildHeight;
    }
}
