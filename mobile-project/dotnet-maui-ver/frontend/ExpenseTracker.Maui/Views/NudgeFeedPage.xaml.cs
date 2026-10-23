using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class NudgeFeedPage : ContentPage
{
    private readonly NudgeFeedViewModel _viewModel;

    public NudgeFeedPage(NudgeFeedViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadNudgesCommand.ExecuteAsync(null);
    }
}
