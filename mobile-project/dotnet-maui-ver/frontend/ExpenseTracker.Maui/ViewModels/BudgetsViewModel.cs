using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Models.Budgets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Realtime.Messages;

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
        Title = "Kế Hoạch Ngân Sách";

        // Listen for budget alerts from SignalR
        WeakReferenceMessenger.Default.Register<BudgetAlertReceivedMessage>(this, (r, m) =>
        {
            StatusMessage = $"[Cảnh báo] {m.Value.CategoryName}: đã dùng {m.Value.PercentUsed:F0}% ({m.Value.SpentAmount:N0} đ / {m.Value.LimitAmount:N0} đ)";
            var target = Allocations.FirstOrDefault(a => a.CategoryName.Equals(m.Value.CategoryName, StringComparison.OrdinalIgnoreCase));
            if (target != null)
            {
                target.CurrentSpent = m.Value.SpentAmount;
                target.RemainingAmount = Math.Max(0, target.AllocatedAmount - target.CurrentSpent);
                target.SpentPercent = m.Value.PercentUsed;
                target.IsAlertTriggered = true;
                if (target.SpentPercent >= 100) target.IsExceeded = true;
            }
        });

        // Listen for realtime transactions to update category budget allocation progress
        WeakReferenceMessenger.Default.Register<TransactionCreatedMessage>(this, (r, m) =>
        {
            if (m.Value.TransactionType == 2) // Expense
            {
                var alloc = Allocations.FirstOrDefault(a => a.CategoryId == m.Value.CategoryId);
                if (alloc != null)
                {
                    alloc.CurrentSpent += m.Value.Amount;
                    alloc.RemainingAmount = Math.Max(0, alloc.AllocatedAmount - alloc.CurrentSpent);
                    if (alloc.AllocatedAmount > 0)
                    {
                        alloc.SpentPercent = Math.Round((alloc.CurrentSpent / alloc.AllocatedAmount) * 100m, 1);
                    }
                    if (alloc.SpentPercent >= alloc.AlertThresholdPercent) alloc.IsAlertTriggered = true;
                    if (alloc.SpentPercent >= 100) alloc.IsExceeded = true;
                }
            }
        });

        // Listen for server mode changes (Live API <-> Offline Mock)
        WeakReferenceMessenger.Default.Register<ServerModeChangedMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadActiveBudgetProgressAsync();
            });
        });
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
        catch (Refit.ApiException apiEx) when (apiEx.StatusCode == System.Net.HttpStatusCode.NotFound || apiEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
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
            StatusMessage = "Đã đóng kế hoạch ngân sách thành công.";
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


