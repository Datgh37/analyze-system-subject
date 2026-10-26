using ExpenseTracker.Maui.Models.Categories;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for category hierarchy and taxonomy operations
public interface ICategoryRepository
{
    Task<List<CategoryModel>> GetCategoriesAsync(int? transactionType = null);
    Task<CategoryModel> GetCategoryByIdAsync(Guid id);
    Task<CategoryModel> CreateCategoryAsync(CreateCategoryModel request);
    Task<CategoryModel> UpdateCategoryAsync(Guid id, UpdateCategoryModel request);
    Task DeleteCategoryAsync(Guid id, Guid? targetCategoryId = null);
}
