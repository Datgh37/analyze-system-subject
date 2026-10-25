using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application;

// Dependency injection extension registering application services and validators
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IRecurringIncomeService, RecurringIncomeService>();
        services.AddScoped<IPayslipOcrService, PayslipOcrService>();
        services.AddScoped<IAutoCategorizationService, AutoCategorizationService>();
        services.AddScoped<IReceiptOcrService, ReceiptOcrService>();
        services.AddScoped<ISubscriptionDetectorService, SubscriptionDetectorService>();
        services.AddScoped<IVietQrService, VietQrService>();
        services.AddScoped<IVoiceExpenseParserService, VoiceExpenseParserService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IAdaptiveDsaService, AdaptiveDsaService>();
        services.AddScoped<ISavingsGoalService, SavingsGoalService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAnalyticsReportService, AnalyticsReportService>();
        services.AddScoped<IForecastingService, ForecastingService>();
        services.AddScoped<INudgeEngineService, NudgeEngineService>();
        services.AddScoped<ICoolingOffService, CoolingOffService>();
        services.AddScoped<IImagePreProcessingService, ImagePreProcessingService>();
        services.AddScoped<IVoiceIntentRouterService, VoiceIntentRouterService>();
        services.AddScoped<IHybridDataImportService, HybridDataImportService>();
        services.AddScoped<IReportExportService, ReportExportService>();
        services.AddScoped<IAutonomousActionEngineService, AutonomousActionEngineService>();

        return services;
    }
}
