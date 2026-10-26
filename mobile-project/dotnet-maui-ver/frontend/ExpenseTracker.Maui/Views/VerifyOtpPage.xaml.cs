using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class VerifyOtpPage : ContentPage
{
    public VerifyOtpPage(VerifyOtpViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
