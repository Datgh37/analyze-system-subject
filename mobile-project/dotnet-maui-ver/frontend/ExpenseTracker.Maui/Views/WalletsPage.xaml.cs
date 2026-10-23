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
}
