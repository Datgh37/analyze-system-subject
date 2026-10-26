using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Forecasting;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing Financial Forecasting, Liquidity Depletion Radar and What-If Simulation Sandbox (B3 Clean MVVM)
public partial class ForecastingViewModel : BaseViewModel
{
    private readonly IForecastingRepository _repository;

    [ObservableProperty]
    private ExpenseForecastModel? _forecast;

    [ObservableProperty]
    private LiquidityDepletionModel? _liquidity;

    [ObservableProperty]
    private WhatIfSimulationResultModel? _whatIfResult;

    [ObservableProperty]
    private WhatIfScenarioType _selectedScenarioType = WhatIfScenarioType.LumpSumExpense;

    [ObservableProperty]
    private decimal _lumpSumAmount = 5000000m;

    [ObservableProperty]
    private decimal _monthlyInstallment = 2500000m;

    [ObservableProperty]
    private int _installmentMonths = 12;

    [ObservableProperty]
    private decimal _incomeReductionPercentage = 20m;

    [ObservableProperty]
    private string? _statusMessage;

    public ObservableCollection<CategoryDeficitRiskModel> CategoryRisks { get; } = new();
    public ObservableCollection<DailyForecastPointModel> TrajectoryPoints { get; } = new();

    public ForecastingViewModel(IForecastingRepository repository)
    {
        _repository = repository;
        Title = "Dự Báo Dòng Tiền";
    }

    [RelayCommand]
    public async Task LoadForecastDataAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            // 1. Load Expenditure Forecast (UC-08.01)
            Forecast = await _repository.GetExpenditureForecastAsync();
            TrajectoryPoints.Clear();
            if (Forecast?.Trajectory != null)
            {
                foreach (var point in Forecast.Trajectory)
                {
                    TrajectoryPoints.Add(point);
                }
            }

            // 2. Load Liquidity Depletion Radar (UC-08.02)
            Liquidity = await _repository.GetLiquidityDepletionAsync();

            // 3. Load Category Deficit Risks (UC-08.03)
            var risks = await _repository.GetCategoryRisksAsync();
            CategoryRisks.Clear();
            if (risks != null)
            {
                foreach (var risk in risks)
                {
                    CategoryRisks.Add(risk);
                }
            }

            // 4. Run baseline What-If Simulation (UC-08.04)
            await RunWhatIfSimulationAsync();

            StatusMessage = "Đã cập nhật dự báo tài chính và radar rủi ro thời gian thực.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải dữ liệu dự báo: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task RunWhatIfSimulationAsync()
    {
        try
        {
            var req = new WhatIfSimulationRequestModel
            {
                ScenarioType = SelectedScenarioType,
                LumpSumAmount = LumpSumAmount,
                MonthlyInstallmentAmount = MonthlyInstallment,
                InstallmentMonths = InstallmentMonths,
                IncomeReductionPercentage = IncomeReductionPercentage
            };

            WhatIfResult = await _repository.SimulateWhatIfAsync(req);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi mô phỏng giả định: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task SelectScenarioAsync(string scenarioName)
    {
        switch (scenarioName)
        {
            case "LumpSum":
                SelectedScenarioType = WhatIfScenarioType.LumpSumExpense;
                break;
            case "Installment":
                SelectedScenarioType = WhatIfScenarioType.InstallmentLoan;
                break;
            case "IncomeReduction":
                SelectedScenarioType = WhatIfScenarioType.IncomeReduction;
                break;
        }

        await RunWhatIfSimulationAsync();
    }
}
