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
}
