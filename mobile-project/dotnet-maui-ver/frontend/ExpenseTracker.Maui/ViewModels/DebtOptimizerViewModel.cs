using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Debts;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel quản lý tương tác người dùng cho động cơ tối ưu hóa trả nợ và hướng dẫn tài chính bình dân hóa (UC-06.06)
public partial class DebtOptimizerViewModel : BaseViewModel
{
    private readonly IDebtRepository _debtRepository;

    [ObservableProperty]
    private DebtEducationalGuideModel? _educationalGuide;

    [ObservableProperty]
    private SafeExtraBudgetRecommendationModel? _safeBudgetRecommendation;

    [ObservableProperty]
    private DebtPayoffComparisonModel? _comparisonResult;

    [ObservableProperty]
    private decimal _extraMonthlyBudget = 1_000_000m;

    [ObservableProperty]
    private string _selectedPersonality = "SAVINGS"; // "STRESS" hoặc "SAVINGS"

    [ObservableProperty]
    private string _selectedStrategy = "AVALANCHE"; // "SNOWBALL" hoặc "AVALANCHE"

    [ObservableProperty]
    private bool _isGuideExpanded;

    [ObservableProperty]
    private string _laymanAdvice = string.Empty;

    public DebtOptimizerViewModel(IDebtRepository debtRepository)
    {
        _debtRepository = debtRepository;
        Title = "Tối Ưu Hóa & Lộ Trình Sạch Nợ";
    }

    [RelayCommand]
    public async Task LoadInitialDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            // 1. Nạp cẩm nang hướng dẫn và gợi ý ngân sách an toàn
            EducationalGuide = await _debtRepository.GetEducationalGuideAsync();
            SafeBudgetRecommendation = await _debtRepository.GetSafeBudgetRecommendationAsync();

            if (SafeBudgetRecommendation != null && SafeBudgetRecommendation.RecommendedAmount > 0)
            {
                ExtraMonthlyBudget = SafeBudgetRecommendation.RecommendedAmount;
            }

            // 2. Chạy mô phỏng so sánh ban đầu
            await RunSimulationInternalAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải dữ liệu tối ưu hóa: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SimulateAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await RunSimulationInternalAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi tính toán mô phỏng: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ApplySuggestedBudgetAsync(decimal amount)
    {
        ExtraMonthlyBudget = amount;
        await SimulateAsync();
    }

    [RelayCommand]
    public void SelectPersonality(string personality)
    {
        SelectedPersonality = personality?.ToUpperInvariant() ?? "SAVINGS";
        if (SelectedPersonality == "STRESS")
        {
            SelectedStrategy = "SNOWBALL";
        }
        else
        {
            SelectedStrategy = "AVALANCHE";
        }
    }

    [RelayCommand]
    public void ToggleGuide()
    {
        IsGuideExpanded = !IsGuideExpanded;
    }

    private async Task RunSimulationInternalAsync()
    {
        ComparisonResult = await _debtRepository.CompareStrategiesAsync(ExtraMonthlyBudget);
        if (ComparisonResult != null)
        {
            LaymanAdvice = ComparisonResult.LaymanRecommendationText;
            SelectedStrategy = ComparisonResult.RecommendedStrategy;
        }
    }
}

