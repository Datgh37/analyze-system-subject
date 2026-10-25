using System.Text;
using ExpenseTracker.Application;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.WebApi.Hubs;
using ExpenseTracker.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load optional local configuration file (ignored by Git for secret keys)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Register Clean Architecture layer dependencies
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register API controllers and documentation with JWT Bearer authorization in Swagger
builder.Services.AddControllers();
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
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Nhập JWT token của bạn vào đây (Swagger sẽ tự động gắn tiền tố 'Bearer ')."
    });

    options.AddSecurityRequirement(_ => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer"),
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

var app = builder.Build();

// Register global exception handler middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

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

        // Tự động áp dụng toàn bộ Migrations mới nhất khi phát hiện kết nối PostgreSQL
        await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync(context.Database);

        await ExpenseTracker.Infrastructure.Persistence.DatabaseSeeder.SeedDefaultCategoriesAsync(context, logger);
        await ExpenseTracker.Infrastructure.Persistence.DatabaseSeeder.SeedAdminUserAsync(context, passwordHasher, logger);
    }
    catch (Exception ex)
    {
        logger.LogWarning("⚠️ [PostgreSQL Warning] Không thể kết nối tới CSDL tại cổng 5432 ({Message}). Hãy bật Docker Desktop ('docker compose up -d') khi cần lưu dữ liệu vào CSDL.", ex.Message);
    }
}

app.Run();
