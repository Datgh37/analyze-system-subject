using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating multi-wallet operations, transfers and balance adjustments
public class WalletServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly WalletService _walletService;
    private readonly Guid _userId = Guid.NewGuid();

    public WalletServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _walletService = new WalletService(_dbContext);
    }

    [Fact]
    public async Task CreateWalletAsync_WithValidData_ReturnsSuccessAndPersistsWallet()
    {
        // Arrange
        var request = new CreateWalletRequest(
            Name: "Ví Tiền Mặt",
            WalletType: WalletType.Cash,
            InitialBalance: 500000m,
            Currency: "VND"
        );

        // Act
        var result = await _walletService.CreateWalletAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Ví Tiền Mặt");
        result.Value.Balance.Should().Be(500000m);

        var savedWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == result.Value.Id);
        savedWallet.Should().NotBeNull();
        savedWallet!.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task TransferMoneyAsync_WithSufficientFunds_DebitsSourceAndCreditsDestination()
    {
        // Arrange
        var sourceWallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 1000000m);
        var destWallet = new Wallet(_userId, "Ví Tiết Kiệm", WalletType.BankAccount, 200000m);
        _dbContext.Wallets.AddRange(sourceWallet, destWallet);
        await _dbContext.SaveChangesAsync();

        var request = new TransferMoneyRequest(
            SourceWalletId: sourceWallet.Id,
            DestinationWalletId: destWallet.Id,
            Amount: 300000m,
            TransferDate: DateTime.UtcNow,
            Note: "Trích tiền tiết kiệm"
        );

        // Act
        var result = await _walletService.TransferMoneyAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedSource = await _dbContext.Wallets.FindAsync(sourceWallet.Id);
        var updatedDest = await _dbContext.Wallets.FindAsync(destWallet.Id);

        updatedSource!.Balance.Should().Be(700000m);
        updatedDest!.Balance.Should().Be(500000m);
    }

    [Fact]
    public async Task TransferMoneyAsync_WithInsufficientFunds_FailsAndPreservesBalances()
    {
        // Arrange
        var sourceWallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 100000m);
        var destWallet = new Wallet(_userId, "Ví Tiết Kiệm", WalletType.BankAccount, 50000m);
        _dbContext.Wallets.AddRange(sourceWallet, destWallet);
        await _dbContext.SaveChangesAsync();

        var request = new TransferMoneyRequest(
            SourceWalletId: sourceWallet.Id,
            DestinationWalletId: destWallet.Id,
            Amount: 300000m,
            TransferDate: DateTime.UtcNow
        );

        // Act
        var result = await _walletService.TransferMoneyAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();

        var updatedSource = await _dbContext.Wallets.FindAsync(sourceWallet.Id);
        var updatedDest = await _dbContext.Wallets.FindAsync(destWallet.Id);

        updatedSource!.Balance.Should().Be(100000m);
        updatedDest!.Balance.Should().Be(50000m);
    }

    [Fact]
    public async Task TransferMoneyAsync_WhenSameWallet_Fails()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Cá Nhân", WalletType.Cash, 500000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        var request = new TransferMoneyRequest(
            SourceWalletId: wallet.Id,
            DestinationWalletId: wallet.Id,
            Amount: 100000m,
            TransferDate: DateTime.UtcNow
        );

        // Act
        var result = await _walletService.TransferMoneyAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("trùng nhau");
    }

    [Fact]
    public async Task AdjustBalanceAsync_ShouldRecordAdjustmentAndSetNewBalance()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Momo", WalletType.EWallet, 150000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        var request = new AdjustBalanceRequest(
            NewBalance: 200000m,
            Reason: "Cân chỉnh số dư thực tế theo app",
            AdjustmentDate: DateTime.UtcNow
        );

        // Act
        var result = await _walletService.AdjustBalanceAsync(_userId, wallet.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.OldBalance.Should().Be(150000m);
        result.Value.NewBalance.Should().Be(200000m);
        result.Value.DifferenceAmount.Should().Be(50000m);

        var updatedWallet = await _dbContext.Wallets.FindAsync(wallet.Id);
        updatedWallet!.Balance.Should().Be(200000m);

        var adjustments = await _dbContext.BalanceAdjustments.Where(a => a.WalletId == wallet.Id).ToListAsync();
        adjustments.Should().HaveCount(1);
        adjustments[0].DifferenceAmount.Should().Be(50000m);
    }

    [Fact]
    public async Task CreateWalletAsync_WhenDuplicateName_ShouldFail()
    {
        // Arrange
        var existingWallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 0m);
        _dbContext.Wallets.Add(existingWallet);
        await _dbContext.SaveChangesAsync();

        var request = new CreateWalletRequest("Ví Tiền Mặt", WalletType.Cash, 100000m);

        // Act
        var result = await _walletService.CreateWalletAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Tên ví này đã tồn tại");
    }

    [Fact]
    public async Task DeleteWalletAsync_WhenWalletHasBalance_ShouldFail()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Tiết Kiệm", WalletType.BankAccount, 500000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _walletService.DeleteWalletAsync(_userId, wallet.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Không thể xóa ví đang có số dư lớn hơn 0");
    }

    [Fact]
    public async Task TransferMoneyAsync_ShouldLogWalletTransferRecord()
    {
        // Arrange
        var source = new Wallet(_userId, "Ví A", WalletType.BankAccount, 1000000m);
        var dest = new Wallet(_userId, "Ví B", WalletType.Cash, 200000m);
        _dbContext.Wallets.AddRange(source, dest);
        await _dbContext.SaveChangesAsync();

        var request = new TransferMoneyRequest(source.Id, dest.Id, 300000m, DateTime.UtcNow, Note: "Rút tiền mặt");

        // Act
        var result = await _walletService.TransferMoneyAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var transfer = await _dbContext.WalletTransfers
            .FirstOrDefaultAsync(t => t.SourceWalletId == source.Id && t.DestinationWalletId == dest.Id);

        transfer.Should().NotBeNull();
        transfer!.Amount.Should().Be(300000m);
        transfer.Note.Should().Be("Rút tiền mặt");
    }

    [Fact]
    public async Task AdjustBalanceAsync_WhenDifferenceIsZero_ShouldFail()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Tiền Mặt Khớp", WalletType.Cash, 1500000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        var request = new AdjustBalanceRequest(NewBalance: 1500000m, Reason: "Kiểm đếm định kỳ");

        // Act
        var result = await _walletService.AdjustBalanceAsync(_userId, wallet.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Số dư thực tế hoàn toàn trùng khớp với sổ sách hệ thống");
    }

    [Fact]
    public async Task AdjustBalanceAsync_WhenNegativeBalanceForCash_ShouldFail()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Tiền Mặt Âm", WalletType.Cash, 500000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        var request = new AdjustBalanceRequest(NewBalance: -50000m, Reason: "Thâm hụt");

        // Act
        var result = await _walletService.AdjustBalanceAsync(_userId, wallet.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("không thể là số âm");
    }

    [Fact]
    public async Task AdjustBalanceAsync_WhenReasonIsEmpty_ShouldFail()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Thiếu Lý Do", WalletType.Cash, 500000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        var request = new AdjustBalanceRequest(NewBalance: 600000m, Reason: "   ");

        // Act
        var result = await _walletService.AdjustBalanceAsync(_userId, wallet.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Vui lòng chọn hoặc nhập lý do cân chỉnh");
    }
}
