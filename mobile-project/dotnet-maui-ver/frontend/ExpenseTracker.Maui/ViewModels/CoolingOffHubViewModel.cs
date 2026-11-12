using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Behavioral;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing 24-hour Positive Friction Cooling-Off Mechanism (UC-09.05)
public partial class CoolingOffHubViewModel : BaseViewModel
{
    private readonly ICoolingOffRepository _repository;

    [ObservableProperty]
    private CoolingOffTransactionModel? _selectedItem;

    [ObservableProperty]
    private CoolingOffEvaluationResultModel? _evaluationResult;

    [ObservableProperty]
    private decimal _testExpenseAmount = 4500000m;

    [ObservableProperty]
    private string _testDescription = "Đồng hồ thông minh cao cấp";

    [ObservableProperty]
    private bool _isEvaluateModalVisible;

    [ObservableProperty]
    private bool _isBypassModalVisible;

    [ObservableProperty]
    private string _bypassReason = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    public ObservableCollection<CoolingOffTransactionModel> PendingTransactions { get; } = new();

    public CoolingOffHubViewModel(ICoolingOffRepository repository)
    {
        _repository = repository;
        Title = "Khoảng Trễ 24h Mua Sắm";
    }

    [RelayCommand]
    public async Task LoadPendingTransactionsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var items = await _repository.GetPendingCoolingOffAsync();
            PendingTransactions.Clear();
            foreach (var item in items)
            {
                PendingTransactions.Add(item);
            }

            if (PendingTransactions.Count == 0)
            {
                StatusMessage = "Không có giao dịch nào đang bị phong tỏa tâm lý. Mọi khoản chi lớn đều được cân nhắc kỹ lưỡng!";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải danh sách giao dịch hạ nhiệt: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task EvaluateTestExpenseAsync()
    {
        if (TestExpenseAmount <= 0)
        {
            ErrorMessage = "Số tiền thử nghiệm phải lớn hơn 0 đ.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var request = new CoolingOffEvaluationRequestModel
            {
                WalletId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Amount = TestExpenseAmount,
                Description = TestDescription
            };

            EvaluationResult = await _repository.EvaluateExpenseAsync(request);
            IsEvaluateModalVisible = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi đánh giá khoảng trễ: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void CloseEvaluateModal()
    {
        IsEvaluateModalVisible = false;
    }

    [RelayCommand]
    public async Task ConfirmPurchaseAsync(CoolingOffTransactionModel item)
    {
        if (item == null || IsBusy) return;

        if (!item.IsExpired)
        {
            ErrorMessage = $"Giao dịch chưa hết thời gian hạ nhiệt ({item.RemainingTimeDisplay}). Hãy cho tâm trí bạn thêm thời gian để quyết định lý trí!";
            return;
        }

        try
        {
            IsBusy = true;
            bool ok = await _repository.ConfirmPurchaseAsync(item.Id);
            if (ok)
            {
                PendingTransactions.Remove(item);
                StatusMessage = $"Đã xác nhận mua khoản chi {item.Amount:N0} đ sau thời gian cân nhắc lý trí.";
            }
            else
            {
                ErrorMessage = "Không thể xác nhận mua giao dịch này.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xác nhận mua: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CancelPurchaseAsync(CoolingOffTransactionModel item)
    {
        if (item == null || IsBusy) return;

        try
        {
            IsBusy = true;
            bool ok = await _repository.CancelPurchaseAsync(item.Id);
            if (ok)
            {
                PendingTransactions.Remove(item);
                StatusMessage = $"Chúc mừng! Bạn đã vượt qua quyết định bốc đồng và bảo toàn thành công {item.Amount:N0} đ (tương đương {item.EquivalentWorkHours:N1} giờ lao động) cho ví tiền của mình.";
            }
            else
            {
                ErrorMessage = "Không thể hủy giao dịch.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi hủy giao dịch: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void OpenBypassModal(CoolingOffTransactionModel item)
    {
        SelectedItem = item;
        BypassReason = string.Empty;
        IsBypassModalVisible = true;
    }

    [RelayCommand]
    public void CloseBypassModal()
    {
        IsBypassModalVisible = false;
        SelectedItem = null;
        BypassReason = string.Empty;
    }

    [RelayCommand]
    public async Task SubmitBypassAsync()
    {
        if (SelectedItem == null || IsBusy) return;

        if (string.IsNullOrWhiteSpace(BypassReason))
        {
            ErrorMessage = "Bắt buộc phải nhập lý do khẩn cấp để mở khóa ma sát tâm lý.";
            return;
        }

        try
        {
            IsBusy = true;
            var request = new BypassCoolingOffRequestModel { Reason = BypassReason.Trim() };
            bool ok = await _repository.BypassCoolingOffAsync(SelectedItem.Id, request);
            if (ok)
            {
                PendingTransactions.Remove(SelectedItem);
                StatusMessage = $"Đã mở khóa khẩn cấp khoản chi {SelectedItem.Amount:N0} đ.";
                CloseBypassModal();
            }
            else
            {
                ErrorMessage = "Không thể mở khóa khẩn cấp.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi mở khóa: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
