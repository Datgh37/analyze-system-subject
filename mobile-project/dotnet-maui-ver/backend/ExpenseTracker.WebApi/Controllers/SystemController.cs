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

    /// <summary>
    /// Nạp bộ dữ liệu kịch bản Demo hoàn hảo (Master Demo Story) cho tài khoản demo@expensetracker.vn.
    /// </summary>
    [HttpPost("seed-demo-story")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedDemoStory(CancellationToken ct)
    {
        try
        {
            var demoUser = await DatabaseSeeder.SeedDemoStoryAsync(_context, _passwordHasher, _logger, ct);

            return Ok(new
            {
                Success = true,
                Message = "Nạp bộ dữ liệu kể chuyện Demo thành công.",
                Credentials = new
                {
                    Email = DatabaseSeeder.DefaultDemoEmail,
                    Password = DatabaseSeeder.DefaultDemoPassword,
                    FullName = demoUser.FullName,
                    BaseCurrency = demoUser.BaseCurrency
                },
                Summary = new
                {
                    Wallets = new[]
                    {
                        new { Name = "Tài khoản Vietcombank", Balance = "15.500.000 đ", Type = "Ngân hàng" },
                        new { Name = "Ví Tiền Mặt", Balance = "1.200.000 đ", Type = "Tiền mặt" },
                        new { Name = "Quỹ Khẩn Cấp (Techcombank)", Balance = "10.000.000 đ", Type = "Quỹ dự phòng" }
                    },
                    TotalNetWorth = "26.700.000 đ",
                    BudgetAlert = "Ăn uống & Thực phẩm: Đã chi 2.550.000 đ / 3.000.000 đ (85.0% - Cảnh báo cam)",
                    CoolingOff = "Tai nghe chống ồn Sony: 4.500.000 đ (còn 18 giờ đếm ngược)",
                    SavingsGoals = new[]
                    {
                        new { Name = "Mua Laptop Mới", Progress = "12.000.000 đ / 25.000.000 đ (48.0%)" },
                        new { Name = "Quỹ Khẩn Cấp 3 Tháng", Progress = "10.000.000 đ / 20.000.000 đ (50.0%)" }
                    },
                    ReceiptAudit = "Hóa đơn siêu thị WinMart: 4 món hàng bóc tách (385.000 đ)",
                    PayslipAudit = "Phiếu lương FPT: Lương gộp 22.000.000 đ, Thực nhận 22.690.000 đ (Khớp 100%)"
                },
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi xảy ra khi nạp kịch bản Demo Story.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Demo Story Seeding Error",
                Detail = ex.Message
            });
        }
    }
}
