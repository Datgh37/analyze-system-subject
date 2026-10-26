using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Budgets;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing real-time budget progress tracking, Adaptive DSA widget, and spending velocity (Clean MVVM Architecture)
public partial class BudgetsViewModel : BaseViewModel
{
    private readonly IBudgetRepository _budgetRepository;

    public ObservableCollection<BudgetAllocationModel> Allocations { get; } = new();

    [ObservableProperty]
    private BudgetModel? _activeBudget;

    [ObservableProperty]
    private bool _hasActiveBudget;

    [ObservableProperty]
    private DsaStatusModel? _dsaStatus;

    [ObservableProperty]
    private VelocityStatusModel? _velocityStatus;

    [ObservableProperty]
    private int _daysPassed;

    [ObservableProperty]
    private int _totalDays;

    [ObservableProperty]
    private string? _statusMessage;

    public BudgetsViewModel(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
        Title = "Ngân Sách & DSA";
    }

    [RelayCommand]
    public async Task LoadActiveBudgetProgressAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var overview = await _budgetRepository.GetBudgetProgressOverviewAsync();

            ActiveBudget = await _budgetRepository.GetActiveBudgetAsync();
            HasActiveBudget = ActiveBudget != null;

            DsaStatus = overview.Dsa;
            VelocityStatus = overview.Velocity;
            DaysPassed = overview.DaysPassed;
            TotalDays = overview.TotalDays;

            Allocations.Clear();
            foreach (var alloc in overview.Allocations)
            {
                Allocations.Add(alloc);
            }
        }
        catch (Refit.ApiException apiEx) when (apiEx.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            HasActiveBudget = false;
            ActiveBudget = null;
            Allocations.Clear();
            StatusMessage = "Chưa có kế hoạch ngân sách nào đang hoạt động. Vui lòng bấm 'Lập Ngân Sách Mới'.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải tiến độ ngân sách: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CloseActiveBudgetAsync()
    {
        if (ActiveBudget == null) return;

        bool confirm = true;
        if (Shell.Current != null)
        {
            confirm = await Shell.Current.DisplayAlertAsync(
                "Xác nhận đóng ngân sách",
                $"Bạn có chắc chắn muốn đóng kế hoạch '{ActiveBudget.Name}' không?",
                "Đóng kế hoạch", "Hủy");
        }

        if (!confirm) return;

        IsBusy = true;
        try
        {
            await _budgetRepository.CloseBudgetAsync(ActiveBudget.Id);
            StatusMessage = "✅ Đã đóng kế hoạch ngân sách thành công.";
            await LoadActiveBudgetProgressAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi đóng ngân sách: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task NavigateToCreateBudgetAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("CreateBudgetPage");
        }
    }
}

