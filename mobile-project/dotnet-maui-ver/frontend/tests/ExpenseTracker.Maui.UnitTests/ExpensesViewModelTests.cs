using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Models.Transactions;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Maui.UnitTests;

public class ExpensesViewModelTests
{
    private readonly Mock<ITransactionRepository> _mockTxRepo;
    private readonly Mock<IWalletRepository> _mockWalletRepo;
    private readonly Mock<ICategoryApi> _mockCategoryApi;
    private readonly Mock<IAutoRuleApi> _mockAutoRuleApi;
    private readonly ExpensesViewModel _viewModel;

    public ExpensesViewModelTests()
    {
        _mockTxRepo = new Mock<ITransactionRepository>();
        _mockWalletRepo = new Mock<IWalletRepository>();
        _mockCategoryApi = new Mock<ICategoryApi>();
        _mockAutoRuleApi = new Mock<IAutoRuleApi>();

        _viewModel = new ExpensesViewModel(
            _mockTxRepo.Object,
            _mockWalletRepo.Object,
            _mockCategoryApi.Object,
            _mockAutoRuleApi.Object
        );
    }

    [Fact]
    public async Task LoadDataAsync_ShouldPopulateWalletsCategoriesAndExpenses()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var catId = Guid.NewGuid();
        var wallets = new List<WalletModel>
        {
            new(walletId, Guid.NewGuid(), "Ví Chính", MobileWalletType.Cash, 3000000m, 3000000m, "VND", null, null, null, null, null, "#10B981", "wallet", 1, false, 1, 1, DateTime.UtcNow, null)
        };
        var categories = new List<CategoryModel>
        {
            new(catId, Guid.NewGuid(), "Ăn uống", 2, null, null, "food", "#EF4444", 1, true, null, 1, null)
        };
        var transactions = new List<TransactionModel>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), walletId, "Ví Chính", catId, "Ăn uống", "food", "#EF4444", 50000m, 2, DateTime.Today, "Cơm trưa", true, false, false, false, DateTime.UtcNow)
        };

        _mockWalletRepo.Setup(r => r.GetWalletsAsync()).ReturnsAsync(wallets);
        _mockCategoryApi.Setup(a => a.GetCategoriesAsync(2)).ReturnsAsync(categories);
        _mockTxRepo.Setup(r => r.GetTransactionsAsync(null, null, null, null, 2, 1, 50)).ReturnsAsync(transactions);
        _mockTxRepo.Setup(r => r.GetMonthlyExpenseTotalAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(50000m);

        // Act
        await _viewModel.LoadDataAsync();

        // Assert
        _viewModel.Wallets.Should().HaveCount(1);
        _viewModel.SelectedWallet.Should().NotBeNull();
        _viewModel.Categories.Should().HaveCount(1);
        _viewModel.SelectedCategory.Should().NotBeNull();
        _viewModel.Expenses.Should().HaveCount(1);
        _viewModel.MonthlyTotalExpense.Should().Be(50000m);
        _viewModel.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task CreateExpenseAsync_WhenAmountIsZeroOrNegative_ShouldSetErrorMessage()
    {
        // Arrange
        _viewModel.NewAmount = 0;

        // Act
        await _viewModel.CreateExpenseAsync();

        // Assert
        _viewModel.ErrorMessage.Should().Be("Số tiền chi tiêu phải lớn hơn 0.");
        _mockTxRepo.Verify(r => r.CreateExpenseAsync(It.IsAny<CreateExpenseModel>()), Times.Never);
    }

    [Fact]
    public async Task CreateExpenseAsync_WhenWalletNotSelected_ShouldSetErrorMessage()
    {
        // Arrange
        _viewModel.NewAmount = 100000m;
        _viewModel.SelectedWallet = null;

        // Act
        await _viewModel.CreateExpenseAsync();

        // Assert
        _viewModel.ErrorMessage.Should().Be("Vui lòng chọn ví thanh toán.");
        _mockTxRepo.Verify(r => r.CreateExpenseAsync(It.IsAny<CreateExpenseModel>()), Times.Never);
    }

    [Fact]
    public async Task CreateExpenseAsync_WhenValid_ShouldCallRepository_AndResetInputFields()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var catId = Guid.NewGuid();
        _viewModel.SelectedWallet = new WalletModel(walletId, Guid.NewGuid(), "Ví Chính", MobileWalletType.Cash, 3000000m, 3000000m, "VND", null, null, null, null, null, "#10B981", "wallet", 1, false, 1, 1, DateTime.UtcNow, null);
        _viewModel.SelectedCategory = new CategoryModel(catId, Guid.NewGuid(), "Ăn uống", 2, null, null, "food", "#EF4444", 1, true, null, 1, null);
        _viewModel.NewAmount = 75000m;
        _viewModel.Note = "Tiền cà phê";

        _mockTxRepo.Setup(r => r.CreateExpenseAsync(It.IsAny<CreateExpenseModel>()))
            .ReturnsAsync(new TransactionModel(Guid.NewGuid(), Guid.NewGuid(), walletId, "Ví Chính", catId, "Ăn uống", "food", "#EF4444", 75000m, 2, DateTime.Today, "Tiền cà phê", false, false, false, false, DateTime.UtcNow));

        _mockWalletRepo.Setup(r => r.GetWalletsAsync()).ReturnsAsync(new List<WalletModel> { _viewModel.SelectedWallet });
        _mockCategoryApi.Setup(a => a.GetCategoriesAsync(2)).ReturnsAsync(new List<CategoryModel> { _viewModel.SelectedCategory });
        _mockTxRepo.Setup(r => r.GetTransactionsAsync(null, null, null, null, 2, 1, 50)).ReturnsAsync(new List<TransactionModel>());
        _mockTxRepo.Setup(r => r.GetMonthlyExpenseTotalAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(75000m);

        // Act
        await _viewModel.CreateExpenseAsync();

        // Assert
        _mockTxRepo.Verify(r => r.CreateExpenseAsync(It.Is<CreateExpenseModel>(req => req.Amount == 75000m && req.WalletId == walletId)), Times.Once);
        _viewModel.NewAmount.Should().Be(0);
        _viewModel.Note.Should().BeEmpty();
        _viewModel.ErrorMessage.Should().BeNull();
    }
}
