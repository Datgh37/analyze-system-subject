using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.SavingsGoals;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing savings goal creation, deadline definition, and monthly contribution projection (UC-06.01)
public partial class CreateSavingsGoalViewModel : BaseViewModel
{
    private readonly ISavingsGoalApi _savingsGoalApi;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedMonthlyDeposit))]
    [NotifyPropertyChangedFor(nameof(FormattedCalculatedMonthly))]
    private decimal _targetAmount = 10_000_000m;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedMonthlyDeposit))]
    [NotifyPropertyChangedFor(nameof(FormattedCalculatedMonthly))]
    private decimal _initialAmount = 0m;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedMonthlyDeposit))]
    [NotifyPropertyChangedFor(nameof(FormattedCalculatedMonthly))]
    private DateTime _startDate = DateTime.Today;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedMonthlyDeposit))]
    [NotifyPropertyChangedFor(nameof(FormattedCalculatedMonthly))]
    private DateTime _targetDate = DateTime.Today.AddMonths(6);

    [ObservableProperty]
    private SavingsGoalType _selectedType = SavingsGoalType.EmergencyFund;

    [ObservableProperty]
    private SavingsGoalPriority _selectedPriority = SavingsGoalPriority.High;

    public List<SavingsGoalType> AvailableTypes { get; } = new()
    {
        SavingsGoalType.EmergencyFund,
        SavingsGoalType.MajorPurchase,
        SavingsGoalType.Investment,
        SavingsGoalType.Education,
        SavingsGoalType.Travel,
        SavingsGoalType.Other
    };

    public List<SavingsGoalPriority> AvailablePriorities { get; } = new()
    {
        SavingsGoalPriority.Low,
        SavingsGoalPriority.Medium,
        SavingsGoalPriority.High
    };

    public decimal CalculatedMonthlyDeposit
    {
        get
        {
            decimal remaining = Math.Max(0m, TargetAmount - InitialAmount);
            int months = Math.Max(1, ((TargetDate.Year - StartDate.Year) * 12) + TargetDate.Month - StartDate.Month);
            return Math.Round(remaining / months, 0);
        }
    }

    public string FormattedCalculatedMonthly => $"{CalculatedMonthlyDeposit:N0} đ/tháng";

    public CreateSavingsGoalViewModel(ISavingsGoalApi savingsGoalApi)
    {
        _savingsGoalApi = savingsGoalApi;
        Title = "Tạo Mục Tiêu Tích Lũy Mới";
    }

    [RelayCommand]
    public async Task SaveGoalAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Vui lòng nhập tên mục tiêu tích lũy.";
            return;
        }

        if (TargetAmount <= 0)
        {
            ErrorMessage = "Số tiền mục tiêu phải lớn hơn 0 đ.";
            return;
        }

        if (TargetDate <= StartDate)
        {
            ErrorMessage = "Hạn chót hoàn thành mục tiêu phải sau ngày bắt đầu.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var request = new CreateSavingsGoalModel
            {
                Name = Name.Trim(),
                Description = Description?.Trim(),
                TargetAmount = TargetAmount,
                InitialAmount = InitialAmount,
                StartDate = StartDate,
                TargetDate = TargetDate,
                Type = SelectedType,
                Priority = SelectedPriority,
                ColorHex = SelectedType == SavingsGoalType.EmergencyFund ? "#DC2626" : "#2563EB",
                IconKey = SelectedType == SavingsGoalType.EmergencyFund ? "shield-check" : "piggy-bank"
            };

            await _savingsGoalApi.CreateSavingsGoalAsync(request);

            await Shell.Current.DisplayAlertAsync(
                "Thành công",
                $"Mục tiêu tích lũy '{Name}' đã được tạo thành công! Bạn cần nạp đều đặn khoảng {CalculatedMonthlyDeposit:N0} đ/tháng để cán đích đúng hạn.",
                "Đồng ý");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi lưu mục tiêu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
