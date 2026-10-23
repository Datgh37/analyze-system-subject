using ExpenseTracker.Application.DTOs.Categories;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Interfaces;

// Service contract for category hierarchy management
public interface ICategoryService
{
    Task<Result<List<CategoryDto>>> GetCategoriesAsync(Guid userId, TransactionType? transactionType = null, CancellationToken ct = default);
    Task<Result<CategoryDto>> GetCategoryByIdAsync(Guid userId, Guid categoryId, CancellationToken ct = default);
    Task<Result<CategoryDto>> CreateCategoryAsync(Guid userId, CreateCategoryRequest request, CancellationToken ct = default);
    Task<Result<CategoryDto>> UpdateCategoryAsync(Guid userId, Guid categoryId, UpdateCategoryRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteCategoryAsync(Guid userId, Guid categoryId, Guid? targetCategoryId = null, CancellationToken ct = default);
}
