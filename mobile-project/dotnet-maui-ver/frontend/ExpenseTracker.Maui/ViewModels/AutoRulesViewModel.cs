using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.AutoRules;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing automatic keyword rules for smart category classification (UC-04.07)
public partial class AutoRulesViewModel : BaseViewModel
{
    private readonly IAutoRuleRepository _autoRuleRepository;
    private readonly ICategoryRepository _categoryRepository;

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

    public AutoRulesViewModel(IAutoRuleRepository autoRuleRepository, ICategoryRepository categoryRepository)
    {
        _autoRuleRepository = autoRuleRepository;
        _categoryRepository = categoryRepository;
        Title = "Quy Tắc Tự Động Hóa";
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
            var categories = await _categoryRepository.GetCategoriesAsync(2);
            AvailableCategories.Clear();
            foreach (var cat in categories)
            {
                AvailableCategories.Add(cat);
            }
            if (AvailableCategories.Count > 0 && SelectedCategory == null)
            {
                SelectedCategory = AvailableCategories[0];
            }

            // Load existing rules
            var rules = await _autoRuleRepository.GetRulesAsync();
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

            await _autoRuleRepository.CreateRuleAsync(req);

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
            await _autoRuleRepository.DeleteRuleAsync(rule.Id);
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
