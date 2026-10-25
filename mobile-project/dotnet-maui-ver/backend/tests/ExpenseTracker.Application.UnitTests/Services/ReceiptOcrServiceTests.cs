using System.Text;
using ExpenseTracker.Application.DTOs.AutoRules;
using ExpenseTracker.Application.DTOs.Receipts;
using ExpenseTracker.Application.DTOs.Transactions;
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

// Unit tests validating ReceiptOcrService, deterministic math audit, and auto-learning rules (UC-03.07/03.08)
public class ReceiptOcrServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AutoCategorizationService _autoRuleService;
    private readonly TransactionService _transactionService;
    private readonly ReceiptOcrService _receiptOcrService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _foodCategory;

    public ReceiptOcrServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _autoRuleService = new AutoCategorizationService(_dbContext);
        _transactionService = new TransactionService(_dbContext);
        _receiptOcrService = new ReceiptOcrService(_dbContext, _autoRuleService, _transactionService);

        _wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 1_000_000m);
        _dbContext.Wallets.Add(_wallet);

        _foodCategory = new Category("Ăn uống siêu thị", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(_foodCategory);

        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task ParseReceiptAsync_WithValidReceipt_ShouldCalculateCorrectMath_NoDiscrepancy()
    {
        // Arrange: Valid JSON payload where Items (100k) - Discount (10k) + Tax (8k) = Total (98k)
        var json = """
        {
            "merchantName": "Siêu thị Co.opmart",
            "items": [
                { "name": "Bánh quy AFC", "quantity": 2, "unitPrice": 25000, "totalPrice": 50000 },
                { "name": "Sữa tươi chua", "quantity": 2, "unitPrice": 25000, "totalPrice": 50000 }
            ],
            "discountAmount": 10000,
            "taxAmount": 8000,
            "totalAmount": 98000,
            "confidence": 0.98
        }
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _receiptOcrService.ParseReceiptAsync(_userId, stream, "hoadon.jpg");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.MerchantName.Should().Be("Siêu thị Co.opmart");
        result.Value.SubTotal.Should().Be(100_000m);
        result.Value.CalculatedTotal.Should().Be(98_000m);
        result.Value.TotalAmount.Should().Be(98_000m);
        result.Value.DiscrepancyAmount.Should().Be(0m);
        result.Value.IsMathDiscrepancy.Should().BeFalse();
        result.Value.AuditWarningMessage.Should().BeNull();
        result.Value.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task ParseReceiptAsync_WithCorruptedReceipt_ShouldDetectMathDiscrepancy()
    {
        // Arrange: Corrupted receipt where OCR read total as 150k but items calculate to 98k (Δ = 52k > 1k)
        var json = """
        {
            "merchantName": "WinMart",
            "items": [
                { "name": "Bánh mì", "quantity": 1, "unitPrice": 50000, "totalPrice": 50000 }
            ],
            "discountAmount": 0,
            "taxAmount": 0,
            "totalAmount": 150000
        }
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _receiptOcrService.ParseReceiptAsync(_userId, stream, "receipt.png");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.IsMathDiscrepancy.Should().BeTrue();
        result.Value.DiscrepancyAmount.Should().Be(100_000m);
        result.Value.AuditWarningMessage.Should().NotBeNull();
        result.Value.AuditWarningMessage.Should().Contain("Phát hiện sai lệch số học");
    }

    [Fact]
    public async Task ParseReceiptAsync_ShouldAutoMatchItemCategoryViaRules()
    {
        // Arrange: Add smart rule: "Sữa" -> Ăn uống siêu thị
        var rule = new AutoCategorizationRule(_userId, "Sữa", _foodCategory.Id, priorityOrder: 1);
        await _dbContext.AutoCategorizationRules.AddAsync(rule);
        await _dbContext.SaveChangesAsync();

        var json = """
        {
            "merchantName": "Bách Hóa Xanh",
            "items": [
                { "name": "Sữa bột trẻ em", "quantity": 1, "unitPrice": 200000, "totalPrice": 200000 }
            ],
            "totalAmount": 200000
        }
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _receiptOcrService.ParseReceiptAsync(_userId, stream, "receipt.pdf");

        // Assert
        result.IsSuccess.Should().BeTrue();
        var item = result.Value!.Items.First();
        item.SuggestedCategoryId.Should().Be(_foodCategory.Id);
        item.SuggestedCategoryName.Should().Be("Ăn uống siêu thị");
    }

    [Fact]
    public async Task ConfirmReceiptAsync_ShouldDebitWalletAndCreateTransactionWithItems()
    {
        // Arrange
        var items = new List<CreateReceiptItemRequest>
        {
            new("Thịt heo sạch", 1, 120000m, 120000m, _foodCategory.Id),
            new("Rau xà lách", 1, 30000m, 30000m, _foodCategory.Id)
        };

        var request = new ConfirmReceiptRequest(
            WalletId: _wallet.Id,
            CategoryId: _foodCategory.Id,
            Amount: 150_000m,
            TransactionDate: DateTime.UtcNow,
            MerchantName: "WinMart",
            Items: items,
            RememberRules: false
        );

        // Act
        var result = await _receiptOcrService.ConfirmReceiptAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(150_000m);

        var updatedWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        updatedWallet.Balance.Should().Be(850_000m); // 1M - 150k

        var savedItems = await _dbContext.ReceiptItems
            .Where(r => r.TransactionId == result.Value.Id)
            .ToListAsync();
        savedItems.Should().HaveCount(2);
    }

    [Fact]
    public async Task ConfirmReceiptAsync_WhenRememberRulesTrue_ShouldCreateAutoRulesForItems()
    {
        // Arrange: Confirm receipt with RememberRules = true
        var items = new List<CreateReceiptItemRequest>
        {
            new("Dầu gội Clear", 1, 85000m, 85000m, _foodCategory.Id)
        };

        var request = new ConfirmReceiptRequest(
            WalletId: _wallet.Id,
            CategoryId: _foodCategory.Id,
            Amount: 85_000m,
            TransactionDate: DateTime.UtcNow,
            Items: items,
            RememberRules: true
        );

        // Act
        var result = await _receiptOcrService.ConfirmReceiptAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var autoRule = await _dbContext.AutoCategorizationRules
            .FirstOrDefaultAsync(r => r.UserId == _userId && r.KeywordPattern == "Dầu gội Clear");
        autoRule.Should().NotBeNull();
        autoRule!.TargetCategoryId.Should().Be(_foodCategory.Id);
    }

    [Fact]
    public async Task ParseReceiptAsync_WithEvnElectricityBill_ShouldAssignUtilityCategory()
    {
        // Arrange: Utility category & EVN bill JSON payload
        var utilityCategory = new Category("Hóa đơn & Tiện ích", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(utilityCategory);
        await _dbContext.SaveChangesAsync();

        var json = """
        {
            "merchantName": "Tổng Công ty Điện lực EVN TP.HCM",
            "items": [
                { "name": "Tiền điện sinh hoạt kỳ 10/2026 (Sản lượng: 250 kWh)", "quantity": 1, "unitPrice": 450000, "totalPrice": 450000 }
            ],
            "discountAmount": 0,
            "taxAmount": 36000,
            "totalAmount": 486000,
            "confidence": 0.98
        }
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _receiptOcrService.ParseReceiptAsync(_userId, stream, "evn_invoice.pdf");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MerchantName.Should().Contain("EVN");
        result.Value.TotalAmount.Should().Be(486_000m);
        result.Value.Items[0].SuggestedCategoryId.Should().Be(utilityCategory.Id);
    }

    [Fact]
    public async Task ParseReceiptAsync_WithGeminiVisionResponse_ShouldExtractMerchantAndItemsAccurately()
    {
        // Arrange
        var geminiMock = new Mock<IGeminiApiClient>();
        var geminiResponseJson = """
        {
            "merchantName": "Highlands Coffee - Landmark 81",
            "invoiceDate": "2026-10-25T08:30:00Z",
            "taxAmount": 8000,
            "discountAmount": 0,
            "totalAmount": 88000,
            "confidence": 0.99,
            "items": [
                { "name": "Phin Sữa Đá Cỡ Lớn", "quantity": 1, "unitPrice": 45000, "totalPrice": 45000 },
                { "name": "Bánh Mì Gà Xé Nước Tương", "quantity": 1, "unitPrice": 35000, "totalPrice": 35000 }
            ]
        }
        """;

        geminiMock.Setup(g => g.GenerateContentWithImageAsync(
            It.IsAny<byte[]>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(geminiResponseJson));

        var service = new ReceiptOcrService(
            _dbContext,
            _autoRuleService,
            _transactionService,
            imagePreProcessor: null,
            geminiClient: geminiMock.Object);

        // Binary JPEG header bytes to trigger real image flow
        var fakeJpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        using var stream = new MemoryStream(fakeJpegBytes);

        // Act
        var result = await service.ParseReceiptAsync(_userId, stream, "highlands_receipt.jpg");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MerchantName.Should().Be("Highlands Coffee - Landmark 81");
        result.Value.TotalAmount.Should().Be(88_000m);
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items[0].ItemName.Should().Be("Phin Sữa Đá Cỡ Lớn");
        result.Value.Items[1].ItemName.Should().Be("Bánh Mì Gà Xé Nước Tương");
        result.Value.IsMathDiscrepancy.Should().BeFalse();
        geminiMock.Verify(g => g.GenerateContentWithImageAsync(
            It.IsAny<byte[]>(),
            "image/jpeg",
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
