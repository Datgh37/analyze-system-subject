using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using ExpenseTracker.Application;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.WebApi.Hubs;
using ExpenseTracker.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

// Allow Npgsql to accept all DateTime kinds (UTC, Local, Unspecified) seamlessly
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for professional, clean, ANSI-colorized structured logging
builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Code,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

// Load optional local configuration file (ignored by Git for secret keys)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Register Clean Architecture layer dependencies
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register API controllers and documentation with JWT Bearer authorization in Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "ExpenseTracker Web API",
        Version = "v1",
        Description = "Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Nhập JWT Token của bạn (Dán chuỗi token eyJ... hoặc 'Bearer eyJ...').",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

// Configure JWT Bearer authentication scheme
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"] 
    ?? "ExpenseTracker_UltraSecure_DevKey_AtLeast32BytesLong!";
var issuer = builder.Configuration["JwtSettings:Issuer"] ?? "ExpenseTracker.WebApi";
var audience = builder.Configuration["JwtSettings:Audience"] ?? "ExpenseTracker.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader["Bearer Bearer ".Length..].Trim();
                }

                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/financial"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Register in-memory cache and native SignalR realtime hub
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddScoped<IFinancialRealtimeNotifier, FinancialRealtimeNotifier>();

// Configure CORS for local development and mobile client access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Rate Limiting for resource-intensive AI endpoints (5 requests / minute per user)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"message\":\"Hệ thống đang bận. Vui lòng thử lại sau 1 phút.\"}",
            cancellationToken: token);
    };

    options.AddPolicy("ai_policy", httpContext =>
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContext.User.FindFirst("sub")?.Value
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous_client";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});

var app = builder.Build();

// Register global exception handler middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Log every HTTP request in a clean single-line format
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0} ms";
});

// Enable Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Health check endpoint for container and client ping
app.MapGet("/api/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
})).WithName("GetHealthStatus");

app.MapControllers();
app.MapHub<FinancialHub>("/hubs/financial");

// Seed initial system financial categories and admin account (graceful fallback if PostgreSQL is not yet running)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ExpenseTracker.Infrastructure.Persistence.ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<ExpenseTracker.Application.Interfaces.IPasswordHasher>();

        if (Microsoft.EntityFrameworkCore.SqliteDatabaseFacadeExtensions.IsSqlite(context.Database))
        {
            logger.LogInformation("[Database Engine] Đang chạy trên CSDL SQLite cục bộ (expensetracker.db) tự động, không phụ thuộc Docker.");
            await context.Database.EnsureCreatedAsync();
            await ExpenseTracker.Infrastructure.Persistence.DatabaseSeeder.SeedDefaultCategoriesAsync(context, logger);
            await ExpenseTracker.Infrastructure.Persistence.DatabaseSeeder.SeedAdminUserAsync(context, passwordHasher, logger);
        }
        else
        {
            logger.LogInformation("[Database Engine] Đang kết nối CSDL PostgreSQL tại cổng 5432...");
            await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync(context.Database);
            await ExpenseTracker.Infrastructure.Persistence.DatabaseSeeder.SeedDefaultCategoriesAsync(context, logger);
            await ExpenseTracker.Infrastructure.Persistence.DatabaseSeeder.SeedAdminUserAsync(context, passwordHasher, logger);
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning("⚠️ [Database Warning] Lỗi khởi tạo CSDL: {Message}", ex.Message);
    }
}

app.Run();
