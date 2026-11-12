using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class IncomesPage : ContentPage
{
    private readonly IncomesViewModel _viewModel;

    public IncomesPage(IncomesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadIncomesAsync();
    }

    private async void OnQuickScanPayslipClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("PayslipOcrPage");
    }
}
