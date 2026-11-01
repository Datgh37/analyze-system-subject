using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.SavingsGoals;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel quản lý giao diện Quỹ Dự Phòng Khẩn Cấp, ma sát tâm lý và bảo vệ số dư khả dụng (UC-06.04)
public partial class EmergencyFundViewModel : BaseViewModel
{
    private readonly IEmergencyFundRepository _repository;

    [ObservableProperty]
    private EmergencyFundRecommendationModel? _recommendation;

    [ObservableProperty]
    private EmergencyFundStatusModel? _status;

    [ObservableProperty]
    private int _selectedMonths = 6;

    [ObservableProperty]
    private decimal _depositAmount;

    [ObservableProperty]
    private decimal _withdrawAmount;

    [ObservableProperty]
    private string _emergencyReason = "Medical";

    [ObservableProperty]
    private string _detailedExplanation = string.Empty;

    [ObservableProperty]
    private string _feedbackMessage = string.Empty;

    public EmergencyFundViewModel(IEmergencyFundRepository repository)
    {
        _repository = repository;
        Title = "Quỹ Dự Phòng Khẩn Cấp";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi khi tải dữ liệu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadDataInternalAsync()
    {
        var recTask = _repository.GetRecommendationAsync();
        var statusTask = _repository.GetStatusAsync();

        await Task.WhenAll(recTask, statusTask);

        Recommendation = await recTask;
        Status = await statusTask;

        if (Status != null && Status.HasFundConfigured && Status.TargetMonths > 0)
        {
            SelectedMonths = Status.TargetMonths;
        }
    }

    [RelayCommand]
    public async Task SetupFundAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;

            var req = new SetupEmergencyFundRequest
            {
                TargetMonths = SelectedMonths
            };

            Status = await _repository.SetupFundAsync(req);
            FeedbackMessage = "Thiết lập Quỹ Khẩn Cấp thành công!";
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi khi thiết lập: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DepositAsync(Guid sourceWalletId)
    {
        if (IsBusy || DepositAmount <= 0) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;

            var req = new DepositEmergencyFundRequest
            {
                Amount = DepositAmount,
                SourceWalletId = sourceWalletId,
                Note = "Tích lũy Quỹ Khẩn Cấp từ Mobile App"
            };

            Status = await _repository.DepositAsync(req);
            DepositAmount = 0m;
            FeedbackMessage = "Nạp tiền vào Quỹ Khẩn Cấp thành công!";
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi nạp tiền: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task WithdrawAsync(Guid targetWalletId)
    {
        if (IsBusy || WithdrawAmount <= 0) return;

        if (string.IsNullOrWhiteSpace(DetailedExplanation) || DetailedExplanation.Trim().Length < 10)
        {
            FeedbackMessage = "Vui lòng nhập giải trình lý do khẩn cấp tối thiểu 10 ký tự.";
            return;
        }

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;

            var req = new WithdrawEmergencyFundRequest
            {
                Amount = WithdrawAmount,
                TargetWalletId = targetWalletId,
                EmergencyReason = EmergencyReason,
                DetailedExplanation = DetailedExplanation.Trim()
            };

            Status = await _repository.WithdrawAsync(req);
            WithdrawAmount = 0m;
            DetailedExplanation = string.Empty;
            FeedbackMessage = "Giải ngân thành công! Hãy lưu ý bù đắp lại số tiền này sớm nhất có thể.";
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi giải ngân: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
