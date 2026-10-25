using System.Text;
using System.Text.Json;
using ExpenseTracker.Application.DTOs.Payslips;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating AI Vision OCR payslip extraction, mathematical audit, and ACID ledger sync (UC-02.08)
public class PayslipOcrServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly PayslipOcrService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _incomeCategory;
    private readonly Category _expenseCategory;

    public PayslipOcrServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new PayslipOcrService(_dbContext);

        _wallet = new Wallet(_userId, "Ví Lương Vietcombank", WalletType.BankAccount, 10_000_000m);
        _incomeCategory = new Category("Tiền lương", TransactionType.Income, isSystem: true);
        _expenseCategory = new Category("Ăn uống", TransactionType.Expense, isSystem: true);

        _dbContext.Wallets.Add(_wallet);
        _dbContext.Categories.AddRange(_incomeCategory, _expenseCategory);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task ParsePayslipAsync_WithInvalidExtension_ShouldReturnFailure()
    {
        // Arrange
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake file content"));

        // Act: Upload .docx file which is not allowed (UC-02.08 Step 2)
        var result = await _service.ParsePayslipAsync(stream, "payslip.docx");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Định dạng tệp không được hỗ trợ");
    }

    [Fact]
    public async Task ParsePayslipAsync_WithEmptyStream_ShouldReturnFailure()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var result = await _service.ParsePayslipAsync(stream, "payslip.pdf");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Tệp tải lên rỗng");
    }

    [Fact]
    public async Task ParsePayslipAsync_WithBalancedSalaryData_ShouldPassMathAudit()
    {
        // Arrange: Custom payload simulating OCR recognition
        var payload = new
        {
            employerName = "Tập đoàn Vingroup",
            payPeriod = "10/2026",
            grossSalary = 30_000_000m,
            netSalary = 25_500_000m,
            socialInsurance = 2_400_000m,
            healthInsurance = 450_000m,
            unemploymentInsurance = 300_000m,
            personalIncomeTax = 1_350_000m,
            allowances = 0m,
            bonus = 0m,
            otherDeductions = 0m
        };
        var json = JsonSerializer.Serialize(payload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _service.ParsePayslipAsync(stream, "phieu_luong_t10.pdf");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.EmployerName.Should().Be("Tập đoàn Vingroup");
        result.Value.GrossSalary.Should().Be(30_000_000m);
        result.Value.NetSalary.Should().Be(25_500_000m);
        result.Value.IsMathAudited.Should().BeTrue();
        result.Value.HasDiscrepancy.Should().BeFalse();
        result.Value.DiscrepancyAmount.Should().Be(0m);
    }

    [Fact]
    public async Task ParsePayslipAsync_WhenSalaryHasDiscrepancy_ShouldFlagWarning()
    {
        // Arrange: Net does not match formula (UC-02.08 E2)
        var payload = new
        {
            employerName = "Công ty ABC",
            payPeriod = "10/2026",
            grossSalary = 20_000_000m,
            netSalary = 15_000_000m, // Expected 18M, discrepancy = 3,000,000 VND
            socialInsurance = 1_600_000m,
            healthInsurance = 300_000m,
            unemploymentInsurance = 200_000m,
            personalIncomeTax = 0m,
            allowances = 0m,
            bonus = 0m,
            otherDeductions = 0m
        };
        var json = JsonSerializer.Serialize(payload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _service.ParsePayslipAsync(stream, "phieu_luong.png");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.IsMathAudited.Should().BeFalse();
        result.Value.HasDiscrepancy.Should().BeTrue();
        result.Value.DiscrepancyAmount.Should().Be(2_900_000m); // 20M - 2.1M = 17.9M. |15M - 17.9M| = 2.9M
    }

    [Fact]
    public async Task ConfirmAndSavePayslipAsync_WithValidRequest_ShouldCreateTransactionAndCreditWalletAndSaveSlip()
    {
        // Arrange: Wallet initial balance is 10,000,000
        var request = new ConfirmPayslipRequest(
            WalletId: _wallet.Id,
            CategoryId: _incomeCategory.Id,
            EmployerName: "Shopee Vietnam",
            PayPeriod: "10/2026",
            GrossSalary: 25_000_000m,
            NetSalary: 21_225_000m,
            SocialInsurance: 2_000_000m,
            HealthInsurance: 375_000m,
            UnemploymentInsurance: 250_000m,
            PersonalIncomeTax: 1_150_000m
        );

        // Act
        var result = await _service.ConfirmAndSavePayslipAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.EmployerName.Should().Be("Shopee Vietnam");
        result.Value.NetSalary.Should().Be(21_225_000m);
        result.Value.IsMathAudited.Should().BeTrue();

        // 1. Verify wallet is credited: 10M + 21,225,000 = 31,225,000 VND
        var updatedWallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        updatedWallet!.Balance.Should().Be(31_225_000m);

        // 2. Verify transaction is recorded in ledger
        var tx = await _dbContext.Transactions.FindAsync(result.Value.TransactionId);
        tx.Should().NotBeNull();
        tx!.Amount.Should().Be(21_225_000m);
        tx.TransactionType.Should().Be(TransactionType.Income);

        // 3. Verify salary slip detail is stored in database
        var slip = await _dbContext.SalarySlipDetails.FindAsync(result.Value.Id);
        slip.Should().NotBeNull();
        slip!.GrossSalary.Should().Be(25_000_000m);
        slip.TransactionId.Should().Be(tx.Id);
    }

    [Fact]
    public async Task ConfirmAndSavePayslipAsync_WithUpdateRecurringIncome_ShouldUpdateRecurringScheduleSalary()
    {
        // Arrange: User has existing recurring income with expectedAmount = 18,000,000
        var recurringIncome = new RecurringIncome(
            _userId,
            "Lương công ty",
            18_000_000m,
            _wallet.Id,
            _incomeCategory.Id,
            RecurringFrequency.Monthly,
            dayOfMonth: 5
        );
        await _dbContext.RecurringIncomes.AddAsync(recurringIncome);
        await _dbContext.SaveChangesAsync();

        var request = new ConfirmPayslipRequest(
            WalletId: _wallet.Id,
            CategoryId: _incomeCategory.Id,
            EmployerName: "Shopee Vietnam",
            PayPeriod: "11/2026",
            GrossSalary: 28_000_000m,
            NetSalary: 24_000_000m,
            SocialInsurance: 2_240_000m,
            HealthInsurance: 420_000m,
            UnemploymentInsurance: 280_000m,
            PersonalIncomeTax: 1_060_000m,
            UpdateRecurringIncomeId: recurringIncome.Id // UC-02.08 A1
        );

        // Act
        var result = await _service.ConfirmAndSavePayslipAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify recurring income expected amount is synchronized to new Net salary (24,000,000 VND)
        var updatedSchedule = await _dbContext.RecurringIncomes.FindAsync(recurringIncome.Id);
        updatedSchedule!.ExpectedAmount.Should().Be(24_000_000m);
    }

    [Fact]
    public async Task ConfirmAndSavePayslipAsync_WithInvalidWallet_ShouldReturnFailure()
    {
        // Arrange
        var request = new ConfirmPayslipRequest(
            WalletId: Guid.NewGuid(),
            CategoryId: _incomeCategory.Id,
            EmployerName: "ABC",
            PayPeriod: "10/2026",
            GrossSalary: 10_000_000m,
            NetSalary: 9_000_000m,
            SocialInsurance: 800_000m,
            HealthInsurance: 150_000m,
            UnemploymentInsurance: 100_000m,
            PersonalIncomeTax: 0m
        );

        // Act
        var result = await _service.ConfirmAndSavePayslipAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Ví nhận tiền lương không tồn tại");
    }

    [Fact]
    public async Task ConfirmAndSavePayslipAsync_WithExpenseCategory_ShouldReturnFailure()
    {
        // Arrange
        var request = new ConfirmPayslipRequest(
            WalletId: _wallet.Id,
            CategoryId: _expenseCategory.Id, // Expense category
            EmployerName: "ABC",
            PayPeriod: "10/2026",
            GrossSalary: 10_000_000m,
            NetSalary: 9_000_000m,
            SocialInsurance: 800_000m,
            HealthInsurance: 150_000m,
            UnemploymentInsurance: 100_000m,
            PersonalIncomeTax: 0m
        );

        // Act
        var result = await _service.ConfirmAndSavePayslipAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Danh mục thu nhập đã chọn không hợp lệ");
    }

    [Fact]
    public async Task GetSalarySlipDetailByTransactionIdAsync_WhenExists_ShouldReturnDto()
    {
        // Arrange: Create a confirmed payslip
        var request = new ConfirmPayslipRequest(
            WalletId: _wallet.Id,
            CategoryId: _incomeCategory.Id,
            EmployerName: "Công ty Công nghệ ABC",
            PayPeriod: "10/2026",
            GrossSalary: 25_000_000m,
            NetSalary: 21_225_000m,
            SocialInsurance: 2_000_000m,
            HealthInsurance: 375_000m,
            UnemploymentInsurance: 250_000m,
            PersonalIncomeTax: 1_150_000m
        );
        var created = await _service.ConfirmAndSavePayslipAsync(_userId, request);

        // Act
        var result = await _service.GetSalarySlipDetailByTransactionIdAsync(_userId, created.Value!.TransactionId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.EmployerName.Should().Be("Công ty Công nghệ ABC");
        result.Value.NetSalary.Should().Be(21_225_000m);
    }

    [Fact]
    public async Task ParsePayslipAsync_WithGeminiVisionResponse_ShouldExtractSalaryFieldsAccurately()
    {
        // Arrange
        var geminiMock = new Mock<IGeminiApiClient>();
        var geminiResponseJson = """
        {
            "employerName": "VNG Corporation",
            "payPeriod": "10/2026",
            "grossSalary": 35000000,
            "netSalary": 29875000,
            "socialInsurance": 2800000,
            "healthInsurance": 525000,
            "unemploymentInsurance": 350000,
            "personalIncomeTax": 1450000,
            "allowances": 0,
            "bonus": 0,
            "otherDeductions": 0,
            "confidence": 0.98
        }
        """;

        geminiMock.Setup(g => g.GenerateContentWithImageAsync(
            It.IsAny<byte[]>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(geminiResponseJson));

        var service = new PayslipOcrService(_dbContext, geminiMock.Object);

        var fakeJpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        using var stream = new MemoryStream(fakeJpegBytes);

        // Act
        var result = await service.ParsePayslipAsync(stream, "vng_payslip.jpg");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EmployerName.Should().Be("VNG Corporation");
        result.Value.PayPeriod.Should().Be("10/2026");
        result.Value.GrossSalary.Should().Be(35_000_000m);
        result.Value.NetSalary.Should().Be(29_875_000m);
        result.Value.SocialInsurance.Should().Be(2_800_000m);
        result.Value.HealthInsurance.Should().Be(525_000m);
        result.Value.UnemploymentInsurance.Should().Be(350_000m);
        result.Value.PersonalIncomeTax.Should().Be(1_450_000m);
        result.Value.IsMathAudited.Should().BeTrue();
        geminiMock.Verify(g => g.GenerateContentWithImageAsync(
            It.IsAny<byte[]>(),
            "image/jpeg",
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
