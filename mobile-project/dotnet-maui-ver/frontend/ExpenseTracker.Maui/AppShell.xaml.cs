using ExpenseTracker.Maui.Views;

namespace ExpenseTracker.Maui;

// Application shell managing top-level navigation routes
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        Routing.RegisterRoute("WalletsPage", typeof(WalletsPage));
        Routing.RegisterRoute("BudgetsPage", typeof(BudgetsPage));
        Routing.RegisterRoute("CreateBudgetPage", typeof(CreateBudgetPage));
        Routing.RegisterRoute("SavingsGoalsPage", typeof(SavingsGoalsPage));
        Routing.RegisterRoute("CreateSavingsGoalPage", typeof(CreateSavingsGoalPage));
        Routing.RegisterRoute("NotificationHistoryPage", typeof(NotificationHistoryPage));
        Routing.RegisterRoute("ReportsPage", typeof(ReportsPage));
        Routing.RegisterRoute("ForecastingPage", typeof(ForecastingPage));
        Routing.RegisterRoute("NudgeFeedPage", typeof(NudgeFeedPage));
        Routing.RegisterRoute("CoolingOffHubPage", typeof(CoolingOffHubPage));
    }
}
