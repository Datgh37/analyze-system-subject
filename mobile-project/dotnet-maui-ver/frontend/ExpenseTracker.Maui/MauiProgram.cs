using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;
using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Notifications;
using ExpenseTracker.Maui.Services.Realtime;
using ExpenseTracker.Maui.Services.Security;
using ExpenseTracker.Maui.ViewModels;
using ExpenseTracker.Maui.Views;
using Microsoft.Extensions.Logging;
using Refit;

namespace ExpenseTracker.Maui;

// Application bootstrap configuring fonts, dependency injection, and Refit API clients
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Dynamic base URL: Android Emulator uses 10.0.2.2 to reach host machine; Windows uses localhost
        var baseApiUrl = DeviceInfo.Platform == DevicePlatform.Android 
            ? "http://10.0.2.2:5018" 
            : "http://localhost:5018";

        // Register token storage and HTTP delegating handlers
        builder.Services.AddSingleton<ITokenStorageService, SecureStorageTokenService>();
        builder.Services.AddTransient<AuthHeaderHandler>();

        // Register declarative Refit API clients
        builder.Services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl));

        builder.Services.AddRefitClient<IUserApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IWalletApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ICategoryApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ITransactionApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IRecurringIncomeApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IPayslipApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAutoRuleApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IReceiptApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ISmartUtilityApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IBudgetApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ISavingsGoalApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<INotificationsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAnalyticsReportApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IForecastingApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<INudgeApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ICoolingOffApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        // Register Clean MVVM Repositories (Swap between Api and Mock for testing)
        builder.Services.AddScoped<IWalletRepository, ApiWalletRepository>();
        builder.Services.AddScoped<ITransactionRepository, ApiTransactionRepository>();
        builder.Services.AddScoped<IBudgetRepository, ApiBudgetRepository>();
        builder.Services.AddScoped<INotificationRepository, ApiNotificationRepository>();
        builder.Services.AddScoped<IAnalyticsReportRepository, ApiAnalyticsReportRepository>();
        builder.Services.AddScoped<IForecastingRepository, ApiForecastingRepository>();
        builder.Services.AddScoped<INudgeRepository, ApiNudgeRepository>();
        builder.Services.AddScoped<ICoolingOffRepository, ApiCoolingOffRepository>();

        // Register Realtime SignalR and Local Notifications Services
        builder.Services.AddSingleton<ISignalRRealtimeService, SignalRRealtimeService>();
        builder.Services.AddSingleton<ILocalNotificationService, LocalNotificationService>();

        // Register client authentication manager
        builder.Services.AddSingleton<IClientAuthService, ClientAuthService>();

        // Register MVVM ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<WalletsViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();
        builder.Services.AddTransient<AutoRulesViewModel>();
        builder.Services.AddTransient<IncomesViewModel>();
        builder.Services.AddTransient<RecurringIncomesViewModel>();
        builder.Services.AddTransient<PayslipOcrViewModel>();
        builder.Services.AddTransient<ExpensesViewModel>();
        builder.Services.AddTransient<ReceiptOcrViewModel>();
        builder.Services.AddTransient<SmartUtilitiesViewModel>();
        builder.Services.AddTransient<BudgetsViewModel>();
        builder.Services.AddTransient<CreateBudgetViewModel>();
        builder.Services.AddTransient<SavingsGoalsViewModel>();
        builder.Services.AddTransient<CreateSavingsGoalViewModel>();
        builder.Services.AddTransient<NotificationHistoryViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();
        builder.Services.AddTransient<ForecastingViewModel>();
        builder.Services.AddTransient<NudgeFeedViewModel>();
        builder.Services.AddTransient<CoolingOffHubViewModel>();

        // Register UI Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<WalletsPage>();
        builder.Services.AddTransient<CategoriesPage>();
        builder.Services.AddTransient<AutoRulesPage>();
        builder.Services.AddTransient<IncomesPage>();
        builder.Services.AddTransient<RecurringIncomesPage>();
        builder.Services.AddTransient<PayslipOcrPage>();
        builder.Services.AddTransient<ExpensesPage>();
        builder.Services.AddTransient<ReceiptOcrPage>();
        builder.Services.AddTransient<SmartUtilitiesPage>();
        builder.Services.AddTransient<BudgetsPage>();
        builder.Services.AddTransient<CreateBudgetPage>();
        builder.Services.AddTransient<SavingsGoalsPage>();
        builder.Services.AddTransient<CreateSavingsGoalPage>();
        builder.Services.AddTransient<NotificationHistoryPage>();
        builder.Services.AddTransient<ReportsPage>();
        builder.Services.AddTransient<ForecastingPage>();
        builder.Services.AddTransient<NudgeFeedPage>();
        builder.Services.AddTransient<CoolingOffHubPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
