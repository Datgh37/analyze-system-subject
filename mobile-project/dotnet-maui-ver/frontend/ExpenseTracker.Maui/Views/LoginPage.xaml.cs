using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

// Code-behind for LoginPage binding LoginViewModel
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
