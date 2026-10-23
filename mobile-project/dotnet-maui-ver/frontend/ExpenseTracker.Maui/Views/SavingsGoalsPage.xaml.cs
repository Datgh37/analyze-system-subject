using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class SavingsGoalsPage : ContentPage
{
    private readonly SavingsGoalsViewModel _viewModel;

    public SavingsGoalsPage(SavingsGoalsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadGoalsAndJarsCommand.ExecuteAsync(null);
    }
}
