using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class NetWorthServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly NetWorthService _service;
    private readonly Guid _userId = Guid.NewGuid();

    public NetWorthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new NetWorthService(_dbContext);
    }

    [Fact]
    public async Task GetCurrentNetWorth_CalculatesAssetsLiabilitiesAndNetWorthAccurately()
    {
        // Arrange
        // Assets: Ví tiền mặt 20tr + Ví ngân hàng 30tr + Quỹ tích lũy 50tr = 100tr
        var cashWallet = new Wallet(_userId, "Tiền mặt", WalletType.Cash, 20_000_000m);
        var bankWallet = new Wallet(_userId, "Ngân hàng", WalletType.BankAccount, 30_000_000m);
        _dbContext.Wallets.AddRange(cashWallet, bankWallet);

        var savingsGoal = new SavingsGoal(_userId, "Mua laptop", 60_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6), SavingsGoalType.MajorPurchase, SavingsGoalPriority.Medium, 50_000_000m);
        _dbContext.SavingsGoals.Add(savingsGoal);

        // Liabilities: Khoản vay tiêu dùng 20tr
        var debt = new Debt(_userId, "Vay tiêu dùng", DebtType.InstallmentLoan, "FE Credit", 30_000_000m, 15.0m, 3_000_000m, 10, DateTime.UtcNow, currentBalance: 20_000_000m);
        _dbContext.Debts.Add(debt);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetCurrentNetWorthAsync(_userId);

        // Assert
        result.Should().NotBeNull();
        result.TotalAssets.Should().Be(100_000_000m);
        result.TotalLiabilities.Should().Be(20_000_000m);
        result.NetWorth.Should().Be(80_000_000m); // 100tr - 20tr = 80tr
        result.DebtToAssetRatio.Should().Be(20.0m); // (20tr / 100tr) * 100% = 20%
        result.FinancialHealthRating.Should().Be("Healthy");
    }

    [Fact]
    public async Task GetCurrentNetWorth_IncludesCreditCardDebtInLiabilities()
    {
        // Arrange
        var cashWallet = new Wallet(_userId, "Tiền mặt", WalletType.Cash, 10_000_000m);
        
        // Thẻ tín dụng đã tiêu 15tr (Số dư âm -15tr)
        var creditWallet = new Wallet(_userId, "Thẻ Tín Dụng", WalletType.CreditCard, 0m, creditLimit: 30_000_000m);
        creditWallet.Debit(15_000_000m); // Balance = -15_000_000

        _dbContext.Wallets.AddRange(cashWallet, creditWallet);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetCurrentNetWorthAsync(_userId);

        // Assert
        result.Assets.LiquidCash.Should().Be(10_000_000m);
        result.Liabilities.CreditCardDebt.Should().Be(15_000_000m);
        result.TotalLiabilities.Should().Be(15_000_000m);
        result.NetWorth.Should().Be(-5_000_000m); // 10tr - 15tr = -5tr
        result.FinancialHealthRating.Should().Be("CriticalRisk");
    }

    [Fact]
    public async Task GetCurrentNetWorth_WhenRatioBetween30And50_AssignsCautionRating()
    {
        // Arrange
        var cashWallet = new Wallet(_userId, "Tiền mặt", WalletType.Cash, 100_000_000m);
        _dbContext.Wallets.Add(cashWallet);

        var debt = new Debt(_userId, "Nợ bạn bè", DebtType.Personal, "Bạn thân", 40_000_000m, 0m, 5_000_000m, 1, DateTime.UtcNow, currentBalance: 40_000_000m);
        _dbContext.Debts.Add(debt);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetCurrentNetWorthAsync(_userId);

        // Assert
        result.DebtToAssetRatio.Should().Be(40.0m); // 40%
        result.FinancialHealthRating.Should().Be("Caution");
    }

    [Fact]
    public async Task RecordSnapshot_SavesDailySnapshot_AndRecalculatesIfSameDay()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Tiền mặt", WalletType.Cash, 50_000_000m);
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();

        // Act 1: Tạo snapshot đầu tiên trong ngày
        var snapshot1 = await _service.RecordSnapshotAsync(_userId, new RecordNetWorthSnapshotDto { Notes = "Điểm ảnh sáng" });
        snapshot1.NetWorth.Should().Be(50_000_000m);

        // Arrange 2: Tiền ví tăng thêm 10tr
        wallet.Credit(10_000_000m);
        await _dbContext.SaveChangesAsync();

        // Act 2: Cập nhật snapshot cùng ngày
        var snapshot2 = await _service.RecordSnapshotAsync(_userId, new RecordNetWorthSnapshotDto { Notes = "Điểm ảnh tối cập nhật" });

        // Assert: Cùng ID nhưng giá trị được tính toán lại
        snapshot2.Id.Should().Be(snapshot1.Id);
        snapshot2.NetWorth.Should().Be(60_000_000m);
        snapshot2.Notes.Should().Be("Điểm ảnh tối cập nhật");

        var allSnapshots = await _dbContext.NetWorthSnapshots.Where(s => s.UserId == _userId).ToListAsync();
        allSnapshots.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetNetWorthHistory_ReturnsOrderedSnapshots()
    {
        // Arrange
        var s1 = new NetWorthSnapshot(_userId, DateTime.UtcNow.AddDays(-20), 100_000_000m, 20_000_000m, "Snapshot 1");
        var s2 = new NetWorthSnapshot(_userId, DateTime.UtcNow.AddDays(-10), 110_000_000m, 15_000_000m, "Snapshot 2");
        var s3 = new NetWorthSnapshot(_userId, DateTime.UtcNow, 120_000_000m, 10_000_000m, "Snapshot 3");
        _dbContext.NetWorthSnapshots.AddRange(s1, s2, s3);
        await _dbContext.SaveChangesAsync();

        // Act
        var history = await _service.GetNetWorthHistoryAsync(_userId, limit: 12);

        // Assert
        history.Should().HaveCount(3);
        history[0].NetWorth.Should().Be(110_000_000m); // Snapshot 3 (mới nhất)
        history[1].NetWorth.Should().Be(95_000_000m);  // Snapshot 2
        history[2].NetWorth.Should().Be(80_000_000m);  // Snapshot 1
    }

    [Fact]
    public async Task SurvivalRunway_CalculatesCorrectMonthsBasedOnNeeds()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Tiền mặt", WalletType.Cash, 60_000_000m);
        var category = new Category("Tiền thuê nhà", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _dbContext.Wallets.Add(wallet);
        _dbContext.Categories.Add(category);

        // Chi phí thiết yếu 30tr trong 90 ngày = 10tr/tháng
        var tx = new Transaction(_userId, wallet.Id, category.Id, 30_000_000m, TransactionType.Expense, DateTime.UtcNow.AddDays(-15));
        _dbContext.Transactions.Add(tx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetCurrentNetWorthAsync(_userId);

        // Assert: 60tr tài sản / 10tr chi phí thiết yếu = 6 tháng sinh tồn
        result.SurvivalRunwayMonths.Should().Be(6.0m);
    }
}
