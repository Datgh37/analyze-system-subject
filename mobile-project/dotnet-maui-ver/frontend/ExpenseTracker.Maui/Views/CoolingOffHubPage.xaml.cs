using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class CoolingOffHubPage : ContentPage
{
    private readonly CoolingOffHubViewModel _viewModel;

    public CoolingOffHubPage(CoolingOffHubViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPendingTransactionsCommand.ExecuteAsync(null);
    }
}
