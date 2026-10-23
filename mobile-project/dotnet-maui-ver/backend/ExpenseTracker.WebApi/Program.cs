using System.Text;
using ExpenseTracker.Application;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.WebApi.Hubs;
using ExpenseTracker.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Register Clean Architecture layer dependencies
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register API controllers and documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Seed initial system financial categories in database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ExpenseTracker.Infrastructure.Persistence.ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await ExpenseTracker.Infrastructure.Persistence.DatabaseSeeder.SeedDefaultCategoriesAsync(context, logger);
}

app.Run();
