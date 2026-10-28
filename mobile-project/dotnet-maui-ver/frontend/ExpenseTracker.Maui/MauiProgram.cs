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
        Action<HttpClient> configureClient = c =>
        {
            c.BaseAddress = new Uri(baseApiUrl);
            c.Timeout = TimeSpan.FromSeconds(10);
        };

        var refitSettings = RefitSettingsFactory.GetSettings();

        // Register declarative Refit API clients kèm DynamicBaseAddressHandler & AuthHeaderHandler
        builder.Services.AddRefitClient<IAuthApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>();

        builder.Services.AddRefitClient<IUserApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IWalletApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ICategoryApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ITransactionApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IRecurringIncomeApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IPayslipApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAutoRuleApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IReceiptApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ISmartUtilityApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IBudgetApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ISavingsGoalApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<INotificationsApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAnalyticsReportApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IForecastingApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<INudgeApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<ICoolingOffApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IVoiceAgentApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IDataImportApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAutonomousActionsApi>(refitSettings)
            .ConfigureHttpClient(configureClient)
            .AddHttpMessageHandler<DynamicBaseAddressHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        // =========================================================================
        // ĐĂNG KÝ INSTANCES THẬT & MOCK (SINGLETON CHO MOCK ĐỂ GIỮ STATE BỘ NHỚ RAM)
        // =========================================================================
        builder.Services.AddSingleton<MockWalletRepository>();
        builder.Services.AddScoped<ApiWalletRepository>();
        builder.Services.AddSingleton<MockTransactionRepository>();
        builder.Services.AddScoped<ApiTransactionRepository>();
        builder.Services.AddSingleton<MockBudgetRepository>();
        builder.Services.AddScoped<ApiBudgetRepository>();
        builder.Services.AddSingleton<MockCategoryRepository>();
        builder.Services.AddScoped<ApiCategoryRepository>();
        builder.Services.AddSingleton<MockSavingsGoalRepository>();
        builder.Services.AddScoped<ApiSavingsGoalRepository>();
        builder.Services.AddSingleton<MockAutoRuleRepository>();
        builder.Services.AddScoped<ApiAutoRuleRepository>();
        builder.Services.AddSingleton<MockRecurringIncomeRepository>();
        builder.Services.AddScoped<ApiRecurringIncomeRepository>();
        builder.Services.AddSingleton<MockSmartUtilityRepository>();
        builder.Services.AddScoped<ApiSmartUtilityRepository>();
        builder.Services.AddSingleton<MockReceiptRepository>();
        builder.Services.AddScoped<ApiReceiptRepository>();
        builder.Services.AddSingleton<MockPayslipRepository>();
        builder.Services.AddScoped<ApiPayslipRepository>();
        builder.Services.AddSingleton<MockNotificationRepository>();
        builder.Services.AddScoped<ApiNotificationRepository>();
        builder.Services.AddSingleton<MockAnalyticsReportRepository>();
        builder.Services.AddScoped<ApiAnalyticsReportRepository>();
        builder.Services.AddSingleton<MockForecastingRepository>();
        builder.Services.AddScoped<ApiForecastingRepository>();
        builder.Services.AddSingleton<MockNudgeRepository>();
        builder.Services.AddScoped<ApiNudgeRepository>();
        builder.Services.AddSingleton<MockCoolingOffRepository>();
        builder.Services.AddScoped<ApiCoolingOffRepository>();
        builder.Services.AddSingleton<MockVoiceAgentRepository>();
        builder.Services.AddScoped<ApiVoiceAgentRepository>();
        builder.Services.AddSingleton<MockDataImportRepository>();
        builder.Services.AddScoped<ApiDataImportRepository>();
        builder.Services.AddSingleton<MockAutonomousActionRepository>();
        builder.Services.AddScoped<ApiAutonomousActionRepository>();

        // =========================================================================
        // DYNAMIC DISPATCH PROXY: ĐIỀU HƯỚNG RUNTIME THỜI GIAN THỰC 0MS
        // Tự động phân nhánh ngay lập tức khi người dùng gạt toggle sang Offline Mode
        // =========================================================================
        builder.Services.AddSingleton<IWalletRepository>(sp =>
            DynamicRepositoryProxy<IWalletRepository>.Create(sp, typeof(ApiWalletRepository), typeof(MockWalletRepository)));

        builder.Services.AddSingleton<ITransactionRepository>(sp =>
            DynamicRepositoryProxy<ITransactionRepository>.Create(sp, typeof(ApiTransactionRepository), typeof(MockTransactionRepository)));

        builder.Services.AddSingleton<IBudgetRepository>(sp =>
            DynamicRepositoryProxy<IBudgetRepository>.Create(sp, typeof(ApiBudgetRepository), typeof(MockBudgetRepository)));

        builder.Services.AddSingleton<ICategoryRepository>(sp =>
            DynamicRepositoryProxy<ICategoryRepository>.Create(sp, typeof(ApiCategoryRepository), typeof(MockCategoryRepository)));

        builder.Services.AddSingleton<ISavingsGoalRepository>(sp =>
            DynamicRepositoryProxy<ISavingsGoalRepository>.Create(sp, typeof(ApiSavingsGoalRepository), typeof(MockSavingsGoalRepository)));

        builder.Services.AddSingleton<IAutoRuleRepository>(sp =>
            DynamicRepositoryProxy<IAutoRuleRepository>.Create(sp, typeof(ApiAutoRuleRepository), typeof(MockAutoRuleRepository)));

        builder.Services.AddSingleton<IRecurringIncomeRepository>(sp =>
            DynamicRepositoryProxy<IRecurringIncomeRepository>.Create(sp, typeof(ApiRecurringIncomeRepository), typeof(MockRecurringIncomeRepository)));

        builder.Services.AddSingleton<ISmartUtilityRepository>(sp =>
            DynamicRepositoryProxy<ISmartUtilityRepository>.Create(sp, typeof(ApiSmartUtilityRepository), typeof(MockSmartUtilityRepository)));

        builder.Services.AddSingleton<IReceiptRepository>(sp =>
            DynamicRepositoryProxy<IReceiptRepository>.Create(sp, typeof(ApiReceiptRepository), typeof(MockReceiptRepository)));

        builder.Services.AddSingleton<IPayslipRepository>(sp =>
            DynamicRepositoryProxy<IPayslipRepository>.Create(sp, typeof(ApiPayslipRepository), typeof(MockPayslipRepository)));

        builder.Services.AddSingleton<INotificationRepository>(sp =>
            DynamicRepositoryProxy<INotificationRepository>.Create(sp, typeof(ApiNotificationRepository), typeof(MockNotificationRepository)));

        builder.Services.AddSingleton<IAnalyticsReportRepository>(sp =>
            DynamicRepositoryProxy<IAnalyticsReportRepository>.Create(sp, typeof(ApiAnalyticsReportRepository), typeof(MockAnalyticsReportRepository)));

        builder.Services.AddSingleton<IForecastingRepository>(sp =>
            DynamicRepositoryProxy<IForecastingRepository>.Create(sp, typeof(ApiForecastingRepository), typeof(MockForecastingRepository)));

        builder.Services.AddSingleton<INudgeRepository>(sp =>
            DynamicRepositoryProxy<INudgeRepository>.Create(sp, typeof(ApiNudgeRepository), typeof(MockNudgeRepository)));

        builder.Services.AddSingleton<ICoolingOffRepository>(sp =>
            DynamicRepositoryProxy<ICoolingOffRepository>.Create(sp, typeof(ApiCoolingOffRepository), typeof(MockCoolingOffRepository)));

        builder.Services.AddSingleton<IVoiceAgentRepository>(sp =>
            DynamicRepositoryProxy<IVoiceAgentRepository>.Create(sp, typeof(ApiVoiceAgentRepository), typeof(MockVoiceAgentRepository)));

        builder.Services.AddSingleton<IDataImportRepository>(sp =>
            DynamicRepositoryProxy<IDataImportRepository>.Create(sp, typeof(ApiDataImportRepository), typeof(MockDataImportRepository)));

        builder.Services.AddSingleton<IAutonomousActionRepository>(sp =>
            DynamicRepositoryProxy<IAutonomousActionRepository>.Create(sp, typeof(ApiAutonomousActionRepository), typeof(MockAutonomousActionRepository)));

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
