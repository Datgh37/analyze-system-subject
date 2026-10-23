using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.AutoRules;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing automatic keyword rules for smart category classification (UC-04.07)
public partial class AutoRulesViewModel : BaseViewModel
{
    private readonly IAutoRuleApi _autoRuleApi;
    private readonly ICategoryApi _categoryApi;

    public ObservableCollection<AutoRuleModel> Rules { get; } = new();
    public ObservableCollection<CategoryModel> AvailableCategories { get; } = new();

    [ObservableProperty]
    private string _newKeywordPattern = string.Empty;

    [ObservableProperty]
    private CategoryModel? _selectedCategory;

    [ObservableProperty]
    private int _priorityOrder = 1;

    [ObservableProperty]
    private bool _isRegex = false;

    [ObservableProperty]
    private bool _isCaseSensitive = false;

    public AutoRulesViewModel(IAutoRuleApi autoRuleApi, ICategoryApi categoryApi)
    {
        _autoRuleApi = autoRuleApi;
        _categoryApi = categoryApi;
        Title = "Quy Tắc Phân Loại Tự Động (Wireframe)";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            // Load user categories (Expense type = 2)
            var categories = await _categoryApi.GetCategoriesAsync(2);
            AvailableCategories.Clear();
            foreach (var cat in categories)
            {
                AvailableCategories.Add(cat);
            }

            // Load existing rules
            var rules = await _autoRuleApi.GetRulesAsync();
            Rules.Clear();
            foreach (var r in rules)
            {
                Rules.Add(r);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải dữ liệu quy tắc: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CreateRuleAsync()
    {
        if (string.IsNullOrWhiteSpace(NewKeywordPattern) || NewKeywordPattern.Trim().Length < 2)
        {
            ErrorMessage = "Vui lòng nhập từ khóa nhận diện hợp lệ (tối thiểu 2 ký tự).";
            return;
        }

        if (SelectedCategory == null)
        {
            ErrorMessage = "Vui lòng chọn danh mục đích.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var req = new CreateAutoRuleModel(
                KeywordPattern: NewKeywordPattern.Trim(),
                TargetCategoryId: SelectedCategory.Id,
                PriorityOrder: PriorityOrder,
                IsRegex: IsRegex,
                IsCaseSensitive: IsCaseSensitive
            );

            await _autoRuleApi.CreateRuleAsync(req);

            NewKeywordPattern = string.Empty;
            PriorityOrder = 1;
            IsRegex = false;
            IsCaseSensitive = false;

            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tạo quy tắc: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteRuleAsync(AutoRuleModel rule)
    {
        if (rule == null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _autoRuleApi.DeleteRuleAsync(rule.Id);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xóa quy tắc: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
