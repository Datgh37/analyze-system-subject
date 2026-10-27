namespace ExpenseTracker.Maui.Views.Components;

public partial class BottomNavDockView : ContentView
{
    public static readonly BindableProperty ActiveTabProperty =
        BindableProperty.Create(
            nameof(ActiveTab),
            typeof(string),
            typeof(BottomNavDockView),
            "Wallets",
            propertyChanged: OnActiveTabPropertyChanged);

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    public Color WalletsTabColor => GetTabColor("Wallets");
    public Color ExpensesTabColor => GetTabColor("Expenses");
    public Color BudgetsTabColor => GetTabColor("Budgets");
    public Color ReportsTabColor => GetTabColor("Reports");

    public BottomNavDockView()
    {
        InitializeComponent();
    }

    private static void OnActiveTabPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BottomNavDockView view)
        {
            view.OnPropertyChanged(nameof(WalletsTabColor));
            view.OnPropertyChanged(nameof(ExpensesTabColor));
            view.OnPropertyChanged(nameof(BudgetsTabColor));
            view.OnPropertyChanged(nameof(ReportsTabColor));
        }
    }

    private Color GetTabColor(string tabName)
    {
        bool isActive = string.Equals(ActiveTab, tabName, StringComparison.OrdinalIgnoreCase);
        if (isActive)
        {
            return Color.FromArgb("#38BDF8"); // Active Cyan Blue
        }

        return Color.FromArgb("#94A3B8"); // Inactive Slate Gray
    }

    private async void OnWalletsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//WalletsPage");
    }

    private async void OnExpensesTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ExpensesPage");
    }

    private async void OnBudgetsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//BudgetsPage");
    }

    private async void OnReportsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ReportsPage");
    }

    private async void OnVoiceMicTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("VoiceMicModalPage");
    }
}
