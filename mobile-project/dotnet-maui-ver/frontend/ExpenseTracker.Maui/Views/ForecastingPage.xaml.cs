using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

// Code-behind for Forecasting and Deficit Risk Radar page
public partial class ForecastingPage : ContentPage
{
    private readonly ForecastingViewModel _viewModel;

    public ForecastingPage(ForecastingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadForecastDataAsync();
    }
}
