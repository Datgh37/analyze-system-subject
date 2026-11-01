using ExpenseTracker.Application.DTOs.Debts;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Bộ kiểm thử tự động cho động cơ tối ưu hóa trả nợ và hướng dẫn tài chính bình dân hóa (UC-06.06)
public class DebtOptimizerServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<ILogger<DebtOptimizerService>> _loggerMock;
    private readonly DebtOptimizerService _optimizerService;
    private readonly Guid _userId = Guid.NewGuid();

    public DebtOptimizerServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<DebtOptimizerService>>();

        _optimizerService = new DebtOptimizerService(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public void GetEducationalGuide_ReturnsCompleteLaymanGuide()
    {
        // Act
        var guide = _optimizerService.GetEducationalGuide();

        // Assert
        guide.Should().NotBeNull();
        guide.Methods.Should().HaveCount(2);
        guide.Methods.Should().Contain(m => m.MethodKey == "SNOWBALL" && m.Title.Contains("Hòn Tuyết Lăn"));
        guide.Methods.Should().Contain(m => m.MethodKey == "AVALANCHE" && m.Title.Contains("Tuyết Lở"));
        guide.QuizQuestions.Should().NotBeEmpty();
        guide.GoldenRules.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetSafeExtraBudgetRecommendation_CalculatesValidSurplusAndSuggestedAmounts()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 20_000_000m);
        var category = new Category("Lương", TransactionType.Income, _userId);
        _dbContext.Wallets.Add(wallet);
        _dbContext.Categories.Add(category);

        // Thu nhập: 25.000.000đ
        var incomeTx = new Transaction(_userId, wallet.Id, category.Id, 25_000_000m, TransactionType.Income, DateTime.UtcNow);
        _dbContext.Transactions.Add(incomeTx);

        // Nợ tối thiểu: 2.000.000đ
        var debt = new Debt(_userId, "Trả Góp", DebtType.InstallmentLoan, "FE Credit", 10_000_000m, 18.0m, 2_000_000m, 15, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        // Act
        var rec = await _optimizerService.GetSafeExtraBudgetRecommendationAsync(_userId);

        // Assert
        rec.Should().NotBeNull();
        rec.MonthlyIncome.Should().Be(25_000_000m);
        rec.TotalMonthlyDebtMinimum.Should().Be(2_000_000m);
        rec.MonthlySurplus.Should().BeGreaterThan(0);
        rec.RecommendedAmount.Should().BeGreaterThan(0);
        rec.ConservativeAmount.Should().BeLessThanOrEqualTo(rec.RecommendedAmount);
        rec.RecommendedAmount.Should().BeLessThanOrEqualTo(rec.AggressiveAmount);
    }

    [Fact]
    public async Task GetSafeExtraBudgetRecommendation_WhenSurplusIsZeroOrNegative_WarnsUserToPayMinimum()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví Rỗng", WalletType.Cash, 500_000m);
        var category = new Category("Thu Nhập Thấp", TransactionType.Income, _userId);
        _dbContext.Wallets.Add(wallet);
        _dbContext.Categories.Add(category);

        // Thu nhập chỉ có 5.000.000đ
        var incomeTx = new Transaction(_userId, wallet.Id, category.Id, 5_000_000m, TransactionType.Income, DateTime.UtcNow);
        _dbContext.Transactions.Add(incomeTx);

        // Nợ tối thiểu tới 6.000.000đ
        var debt = new Debt(_userId, "Khoản Vay Lớn", DebtType.Personal, "Ngân hàng", 50_000_000m, 20.0m, 6_000_000m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        // Act
        var rec = await _optimizerService.GetSafeExtraBudgetRecommendationAsync(_userId);

        // Assert
        rec.Should().NotBeNull();
        rec.MonthlySurplus.Should().BeLessThanOrEqualTo(0);
        rec.ConservativeAmount.Should().Be(0);
        rec.RecommendedAmount.Should().Be(0);
        rec.AdviceText.Should().Contain("Dòng tiền hàng tháng của bạn đang khá căng thẳng hoặc bị thâm hụt");
    }

    [Fact]
    public async Task SimulateSnowball_PrioritizesLowestBalanceFirst()
    {
        // Arrange: 2 khoản nợ, khoản A dư nợ nhỏ (5tr), khoản B dư nợ lớn (20tr)
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Khoản Lớn Lãi Cao", 20_000_000m, 30.0m, 1_000_000m),
            new(Guid.NewGuid(), "Khoản Nhỏ Lãi Thấp", 5_000_000m, 10.0m, 500_000m)
        };

        // Act: Extra budget 1.000.000đ
        var result = await _optimizerService.SimulateSnowballAsync(_userId, 1_000_000m, customDebts);

        // Assert
        result.Should().NotBeNull();
        result.IsDebtFree.Should().BeTrue();
        result.PayoffOrder.Should().NotBeEmpty();
        // Snowball phải tất toán khoản nhỏ trước!
        result.PayoffOrder.First().Should().Be("Khoản Nhỏ Lãi Thấp");
    }

    [Fact]
    public async Task SimulateAvalanche_PrioritizesHighestInterestFirst()
    {
        // Arrange: Khoản Lãi Cao (5tr, 36%), Khoản Lãi Thấp (15tr, 8%)
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Khoản Lãi Cao", 5_000_000m, 36.0m, 500_000m),
            new(Guid.NewGuid(), "Khoản Lãi Thấp", 15_000_000m, 8.0m, 500_000m)
        };

        // Act: Extra budget 2.000.000đ -> Khoản Lãi Cao nhận 2.5tr/tháng xong ngay sau ~2 tháng
        var result = await _optimizerService.SimulateAvalancheAsync(_userId, 2_000_000m, customDebts);

        // Assert
        result.Should().NotBeNull();
        result.IsDebtFree.Should().BeTrue();
        // Avalanche dồn lực trả khoản lãi cao nhất trước!
        result.PayoffOrder.First().Should().Be("Khoản Lãi Cao");
    }

    [Fact]
    public async Task CompareStrategies_AvalancheSavesMoreOrEqualInterestThanSnowball()
    {
        // Arrange
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Thẻ Tín Dụng VIB", 15_000_000m, 32.0m, 800_000m),
            new(Guid.NewGuid(), "Trả Góp Laptop", 10_000_000m, 15.0m, 1_000_000m),
            new(Guid.NewGuid(), "Mượn Bạn Bè", 3_000_000m, 0.0m, 500_000m)
        };

        var request = new DebtPayoffSimulationRequestDto(
            MonthlyExtraBudget: 1_000_000m,
            CustomDebts: customDebts
        );

        // Act
        var comparison = await _optimizerService.CompareStrategiesAsync(_userId, request);

        // Assert
        comparison.Should().NotBeNull();
        comparison.Snowball.IsDebtFree.Should().BeTrue();
        comparison.Avalanche.IsDebtFree.Should().BeTrue();
        // Về mặt toán học, Avalanche luôn trả ít tiền lãi hơn hoặc bằng Snowball
        comparison.Avalanche.TotalInterestPaid.Should().BeLessThanOrEqualTo(comparison.Snowball.TotalInterestPaid);
        comparison.InterestSavingsWithAvalanche.Should().Be(comparison.Snowball.TotalInterestPaid - comparison.Avalanche.TotalInterestPaid);
    }

    [Fact]
    public async Task CompareStrategies_WhenExtraBudgetIsZero_CalculatesValidPayoff()
    {
        // Arrange
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Khoản Vay 1", 5_000_000m, 12.0m, 1_000_000m),
            new(Guid.NewGuid(), "Khoản Vay 2", 8_000_000m, 15.0m, 1_500_000m)
        };

        var request = new DebtPayoffSimulationRequestDto(
            MonthlyExtraBudget: 0m,
            CustomDebts: customDebts
        );

        // Act
        var comparison = await _optimizerService.CompareStrategiesAsync(_userId, request);

        // Assert
        comparison.Should().NotBeNull();
        comparison.Snowball.TotalMonths.Should().BeGreaterThan(0);
        comparison.Avalanche.TotalMonths.Should().BeGreaterThan(0);
        comparison.Snowball.IsDebtFree.Should().BeTrue();
    }

    [Fact]
    public async Task CompareStrategies_RollsOverMinimumPayment_WhenDebtPaidOff()
    {
        // Arrange: 2 khoản nợ, khoản 1 trả 1.000.000đ/tháng, khi trả xong ngân sách dồn sang khoản 2
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Khoản Nhỏ Xong Nhanh", 1_000_000m, 0.0m, 1_000_000m),
            new(Guid.NewGuid(), "Khoản Lớn", 10_000_000m, 10.0m, 500_000m)
        };

        var request = new DebtPayoffSimulationRequestDto(
            MonthlyExtraBudget: 0m,
            CustomDebts: customDebts
        );

        // Act
        var comparison = await _optimizerService.CompareStrategiesAsync(_userId, request);

        // Assert
        var snowball = comparison.Snowball;
        snowball.PayoffOrder.First().Should().Be("Khoản Nhỏ Xong Nhanh");
        // Tháng 2 tiền gốc trả cho Khoản Lớn phải tăng lên 1.500.000đ (500k min + 1.000k rollover)
        snowball.Timeline.Count.Should().BeGreaterThanOrEqualTo(2);
        var month2 = snowball.Timeline[1];
        month2.TotalPrincipalPaid.Should().Be(1_500_000m);
    }

    [Fact]
    public async Task CompareStrategies_DetectsNegativeAmortization_WhenPaymentCannotCoverInterest()
    {
        // Arrange: Khoản nợ 100 triệu, lãi 36%/năm (lãi tháng là 3 triệu), nhưng chỉ trả tối thiểu 500k và extra 0
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Bẫy Nợ Lãi Cắt Cổ", 100_000_000m, 36.0m, 500_000m)
        };

        var request = new DebtPayoffSimulationRequestDto(
            MonthlyExtraBudget: 0m,
            CustomDebts: customDebts
        );

        // Act
        var comparison = await _optimizerService.CompareStrategiesAsync(_userId, request);

        // Assert
        comparison.Should().NotBeNull();
        comparison.Snowball.HasNegativeAmortization.Should().BeTrue();
        comparison.Avalanche.HasNegativeAmortization.Should().BeTrue();
        comparison.RecommendedStrategy.Should().Be("ALERT_NEGATIVE_AMORTIZATION");
        comparison.LaymanRecommendationText.Should().Contain("Nguy hiểm! Số tiền bạn dự định trả mỗi tháng còn ít hơn cả tiền lãi");
    }

    [Fact]
    public async Task CompareStrategies_WhenSavingsExceedOneMillion_RecommendsAvalanche()
    {
        // Arrange: Nợ thẻ tín dụng lớn lãi 36% và nợ trả góp lớn lãi 8% -> Avalanche tiết kiệm hàng triệu tiền lãi
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Thẻ Tín Dụng HSBC", 40_000_000m, 36.0m, 1_500_000m),
            new(Guid.NewGuid(), "Vay Mua Xe Máy", 20_000_000m, 8.0m, 1_000_000m)
        };

        var request = new DebtPayoffSimulationRequestDto(
            MonthlyExtraBudget: 1_000_000m,
            CustomDebts: customDebts
        );

        // Act
        var comparison = await _optimizerService.CompareStrategiesAsync(_userId, request);

        // Assert
        comparison.InterestSavingsWithAvalanche.Should().BeGreaterThanOrEqualTo(1_000_000m);
        comparison.RecommendedStrategy.Should().Be("AVALANCHE");
        comparison.LaymanRecommendationText.Should().Contain("Bạn nên chọn Tuyết Lở (Avalanche)");
    }

    [Fact]
    public async Task CompareStrategies_WhenSavingsBelowOneMillion_RecommendsSnowball()
    {
        // Arrange: Các khoản nợ có lãi suất xấp xỉ nhau -> Chênh lệch lãi < 1 triệu -> Khuyên Snowball để giải tỏa tâm lý
        var customDebts = new List<DebtSimulationItemDto>
        {
            new(Guid.NewGuid(), "Khoản Vay A", 4_000_000m, 10.0m, 1_000_000m),
            new(Guid.NewGuid(), "Khoản Vay B", 6_000_000m, 10.5m, 1_000_000m)
        };

        var request = new DebtPayoffSimulationRequestDto(
            MonthlyExtraBudget: 500_000m,
            CustomDebts: customDebts
        );

        // Act
        var comparison = await _optimizerService.CompareStrategiesAsync(_userId, request);

        // Assert
        comparison.InterestSavingsWithAvalanche.Should().BeLessThan(1_000_000m);
        comparison.RecommendedStrategy.Should().Be("SNOWBALL");
        comparison.LaymanRecommendationText.Should().Contain("Bạn nên chọn Hòn Tuyết Lăn (Snowball)");
    }

    [Fact]
    public async Task CompareStrategies_WhenNoDebts_ReturnsZeroMonthsDebtFree()
    {
        // Arrange: Người dùng không có nợ trong CSDL và customDebts rỗng
        var request = new DebtPayoffSimulationRequestDto(
            MonthlyExtraBudget: 1_000_000m,
            CustomDebts: new List<DebtSimulationItemDto>()
        );

        // Act
        var comparison = await _optimizerService.CompareStrategiesAsync(_userId, request);

        // Assert
        comparison.Should().NotBeNull();
        comparison.Snowball.TotalMonths.Should().Be(0);
        comparison.Snowball.IsDebtFree.Should().BeTrue();
        comparison.Avalanche.TotalMonths.Should().Be(0);
        comparison.Avalanche.IsDebtFree.Should().BeTrue();
    }
}
