using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace ExpenseTracker.Maui.Views.Autonomous;

public partial class SmartActionCardView : ContentView
{
    public static readonly BindableProperty QuickCommitCommandProperty =
        BindableProperty.Create(nameof(QuickCommitCommand), typeof(ICommand), typeof(SmartActionCardView));

    public static readonly BindableProperty DismissCommandProperty =
        BindableProperty.Create(nameof(DismissCommand), typeof(ICommand), typeof(SmartActionCardView));

    public static readonly BindableProperty NavigateCommandProperty =
        BindableProperty.Create(nameof(NavigateCommand), typeof(ICommand), typeof(SmartActionCardView));

    public static readonly BindableProperty SelectChoiceCommandProperty =
        BindableProperty.Create(nameof(SelectChoiceCommand), typeof(ICommand), typeof(SmartActionCardView));

    public ICommand? QuickCommitCommand
    {
        get => (ICommand?)GetValue(QuickCommitCommandProperty);
        set => SetValue(QuickCommitCommandProperty, value);
    }

    public ICommand? DismissCommand
    {
        get => (ICommand?)GetValue(DismissCommandProperty);
        set => SetValue(DismissCommandProperty, value);
    }

    public ICommand? NavigateCommand
    {
        get => (ICommand?)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public ICommand? SelectChoiceCommand
    {
        get => (ICommand?)GetValue(SelectChoiceCommandProperty);
        set => SetValue(SelectChoiceCommandProperty, value);
    }

    public SmartActionCardView()
    {
        InitializeComponent();
    }
}
