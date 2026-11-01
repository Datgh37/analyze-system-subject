using System.Collections.ObjectModel;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Models.Receipts;
using ExpenseTracker.Maui.Models.Transactions;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

// Unit tests for ReceiptOcrViewModel verifying camera native preview, in-place math audit, and ledger commitment (UC-03.07/03.08)
public class ReceiptOcrViewModelTests
{
    private readonly Mock<IReceiptRepository> _mockReceiptRepo;
    private readonly Mock<IWalletRepository> _mockWalletRepo;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Mock<ISmartUtilityRepository> _mockSmartUtilityRepo;
    private readonly ReceiptOcrViewModel _viewModel;

    public ReceiptOcrViewModelTests()
    {
        _mockReceiptRepo = new Mock<IReceiptRepository>();
        _mockWalletRepo = new Mock<IWalletRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockSmartUtilityRepo = new Mock<ISmartUtilityRepository>();

        _viewModel = new ReceiptOcrViewModel(
            _mockReceiptRepo.Object,
            _mockWalletRepo.Object,
            _mockCategoryRepo.Object,
            _mockSmartUtilityRepo.Object
        );
    }

    [Fact]
    public async Task LoadReferenceDataAsync_ShouldPopulateWalletsAndCategories()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var catId = Guid.NewGuid();
        var wallets = new List<WalletModel>
        {
            new(walletId, Guid.NewGuid(), "Ví Tiền Mặt", MobileWalletType.Cash, 2_000_000m, 2_000_000m, "VND", null, null, null, null, null, "#10B981", "wallet", 1, false, 1, 1, DateTime.UtcNow, null)
        };
        var categories = new List<CategoryModel>
        {
            new(catId, Guid.NewGuid(), "Ăn uống", 2, null, null, "food", "#EF4444", 1, true, null, 1, null)
        };

        _mockWalletRepo.Setup(r => r.GetWalletsAsync()).ReturnsAsync(wallets);
        _mockCategoryRepo.Setup(r => r.GetCategoriesAsync(2)).ReturnsAsync(categories);

        // Act
        await _viewModel.LoadReferenceDataAsync();

        // Assert
        _viewModel.Wallets.Should().HaveCount(1);
        _viewModel.Wallets[0].Name.Should().Be("Ví Tiền Mặt");
        _viewModel.SelectedWallet.Should().Be(_viewModel.Wallets[0]);

        _viewModel.ExpenseCategories.Should().HaveCount(1);
        _viewModel.ExpenseCategories[0].Name.Should().Be("Ăn uống");
        _viewModel.SelectedCategory.Should().Be(_viewModel.ExpenseCategories[0]);
    }

    [Fact]
    public void RecalculateTotalsAndAudit_WhenItemsMatchTotal_ShouldHaveNoDiscrepancy()
    {
        // Arrange
        _viewModel.Items.Clear();
        _viewModel.Items.Add(new ParsedReceiptItemModel { ItemName = "Sữa chua Vinamilk", Quantity = 2, UnitPrice = 15000, TotalPrice = 30000 });
        _viewModel.Items.Add(new ParsedReceiptItemModel { ItemName = "Bánh mì sandwich", Quantity = 1, UnitPrice = 25000, TotalPrice = 25000 });
        _viewModel.DiscountAmount = 5000m;
        _viewModel.TaxAmount = 4000m;
        _viewModel.TotalAmount = 54000m; // 30k + 25k - 5k + 4k = 54k

        // Act
        _viewModel.RecalculateTotalsAndAudit();

        // Assert
        _viewModel.SubTotal.Should().Be(55_000m);
        _viewModel.CalculatedTotal.Should().Be(54_000m);
        _viewModel.DiscrepancyAmount.Should().Be(0m);
        _viewModel.IsMathDiscrepancy.Should().BeFalse();
        _viewModel.AuditWarningMessage.Should().BeNull();
    }

    [Fact]
    public void RecalculateTotalsAndAudit_WhenItemsDifferFromTotal_ShouldDetectMathDiscrepancy()
    {
        // Arrange: Sum of items = 50k, but receipt total was OCR'd as 120k (Δ = 70k > 1k)
        _viewModel.Items.Clear();
        _viewModel.Items.Add(new ParsedReceiptItemModel { ItemName = "Nước ngọt Coca", Quantity = 5, UnitPrice = 10000, TotalPrice = 50000 });
        _viewModel.DiscountAmount = 0m;
        _viewModel.TaxAmount = 0m;
        _viewModel.TotalAmount = 120000m;

        // Act
        _viewModel.RecalculateTotalsAndAudit();

        // Assert
        _viewModel.SubTotal.Should().Be(50_000m);
        _viewModel.CalculatedTotal.Should().Be(50_000m);
        _viewModel.DiscrepancyAmount.Should().Be(70_000m);
        _viewModel.IsMathDiscrepancy.Should().BeTrue();
        _viewModel.AuditWarningMessage.Should().NotBeNull();
        _viewModel.AuditWarningMessage.Should().Contain("Phát hiện sai lệch số học");
    }

    [Fact]
    public void RemoveItem_ShouldUpdateTotalsAndAuditImmediately()
    {
        // Arrange
        var item1 = new ParsedReceiptItemModel { ItemName = "Món 1", Quantity = 1, UnitPrice = 20000, TotalPrice = 20000 };
        var item2 = new ParsedReceiptItemModel { ItemName = "Món 2", Quantity = 1, UnitPrice = 30000, TotalPrice = 30000 };
        _viewModel.Items.Clear();
        _viewModel.Items.Add(item1);
        _viewModel.Items.Add(item2);
        _viewModel.TotalAmount = 20000m; // Khớp nếu bỏ Món 2

        // Act
        _viewModel.RemoveItem(item2);

        // Assert
        _viewModel.Items.Should().HaveCount(1);
        _viewModel.Items[0].ItemName.Should().Be("Món 1");
        _viewModel.SubTotal.Should().Be(20_000m);
        _viewModel.CalculatedTotal.Should().Be(20_000m);
        _viewModel.IsMathDiscrepancy.Should().BeFalse();
    }

    [Fact]
    public void AddNewItem_ShouldAppendItemAndRecalculate()
    {
        // Arrange
        _viewModel.Items.Clear();
        _viewModel.TotalAmount = 50000m;

        // Act
        _viewModel.AddNewItem();

        // Assert
        _viewModel.Items.Should().HaveCount(1);
        _viewModel.Items[0].ItemName.Should().Be("Món hàng bổ sung");
        _viewModel.Items[0].Quantity.Should().Be(1);
        _viewModel.Items[0].UnitPrice.Should().Be(0);
    }

    [Fact]
    public async Task ConfirmReceiptAsync_WhenValid_ShouldCommitAndResetParsedData()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var catId = Guid.NewGuid();
        var wallet = new WalletModel(walletId, Guid.NewGuid(), "Ví Chính", MobileWalletType.Cash, 1_000_000m, 1_000_000m, "VND", null, null, null, null, null, "#10B981", "wallet", 1, false, 1, 1, DateTime.UtcNow, null);
        var category = new CategoryModel(catId, Guid.NewGuid(), "Ăn uống", 2, null, null, "food", "#EF4444", 1, true, null, 1, null);

        _viewModel.SelectedWallet = wallet;
        _viewModel.SelectedCategory = category;
        _viewModel.MerchantName = "WinMart Ba Tháng Hai";
        _viewModel.TotalAmount = 85000m;
        _viewModel.HasParsedData = true;
        _viewModel.Items.Clear();
        _viewModel.Items.Add(new ParsedReceiptItemModel { ItemName = "Trứng gà Ba Huân", Quantity = 1, UnitPrice = 35000, TotalPrice = 35000 });
        _viewModel.Items.Add(new ParsedReceiptItemModel { ItemName = "Sữa chua nếp cẩm", Quantity = 2, UnitPrice = 25000, TotalPrice = 50000 });

        var createdTx = new TransactionModel(
            Guid.NewGuid(),
            Guid.NewGuid(),
            walletId,
            "Ví Chính",
            catId,
            "Ăn uống",
            "food",
            "#EF4444",
            85000m,
            2,
            DateTime.Today,
            "WinMart Ba Tháng Hai",
            true,
            false,
            false,
            false,
            DateTime.UtcNow
        );

        _mockReceiptRepo.Setup(r => r.ConfirmReceiptAsync(It.IsAny<ConfirmReceiptModel>()))
            .ReturnsAsync(createdTx);

        // Act
        await _viewModel.ConfirmReceiptAsync();

        // Assert
        _mockReceiptRepo.Verify(r => r.ConfirmReceiptAsync(It.Is<ConfirmReceiptModel>(c =>
            c.MerchantName == "WinMart Ba Tháng Hai" &&
            c.Amount == 85000m &&
            c.WalletId == walletId &&
            c.Items != null &&
            c.Items.Count == 2)), Times.Once);

        _viewModel.HasParsedData.Should().BeFalse();
        _viewModel.MerchantName.Should().BeEmpty();
        _viewModel.Items.Should().BeEmpty();
    }
}
