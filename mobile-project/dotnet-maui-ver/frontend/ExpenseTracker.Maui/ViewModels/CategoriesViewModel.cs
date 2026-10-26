using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing category taxonomy browsing, dual-type filtering and hierarchy creation
public partial class CategoriesViewModel : BaseViewModel
{
    private readonly ICategoryRepository _categoryRepository;

    public ObservableCollection<CategoryModel> Categories { get; } = new();
    public ObservableCollection<CategoryModel> AvailableParentCategories { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIncomeSelected))]
    [NotifyPropertyChangedFor(nameof(IsExpenseSelected))]
    private int _selectedTypeFilter = 1; // 1 = Income, 2 = Expense

    public bool IsIncomeSelected => SelectedTypeFilter == 1;
    public bool IsExpenseSelected => SelectedTypeFilter == 2;

    [ObservableProperty]
    private string _newCategoryName = string.Empty;

    [ObservableProperty]
    private CategoryModel? _selectedParentCategory;

    public CategoriesViewModel(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
        Title = "Danh Mục Thu & Chi";
    }

    [RelayCommand]
    public async Task SwitchTypeFilterAsync(string typeStr)
    {
        if (int.TryParse(typeStr, out var type))
        {
            SelectedTypeFilter = type;
            SelectedParentCategory = null;
            await LoadCategoriesAsync();
        }
    }

    [RelayCommand]
    public async Task LoadCategoriesAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var list = await _categoryRepository.GetCategoriesAsync(SelectedTypeFilter);
            Categories.Clear();
            AvailableParentCategories.Clear();

            foreach (var item in list)
            {
                Categories.Add(item);
                // Parent categories available to be selected as parent
                AvailableParentCategories.Add(item);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải danh mục: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CreateCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
        {
            ErrorMessage = "Vui lòng nhập tên danh mục.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var req = new CreateCategoryModel(
                Name: NewCategoryName.Trim(),
                TransactionType: SelectedTypeFilter,
                ParentId: SelectedParentCategory?.Id
            );

            await _categoryRepository.CreateCategoryAsync(req);
            NewCategoryName = string.Empty;
            SelectedParentCategory = null;
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tạo danh mục: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteCategoryAsync(CategoryModel category)
    {
        if (category == null || category.IsSystem)
        {
            ErrorMessage = "Không thể xóa danh mục mặc định của hệ thống.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _categoryRepository.DeleteCategoryAsync(category.Id);
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xóa danh mục: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
