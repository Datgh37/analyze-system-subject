using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Reports;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing multi-dimensional financial reports and analytics (Clean MVVM Architecture - B3)
public partial class ReportsViewModel : BaseViewModel
{
    private readonly IAnalyticsReportRepository _repository;

    [ObservableProperty]
    private string _activeTab = "Overview"; // Overview, NetWorth, Cashflow, Structure, Outliers

    [ObservableProperty]
    private DateTime _fromDate = DateTime.UtcNow.AddMonths(-1).Date;

    [ObservableProperty]
    private DateTime _toDate = DateTime.UtcNow.Date;

    [ObservableProperty]
    private IncomeExpenseComparisonModel? _incomeExpenseReport;

    [ObservableProperty]
    private NetWorthReportModel? _netWorthReport;

    [ObservableProperty]
    private NetCashflowReportModel? _cashflowReport;

    [ObservableProperty]
    private ExpenseStructureReportModel? _expenseStructureReport;

    [ObservableProperty]
    private OutlierDetectionReportModel? _outlierReport;

    [ObservableProperty]
    private string? _statusMessage;

    public ObservableCollection<PeriodDataPointModel> BreakdownItems { get; } = new();
    public ObservableCollection<CategorySpendingShareModel> SubCategories { get; } = new();
    public ObservableCollection<OutlierTransactionModel> Outliers { get; } = new();
    public ObservableCollection<WaterfallStepModel> WaterfallSteps { get; } = new();

    public ReportsViewModel(IAnalyticsReportRepository repository)
    {
        _repository = repository;
        Title = "Báo Cáo Tài Chính";
    }

    [RelayCommand]
    public async Task LoadReportsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            // 1. Overview (Income vs Expense)
            IncomeExpenseReport = await _repository.GetIncomeExpenseAsync(FromDate, ToDate, "month");
            BreakdownItems.Clear();
            if (IncomeExpenseReport?.Breakdown != null)
            {
                foreach (var item in IncomeExpenseReport.Breakdown)
                {
                    BreakdownItems.Add(item);
                }
            }

            // 2. Net Worth
            NetWorthReport = await _repository.GetNetWorthAsync(FromDate, ToDate);

            // 3. Cashflow & Waterfall
            CashflowReport = await _repository.GetCashflowAsync(FromDate, ToDate);
            WaterfallSteps.Clear();
            if (CashflowReport?.WaterfallSteps != null)
            {
                foreach (var step in CashflowReport.WaterfallSteps)
                {
                    WaterfallSteps.Add(step);
                }
            }

            // 4. Structure
            ExpenseStructureReport = await _repository.GetExpenseStructureAsync(FromDate, ToDate);
            SubCategories.Clear();
            if (ExpenseStructureReport?.BySubCategory != null)
            {
                foreach (var sub in ExpenseStructureReport.BySubCategory)
                {
                    SubCategories.Add(sub);
                }
            }

            // 5. Outliers
            OutlierReport = await _repository.GetOutliersAsync(days: 30, threshold: 2.0);
            Outliers.Clear();
            if (OutlierReport?.Outliers != null)
            {
                foreach (var o in OutlierReport.Outliers)
                {
                    Outliers.Add(o);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải báo cáo phân tích: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SelectTabAsync(string tabName)
    {
        if (string.IsNullOrWhiteSpace(tabName) || ActiveTab == tabName) return;

        ActiveTab = tabName;
        // Optionally refresh or keep existing data
        if (IncomeExpenseReport == null)
        {
            await LoadReportsAsync();
        }
    }

    [RelayCommand]
    public async Task TakeSnapshotAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var snapshot = await _repository.RecordNetWorthSnapshotAsync("Chụp ảnh số dư thủ công");
            StatusMessage = $"Đã ghi nhận ảnh chụp số dư ngày {snapshot.SnapshotDate:dd/MM/yyyy}: {snapshot.NetWorth:N0} đ";

            // Refresh Net Worth
            NetWorthReport = await _repository.GetNetWorthAsync(FromDate, ToDate);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi lưu ảnh chụp tài sản: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
