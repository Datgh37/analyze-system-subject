using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;
using ExpenseTracker.Maui.Services.Auth;
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

        builder.Services.AddRefitClient<IVoiceAgentApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IDataImportApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAutonomousActionsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        // =========================================================================
        // CHẾ ĐỘ 1: DÙNG MOCK DATA OFFLINE (ĐANG BẬT ĐỂ TEST GIAO DIỆN & VIEWMODELS)
        // Không cần khởi động Docker hay Backend WebAPI, hệ thống vẫn có đầy đủ dữ liệu sống động.
        // =========================================================================
        builder.Services.AddScoped<IWalletRepository, MockWalletRepository>();
        builder.Services.AddScoped<ITransactionRepository, MockTransactionRepository>();
        builder.Services.AddScoped<IBudgetRepository, MockBudgetRepository>();
        builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();
        builder.Services.AddScoped<ISavingsGoalRepository, MockSavingsGoalRepository>();
        builder.Services.AddScoped<IAutoRuleRepository, MockAutoRuleRepository>();
        builder.Services.AddScoped<IRecurringIncomeRepository, MockRecurringIncomeRepository>();
        builder.Services.AddScoped<ISmartUtilityRepository, MockSmartUtilityRepository>();
        builder.Services.AddScoped<IReceiptRepository, MockReceiptRepository>();
        builder.Services.AddScoped<IPayslipRepository, MockPayslipRepository>();
        builder.Services.AddSingleton<INotificationRepository, MockNotificationRepository>();
        builder.Services.AddScoped<IAnalyticsReportRepository, MockAnalyticsReportRepository>();
        builder.Services.AddScoped<IForecastingRepository, MockForecastingRepository>();
        builder.Services.AddScoped<INudgeRepository, MockNudgeRepository>();
        builder.Services.AddScoped<ICoolingOffRepository, MockCoolingOffRepository>();
        builder.Services.AddScoped<IVoiceAgentRepository, MockVoiceAgentRepository>();
        builder.Services.AddScoped<IDataImportRepository, MockDataImportRepository>();
        builder.Services.AddScoped<IAutonomousActionRepository, MockAutonomousActionRepository>();

        // =========================================================================
        // CHẾ ĐỘ 2: DÙNG API THẬT KẾT NỐI BACKEND ASP.NET CORE (ĐÃ ĐÓNG NOTE)
        // Khi cần kết nối Backend thật và Docker PostgreSQL, hãy mở ghi chú (uncomment) các dòng dưới đây
        // và đóng ghi chú (comment) các dòng Mock ở trên:
        // =========================================================================
        // builder.Services.AddScoped<IWalletRepository, ApiWalletRepository>();
        // builder.Services.AddScoped<ITransactionRepository, ApiTransactionRepository>();
        // builder.Services.AddScoped<IBudgetRepository, ApiBudgetRepository>();
        // builder.Services.AddScoped<ICategoryRepository, ApiCategoryRepository>();
        // builder.Services.AddScoped<ISavingsGoalRepository, ApiSavingsGoalRepository>();
        // builder.Services.AddScoped<IAutoRuleRepository, ApiAutoRuleRepository>();
        // builder.Services.AddScoped<IRecurringIncomeRepository, ApiRecurringIncomeRepository>();
        // builder.Services.AddScoped<ISmartUtilityRepository, ApiSmartUtilityRepository>();
        // builder.Services.AddScoped<IReceiptRepository, ApiReceiptRepository>();
        // builder.Services.AddScoped<IPayslipRepository, ApiPayslipRepository>();
        // builder.Services.AddScoped<INotificationRepository, ApiNotificationRepository>();
        // builder.Services.AddScoped<IAnalyticsReportRepository, ApiAnalyticsReportRepository>();
        // builder.Services.AddScoped<IForecastingRepository, ApiForecastingRepository>();
        // builder.Services.AddScoped<INudgeRepository, ApiNudgeRepository>();
        // builder.Services.AddScoped<ICoolingOffRepository, ApiCoolingOffRepository>();
        // builder.Services.AddScoped<IVoiceAgentRepository, ApiVoiceAgentRepository>();
        // builder.Services.AddScoped<IDataImportRepository, ApiDataImportRepository>();
        // builder.Services.AddScoped<IAutonomousActionRepository, ApiAutonomousActionRepository>();

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
