using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Autonomous;
using ExpenseTracker.Maui.Repositories.Autonomous;

namespace ExpenseTracker.Maui.ViewModels.Autonomous;

// ViewModel managing Server-Driven Action Protocol & Jev Probabilistic Decision Gating (Chặng 10.5 / PLAN-47)
public partial class ActionInboxViewModel : BaseViewModel
{
    private readonly IAutonomousActionRepository _repository;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPendingActions))]
    private int _actionCount;

    [ObservableProperty]
    private string? _actionStatusMessage;

    public ObservableCollection<AutonomousActionModel> PendingActions { get; } = new();

    public bool HasPendingActions => PendingActions.Count > 0;

    public ActionInboxViewModel(IAutonomousActionRepository repository)
    {
        _repository = repository;
        Title = "Việc cần xử lý";
    }

    [RelayCommand]
    public async Task LoadInboxAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var response = await _repository.GetInboxAsync();
            PendingActions.Clear();
            if (response?.Actions != null)
            {
                foreach (var action in response.Actions)
                {
                    PendingActions.Add(action);
                }
            }
            ActionCount = PendingActions.Count;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải danh sách hành động: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task QuickCommitAsync(AutonomousActionModel? action)
    {
        if (action == null || IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var req = new QuickCommitActionRequestModel(
                ActionId: action.ActionId,
                CategoryName: action.Display.BadgeText,
                Amount: action.Display.Amount,
                WalletName: action.PrimaryAction?.Payload?.GetValueOrDefault("walletName"),
                Note: action.Display.Title
            );

            await _repository.QuickCommitAsync(req);
            PendingActions.Remove(action);
            ActionCount = PendingActions.Count;
            ActionStatusMessage = $"Đã lưu {action.Display.Amount:N0} đ vào sổ cái";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể lưu nhanh: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SelectChoiceAsync(ActionChoiceModel? choice)
    {
        if (choice == null || IsBusy) return;

        // Find the action containing this choice
        var action = PendingActions.FirstOrDefault(a => a.Choices != null && a.Choices.Any(c => c.ChoiceId == choice.ChoiceId));
        if (action == null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            string categoryName = choice.Payload?.GetValueOrDefault("categoryName") ?? choice.Label;
            var req = new QuickCommitActionRequestModel(
                ActionId: action.ActionId,
                CategoryName: categoryName,
                Amount: action.Display.Amount,
                Note: $"{action.Display.Title} ({categoryName})"
            );

            await _repository.QuickCommitAsync(req);
            PendingActions.Remove(action);
            ActionCount = PendingActions.Count;
            ActionStatusMessage = $"Đã phân loại vào '{categoryName}' và lưu thành công";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi lưu lựa chọn: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DismissAsync(AutonomousActionModel? action)
    {
        if (action == null) return;

        try
        {
            await _repository.DismissActionAsync(action.ActionId);
            PendingActions.Remove(action);
            ActionCount = PendingActions.Count;
        }
        catch
        {
            PendingActions.Remove(action);
            ActionCount = PendingActions.Count;
        }
    }

    [RelayCommand]
    public async Task NavigateToSecondaryAsync(AutonomousActionModel? action)
    {
        if (action?.SecondaryAction?.Route == null) return;

        try
        {
            await Shell.Current.GoToAsync(action.SecondaryAction.Route);
        }
        catch
        {
            // Route navigation fallback
        }
    }

    [RelayCommand]
    public async Task SimulateBankNotificationAsync(string? bankCode)
    {
        if (string.IsNullOrWhiteSpace(bankCode) || IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            RawBankNotificationRequestModel req = bankCode.ToUpperInvariant() switch
            {
                "ACB" => new RawBankNotificationRequestModel(
                    PackageName: "mobile.acb.com.vn",
                    Title: "ACB - Bien dong so du",
                    Message: "TK 123456 -350,000VND luc 12:30. ND: Emart Go Vap",
                    DetectedAmount: 350000m,
                    NotificationTime: DateTime.UtcNow
                ),
                "MOMO" => new RawBankNotificationRequestModel(
                    PackageName: "com.mservice.momotransfer",
                    Title: "MoMo - Thanh toán thành công",
                    Message: "Thanh toán thành công 85.000đ tại Circle K",
                    DetectedAmount: 85000m,
                    NotificationTime: DateTime.UtcNow
                ),
                "TCB" => new RawBankNotificationRequestModel(
                    PackageName: "com.techcombank.mobile",
                    Title: "Techcombank Thông báo",
                    Message: "GD: -120,000VND luc 14:15 tai Highlands Coffee. So du: 5,400,000VND",
                    DetectedAmount: 120000m,
                    NotificationTime: DateTime.UtcNow
                ),
                "VCB" => new RawBankNotificationRequestModel(
                    PackageName: "com.VCB",
                    Title: "Vietcombank VCB Digibank",
                    Message: "SD TK 0071... -500,000 VND luc 10:20. ND: SHOPEEPAY",
                    DetectedAmount: 500000m,
                    NotificationTime: DateTime.UtcNow
                ),
                "MB" => new RawBankNotificationRequestModel(
                    PackageName: "com.mbmobile",
                    Title: "MB Bank Thông báo",
                    Message: "TK 9999 -200,000VND 18:00 ND: Mua xang Petrolimex",
                    DetectedAmount: 200000m,
                    NotificationTime: DateTime.UtcNow
                ),
                _ => new RawBankNotificationRequestModel(
                    PackageName: "com.banking.generic",
                    Title: "Ngân hàng",
                    Message: "GD: -100,000 VND thanh toán dịch vụ",
                    DetectedAmount: 100000m,
                    NotificationTime: DateTime.UtcNow
                )
            };

            var action = await _repository.ProcessNotificationAsync(req);
            if (action != null)
            {
                PendingActions.Insert(0, action);
                ActionCount = PendingActions.Count;
                ActionStatusMessage = $"Đã nhận thông báo biến động {bankCode}: {action.Display?.Amount:N0} đ";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi giả lập biến động số dư: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
