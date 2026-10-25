using ExpenseTracker.Maui.Models.ImportExport;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Repositories.ImportExport;
using ExpenseTracker.Maui.ViewModels.ImportExport;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

// Unit tests for ImportPreviewViewModel (UC-07.06: Hybrid Import HITL Preview)
public class ImportPreviewViewModelTests
{
    private readonly Mock<IDataImportRepository> _mockImportRepo;
    private readonly Mock<IWalletRepository> _mockWalletRepo;
    private readonly ImportPreviewViewModel _vm;

    public ImportPreviewViewModelTests()
    {
        _mockImportRepo = new Mock<IDataImportRepository>();
        _mockWalletRepo = new Mock<IWalletRepository>();
        _vm = new ImportPreviewViewModel(_mockImportRepo.Object, _mockWalletRepo.Object);
    }

    [Fact]
    public async Task InitializeAsync_ShouldLoadWalletsAndSelectFirst()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var wallets = new List<WalletModel>
        {
            new(walletId, Guid.NewGuid(), "Ví Tiền Mặt", MobileWalletType.Cash, 1000000m, 1000000m, "VND", null, null, null, null, null, "#000", "wallet", 1, false, 1, 1, DateTime.UtcNow, null)
        };
        _mockWalletRepo.Setup(r => r.GetWalletsAsync()).ReturnsAsync(wallets);

        // Act
        await _vm.InitializeAsync();

        // Assert
        _vm.Wallets.Should().HaveCount(1);
        _vm.SelectedWallet.Should().NotBeNull();
        _vm.SelectedWallet!.Id.Should().Be(walletId);
    }

    [Fact]
    public async Task LoadSpreadsheetPreviewAsync_ShouldPopulateRowsAndTotals()
    {
        // Arrange
        var previewResult = new ImportPreviewResultModel
        {
            SourceType = "StructuredExcel",
            TotalRows = 2,
            ValidRows = 2,
            InvalidRows = 0,
            TotalAmount = 95000m,
            SummaryMessage = "Bóc tách 2 dòng thành công",
            Rows = new List<ParsedImportRowModel>
            {
                new() { RowIndex = 1, Amount = 45000m, CategoryName = "Ăn uống", IsValid = true, IsSelected = true },
                new() { RowIndex = 2, Amount = 50000m, CategoryName = "Đi lại", IsValid = true, IsSelected = true }
            }
        };

        using var ms = new MemoryStream();
        _mockImportRepo.Setup(r => r.PreviewSpreadsheetAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                       .ReturnsAsync(previewResult);

        // Act
        await _vm.LoadSpreadsheetPreviewAsync((ms, "test.xlsx"));

        // Assert
        _vm.Rows.Should().HaveCount(2);
        _vm.TotalCount.Should().Be(2);
        _vm.TotalAmount.Should().Be(95000m);
        _vm.SummaryMessage.Should().Be("Bóc tách 2 dòng thành công");
    }

    [Fact]
    public async Task CommitBulkAsync_WhenWalletSelected_ShouldCallRepoAndClearRows()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        _vm.SelectedWallet = new WalletModel(walletId, Guid.NewGuid(), "Ví Chính", MobileWalletType.Cash, 5000000m, 5000000m, "VND", null, null, null, null, null, "#000", "wallet", 1, false, 1, 1, DateTime.UtcNow, null);
        _vm.Rows.Add(new ParsedImportRowModel { RowIndex = 1, Amount = 50000m, IsValid = true, IsSelected = true });

        var commitResult = new BulkCommitImportResultModel(1, 50000m, walletId, "Nhập thành công!");
        _mockImportRepo.Setup(r => r.CommitBulkImportAsync(It.IsAny<BulkCommitImportRequestModel>()))
                       .ReturnsAsync(commitResult);

        // Act
        await _vm.CommitBulkAsync();

        // Assert
        _vm.Rows.Should().BeEmpty();
        _vm.SummaryMessage.Should().Be("Nhập thành công!");
        _mockImportRepo.Verify(r => r.CommitBulkImportAsync(It.Is<BulkCommitImportRequestModel>(m => m.WalletId == walletId)), Times.Once);
    }
}
