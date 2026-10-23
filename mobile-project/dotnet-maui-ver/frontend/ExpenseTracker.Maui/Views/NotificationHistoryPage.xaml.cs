using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class NotificationHistoryPage : ContentPage
{
    private readonly NotificationHistoryViewModel _viewModel;

    public NotificationHistoryPage(NotificationHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadNotificationsAsync();
    }
}
