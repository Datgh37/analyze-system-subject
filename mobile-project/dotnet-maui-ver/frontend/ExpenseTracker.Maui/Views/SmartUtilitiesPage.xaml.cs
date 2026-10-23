using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class SmartUtilitiesPage : ContentPage
{
    private readonly SmartUtilitiesViewModel _viewModel;

    public SmartUtilitiesPage(SmartUtilitiesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeDataCommand.ExecuteAsync(null);
    }
}
