using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Production API repository connecting to backend ASP.NET Core WebApi
public class ApiCategoryRepository : ICategoryRepository
{
    private readonly ICategoryApi _api;

    public ApiCategoryRepository(ICategoryApi api)
    {
        _api = api;
    }

    public Task<List<CategoryModel>> GetCategoriesAsync(int? transactionType = null)
        => _api.GetCategoriesAsync(transactionType);

    public Task<CategoryModel> GetCategoryByIdAsync(Guid id)
        => _api.GetCategoryByIdAsync(id);

    public Task<CategoryModel> CreateCategoryAsync(CreateCategoryModel request)
        => _api.CreateCategoryAsync(request);

    public Task<CategoryModel> UpdateCategoryAsync(Guid id, UpdateCategoryModel request)
        => _api.UpdateCategoryAsync(id, request);

    public Task DeleteCategoryAsync(Guid id, Guid? targetCategoryId = null)
        => _api.DeleteCategoryAsync(id, targetCategoryId);
}
