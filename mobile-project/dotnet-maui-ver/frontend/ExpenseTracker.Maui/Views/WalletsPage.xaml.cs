using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

// Code-behind for WalletsPage auto-triggering data load on appearing
public partial class WalletsPage : ContentPage
{
    private readonly WalletsViewModel _viewModel;

    public WalletsPage(WalletsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWalletsCommand.ExecuteAsync(null);
    }

    private async void OnNotificationsClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("NotificationHistoryPage");
        }
    }

    private async void OnAddWalletClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("ManageWalletsPage");
        }
    }

    private async void OnManageWalletsClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("ManageWalletsPage");
        }
    }

    private async void OnQuickIncomeClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("//IncomesPage");
    }

    private async void OnQuickExpenseClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("//ExpensesPage");
    }

    private async void OnQuickOcrClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("//ReceiptOcrPage");
    }

    private async void OnQuickBudgetClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("//BudgetsPage");
    }

    private async void OnQuickTransferClicked(object? sender, EventArgs e)
    {
        if (decimal.TryParse(TransferAmountEntry.Text, out var amount) && amount > 0)
        {
            if (_viewModel.Wallets.Count >= 2)
            {
                var fromWallet = _viewModel.Wallets[0];
                var toWallet = _viewModel.Wallets[1];
                var model = new Models.Wallets.TransferMoneyModel(fromWallet.Id, toWallet.Id, amount, DateTime.Now, "Chuyển tiền nội bộ nhanh");
                await _viewModel.TransferMoneyCommand.ExecuteAsync(model);
                TransferAmountEntry.Text = string.Empty;
                await DisplayAlert("Thành công", $"Đã chuyển {amount:N0} đ từ {fromWallet.Name} sang {toWallet.Name}!", "OK");
            }
            else
            {
                await DisplayAlert("Thông báo", "Bạn cần ít nhất 2 ví để thực hiện chuyển tiền nội bộ.", "Đóng");
            }
        }
        else
        {
            await DisplayAlert("Thông báo", "Vui lòng nhập số tiền hợp lệ cần chuyển.", "Đóng");
        }
    }
}
