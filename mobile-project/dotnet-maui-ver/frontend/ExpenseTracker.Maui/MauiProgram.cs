using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;
using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Config;
using ExpenseTracker.Maui.Services.Notifications;
using ExpenseTracker.Maui.Services.Realtime;
using ExpenseTracker.Maui.Services.Security;
using ExpenseTracker.Maui.ViewModels;
using ExpenseTracker.Maui.ViewModels.AiAgent;
using ExpenseTracker.Maui.ViewModels.Autonomous;
using ExpenseTracker.Maui.ViewModels.ImportExport;
using ExpenseTracker.Maui.Views;
using ExpenseTracker.Maui.Views.AiAgent;
using ExpenseTracker.Maui.Views.ImportExport;
using ExpenseTracker.Maui.Repositories.AiAgent;
using ExpenseTracker.Maui.Repositories.Autonomous;
using ExpenseTracker.Maui.Repositories.ImportExport;
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

        // Cấu hình Dịch vụ Quản lý Máy chủ & Chuyển mạch Dynamic BaseAddress (Kiềng 3 Chân Demo)
        builder.Services.AddSingleton<IServerConfigService, ServerConfigService>();
        builder.Services.AddTransient<DynamicBaseAddressHandler>();
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddSingleton<ITokenStorageService, SecureStorageTokenService>();

        var baseApiUrl = ServerConfigService.GetDefaultServerUrl();

        // Register declarative Refit API clients kèm DynamicBaseAddressHandler & AuthHeaderHandler
        builder.Services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>();

        builder.Services.AddRefitClient<IUserApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IWalletApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ICategoryApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ITransactionApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IRecurringIncomeApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IPayslipApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAutoRuleApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IReceiptApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ISmartUtilityApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IBudgetApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ISavingsGoalApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<INotificationsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAnalyticsReportApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IForecastingApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<INudgeApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ICoolingOffApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IVoiceAgentApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IDataImportApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAutonomousActionsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        // =========================================================================
        // ĐĂNG KÝ TOÀN BỘ INSTANCES (MOCK & API IMPLEMENTATIONS)
        // =========================================================================
        builder.Services.AddScoped<MockWalletRepository>();
        builder.Services.AddScoped<ApiWalletRepository>();
        builder.Services.AddScoped<MockTransactionRepository>();
        builder.Services.AddScoped<ApiTransactionRepository>();
        builder.Services.AddScoped<MockBudgetRepository>();
        builder.Services.AddScoped<ApiBudgetRepository>();
        builder.Services.AddScoped<MockCategoryRepository>();
        builder.Services.AddScoped<ApiCategoryRepository>();
        builder.Services.AddScoped<MockSavingsGoalRepository>();
        builder.Services.AddScoped<ApiSavingsGoalRepository>();
        builder.Services.AddScoped<MockAutoRuleRepository>();
        builder.Services.AddScoped<ApiAutoRuleRepository>();
        builder.Services.AddScoped<MockRecurringIncomeRepository>();
        builder.Services.AddScoped<ApiRecurringIncomeRepository>();
        builder.Services.AddScoped<MockSmartUtilityRepository>();
        builder.Services.AddScoped<ApiSmartUtilityRepository>();
        builder.Services.AddScoped<MockReceiptRepository>();
        builder.Services.AddScoped<ApiReceiptRepository>();
        builder.Services.AddScoped<MockPayslipRepository>();
        builder.Services.AddScoped<ApiPayslipRepository>();
        builder.Services.AddSingleton<MockNotificationRepository>();
        builder.Services.AddScoped<ApiNotificationRepository>();
        builder.Services.AddScoped<MockAnalyticsReportRepository>();
        builder.Services.AddScoped<ApiAnalyticsReportRepository>();
        builder.Services.AddScoped<MockForecastingRepository>();
        builder.Services.AddScoped<ApiForecastingRepository>();
        builder.Services.AddScoped<MockNudgeRepository>();
        builder.Services.AddScoped<ApiNudgeRepository>();
        builder.Services.AddScoped<MockCoolingOffRepository>();
        builder.Services.AddScoped<ApiCoolingOffRepository>();
        builder.Services.AddScoped<MockVoiceAgentRepository>();
        builder.Services.AddScoped<ApiVoiceAgentRepository>();
        builder.Services.AddScoped<MockDataImportRepository>();
        builder.Services.AddScoped<ApiDataImportRepository>();
        builder.Services.AddScoped<MockAutonomousActionRepository>();
        builder.Services.AddScoped<ApiAutonomousActionRepository>();

        // =========================================================================
        // DYNAMIC REPOSITORY FACTORY: TỰ ĐỘNG CHUYỂN ĐỔI GIỮA ONLINE VÀ OFFLINE MOCK
        // Khi IsLiveApiMode = true -> Dùng ApiRepository; khi false -> Dùng MockRepository 0ms
        // =========================================================================
        builder.Services.AddScoped<IWalletRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiWalletRepository>() 
                : sp.GetRequiredService<MockWalletRepository>());

        builder.Services.AddScoped<ITransactionRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiTransactionRepository>() 
                : sp.GetRequiredService<MockTransactionRepository>());

        builder.Services.AddScoped<IBudgetRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiBudgetRepository>() 
                : sp.GetRequiredService<MockBudgetRepository>());

        builder.Services.AddScoped<ICategoryRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiCategoryRepository>() 
                : sp.GetRequiredService<MockCategoryRepository>());

        builder.Services.AddScoped<ISavingsGoalRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiSavingsGoalRepository>() 
                : sp.GetRequiredService<MockSavingsGoalRepository>());

        builder.Services.AddScoped<IAutoRuleRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiAutoRuleRepository>() 
                : sp.GetRequiredService<MockAutoRuleRepository>());

        builder.Services.AddScoped<IRecurringIncomeRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiRecurringIncomeRepository>() 
                : sp.GetRequiredService<MockRecurringIncomeRepository>());

        builder.Services.AddScoped<ISmartUtilityRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiSmartUtilityRepository>() 
                : sp.GetRequiredService<MockSmartUtilityRepository>());

        builder.Services.AddScoped<IReceiptRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiReceiptRepository>() 
                : sp.GetRequiredService<MockReceiptRepository>());

        builder.Services.AddScoped<IPayslipRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiPayslipRepository>() 
                : sp.GetRequiredService<MockPayslipRepository>());

        builder.Services.AddSingleton<INotificationRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiNotificationRepository>() 
                : sp.GetRequiredService<MockNotificationRepository>());

        builder.Services.AddScoped<IAnalyticsReportRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiAnalyticsReportRepository>() 
                : sp.GetRequiredService<MockAnalyticsReportRepository>());

        builder.Services.AddScoped<IForecastingRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiForecastingRepository>() 
                : sp.GetRequiredService<MockForecastingRepository>());

        builder.Services.AddScoped<INudgeRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiNudgeRepository>() 
                : sp.GetRequiredService<MockNudgeRepository>());

        builder.Services.AddScoped<ICoolingOffRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiCoolingOffRepository>() 
                : sp.GetRequiredService<MockCoolingOffRepository>());

        builder.Services.AddScoped<IVoiceAgentRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiVoiceAgentRepository>() 
                : sp.GetRequiredService<MockVoiceAgentRepository>());

        builder.Services.AddScoped<IDataImportRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiDataImportRepository>() 
                : sp.GetRequiredService<MockDataImportRepository>());

        builder.Services.AddScoped<IAutonomousActionRepository>(sp =>
            sp.GetRequiredService<IServerConfigService>().IsLiveApiMode 
                ? sp.GetRequiredService<ApiAutonomousActionRepository>() 
                : sp.GetRequiredService<MockAutonomousActionRepository>());

        // Register Realtime SignalR, Local & Native Notifications Services
        builder.Services.AddSingleton<ISignalRRealtimeService, SignalRRealtimeService>();
        builder.Services.AddSingleton<ILocalNotificationService, LocalNotificationService>();
#if ANDROID
        builder.Services.AddSingleton<INativeNotificationService, ExpenseTracker.Maui.Platforms.Android.Services.AndroidNativeNotificationService>();
#else
        builder.Services.AddSingleton<INativeNotificationService, DesktopFallbackNotificationService>();
#endif

        // Register client authentication manager
        builder.Services.AddSingleton<IClientAuthService, ClientAuthService>();

        // Register MVVM ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<VerifyOtpViewModel>();
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
        builder.Services.AddTransient<VoiceFloatingMicViewModel>();
        builder.Services.AddTransient<ImportPreviewViewModel>();
        builder.Services.AddTransient<ActionInboxViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Register UI Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<VerifyOtpPage>();
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
        builder.Services.AddTransient<VoiceMicModalPage>();
        builder.Services.AddTransient<ImportPreviewPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
