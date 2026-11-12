namespace ExpenseTracker.Maui.Views;

using ExpenseTracker.Maui.ViewModels;

public partial class DebtOptimizerPage : ContentPage
{
    private readonly DebtOptimizerViewModel _viewModel;

    public DebtOptimizerPage(DebtOptimizerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadInitialDataCommand.ExecuteAsync(null);
    }
}

