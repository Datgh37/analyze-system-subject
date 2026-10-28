using ExpenseTracker.Application.Common.Settings;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Infrastructure.Ai;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Security;
using ExpenseTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure;

// Dependency injection extension registering infrastructure and persistence services
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        bool isPostgresAvailable = CheckPostgreSqlAvailability(connectionString);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (isPostgresAvailable)
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                var dbPath = Path.Combine(AppContext.BaseDirectory, "expensetracker.db");
                options.UseSqlite($"Data Source={dbPath}");
            }
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IBalanceOcrService, MockBalanceOcrService>();
        services.AddScoped<IEmailService, EmailService>();

        // Register Google Gemini AI Gateway & configuration
        services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));
        services.AddHttpClient<IGeminiApiClient, GeminiApiClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        // Register Push Notification Services (Adapter Pattern: FCM v1 with Graceful Mock Fallback)
        services.AddHttpClient("FcmClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        services.AddScoped<MockPushNotificationSender>();
        services.AddScoped<IPushNotificationSender, FirebasePushNotificationSender>();

        return services;
    }

    private static bool CheckPostgreSqlAvailability(string connectionString)
    {
        try
        {
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
            var host = string.IsNullOrWhiteSpace(builder.Host) ? "localhost" : builder.Host;
            var port = builder.Port > 0 ? builder.Port : 5432;

            using var client = new System.Net.Sockets.TcpClient();
            var result = client.BeginConnect(host, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
            if (!success)
            {
                return false;
            }
            client.EndConnect(result);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
