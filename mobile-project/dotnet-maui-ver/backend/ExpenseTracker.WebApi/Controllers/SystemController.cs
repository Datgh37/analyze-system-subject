using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[Tags("Hệ Thống & Quản Trị CSDL (Dev)")]
public class SystemController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<SystemController> _logger;

    public SystemController(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<SystemController> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Kiểm tra trạng thái dữ liệu các bảng chính trong hệ thống.
    /// </summary>
    [HttpGet("database-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDatabaseStatus(CancellationToken ct)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(ct);
            if (!canConnect)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    Status = "Offline",
                    DatabaseConnected = false,
                    Message = "Không thể kết nối đến PostgreSQL container. Hãy bật Docker Desktop và chạy 'docker compose up -d'.",
                    Timestamp = DateTime.UtcNow
                });
            }

            var usersCount = await _context.Users.CountAsync(ct);
            var walletsCount = await _context.Wallets.CountAsync(ct);
            var categoriesCount = await _context.Categories.CountAsync(ct);
            var transactionsCount = await _context.Transactions.CountAsync(ct);
            var budgetsCount = await _context.Budgets.CountAsync(ct);
            var adminExists = await _context.Users.AnyAsync(u => u.Email == DatabaseSeeder.DefaultAdminEmail, ct);

            return Ok(new
            {
                Status = "Online",
                DatabaseConnected = true,
                AdminExists = adminExists,
                AdminEmail = DatabaseSeeder.DefaultAdminEmail,
                Statistics = new
                {
                    Users = usersCount,
                    Wallets = walletsCount,
                    Categories = categoriesCount,
                    Transactions = transactionsCount,
                    Budgets = budgetsCount
                },
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi kiểm tra trạng thái CSDL trong GetDatabaseStatus.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Status = "Error",
                DatabaseConnected = false,
                Message = "Lỗi khi truy vấn CSDL: " + ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Seed tài khoản Quản trị viên (Admin) mặc định nếu chưa tồn tại.
    /// </summary>
    [HttpPost("seed-admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SeedAdmin(CancellationToken ct)
    {
        var admin = await DatabaseSeeder.SeedAdminUserAsync(_context, _passwordHasher, _logger, ct);

        return Ok(new
        {
            Success = true,
            Message = "Khởi tạo tài khoản Quản trị viên (Admin) thành công.",
            Credentials = new
            {
                Email = DatabaseSeeder.DefaultAdminEmail,
                Password = DatabaseSeeder.DefaultAdminPassword,
                FullName = admin.FullName,
                BaseCurrency = admin.BaseCurrency
            }
        });
    }

    /// <summary>
    /// Xóa toàn bộ dữ liệu phát sinh và khôi phục CSDL về trạng thái Seed ban đầu (Gồm Categories chuẩn, Admin user, 2 ví, giao dịch và ngân sách mẫu).
    /// </summary>
    [HttpPost("reset-database")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetDatabase(CancellationToken ct)
    {
        try
        {
            await DatabaseSeeder.ResetDatabaseAsync(_context, _passwordHasher, _logger, ct);

            return Ok(new
            {
                Success = true,
                Message = "CSDL đã được làm mới hoàn toàn (Truncate & Re-seed) về trạng thái chuẩn ban đầu.",
                AdminCredentials = new
                {
                    Email = DatabaseSeeder.DefaultAdminEmail,
                    Password = DatabaseSeeder.DefaultAdminPassword,
                    Note = "Bạn có thể sử dụng thông tin này để đăng nhập ngay trên ứng dụng Mobile hoặc qua API /api/auth/login."
                },
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi xảy ra khi thực hiện Reset Database.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Database Reset Error",
                Detail = ex.Message
            });
        }
    }
}
