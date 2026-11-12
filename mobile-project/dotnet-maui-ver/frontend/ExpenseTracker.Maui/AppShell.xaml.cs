using ExpenseTracker.Maui.Views;
using ExpenseTracker.Maui.Views.AiAgent;
using ExpenseTracker.Maui.Views.ImportExport;

namespace ExpenseTracker.Maui;

// Application shell managing top-level navigation routes
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("QuickLoginPage", typeof(QuickLoginPage));
        Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        Routing.RegisterRoute("VerifyOtpPage", typeof(VerifyOtpPage));
        Routing.RegisterRoute("WalletsPage", typeof(WalletsPage));
        Routing.RegisterRoute("ManageWalletsPage", typeof(ManageWalletsPage));
        Routing.RegisterRoute("ExpensesPage", typeof(ExpensesPage));
        Routing.RegisterRoute("IncomesPage", typeof(IncomesPage));
        Routing.RegisterRoute("RecurringIncomesPage", typeof(RecurringIncomesPage));
        Routing.RegisterRoute("BudgetsPage", typeof(BudgetsPage));
        Routing.RegisterRoute("CreateBudgetPage", typeof(CreateBudgetPage));
        Routing.RegisterRoute("SavingsGoalsPage", typeof(SavingsGoalsPage));
        Routing.RegisterRoute("CreateSavingsGoalPage", typeof(CreateSavingsGoalPage));
        Routing.RegisterRoute("CategoriesPage", typeof(CategoriesPage));
        Routing.RegisterRoute("AutoRulesPage", typeof(AutoRulesPage));
        Routing.RegisterRoute("NotificationHistoryPage", typeof(NotificationHistoryPage));
        Routing.RegisterRoute("ReportsPage", typeof(ReportsPage));
        Routing.RegisterRoute("ForecastingPage", typeof(ForecastingPage));
        Routing.RegisterRoute("NudgeFeedPage", typeof(NudgeFeedPage));
        Routing.RegisterRoute("CoolingOffHubPage", typeof(CoolingOffHubPage));
        Routing.RegisterRoute("VoiceMicModalPage", typeof(VoiceMicModalPage));
        Routing.RegisterRoute("ImportPreviewPage", typeof(ImportPreviewPage));
        Routing.RegisterRoute("DebtOptimizerPage", typeof(DebtOptimizerPage));
        Routing.RegisterRoute("EmergencyFundPage", typeof(EmergencyFundPage));
        Routing.RegisterRoute("SubscriptionsPage", typeof(SubscriptionsPage));
        Routing.RegisterRoute("ReceiptOcrPage", typeof(ReceiptOcrPage));
        Routing.RegisterRoute("PayslipOcrPage", typeof(PayslipOcrPage));
        Routing.RegisterRoute("SmartUtilitiesPage", typeof(SmartUtilitiesPage));
        Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
    }
}
