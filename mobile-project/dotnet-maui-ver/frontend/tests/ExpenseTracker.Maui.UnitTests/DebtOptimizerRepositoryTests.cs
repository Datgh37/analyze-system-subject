using ExpenseTracker.Maui.Models.Debts;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests.Repositories;

// Bộ kiểm thử tự động cho Repository và ViewModel Tối ưu hóa nợ trên Mobile (UC-06.06)
public class DebtOptimizerRepositoryTests
{
    private readonly MockDebtRepository _mockRepo;
    private readonly DebtOptimizerViewModel _viewModel;

    public DebtOptimizerRepositoryTests()
    {
        _mockRepo = new MockDebtRepository();
        _viewModel = new DebtOptimizerViewModel(_mockRepo);
    }

    [Fact]
    public async Task MockDebtRepository_GetEducationalGuideAsync_ReturnsValidGuide()
    {
        // Act
        var guide = await _mockRepo.GetEducationalGuideAsync();

        // Assert
        guide.Should().NotBeNull();
        guide.Methods.Should().HaveCount(2);
        guide.Methods.Should().Contain(m => m.MethodKey == "SNOWBALL");
        guide.Methods.Should().Contain(m => m.MethodKey == "AVALANCHE");
        guide.QuizQuestions.Should().NotBeEmpty();
        guide.GoldenRules.Should().HaveCount(3);
    }

    [Fact]
    public async Task MockDebtRepository_GetSafeBudgetRecommendationAsync_ReturnsSuggestedBudgets()
    {
        // Act
        var rec = await _mockRepo.GetSafeBudgetRecommendationAsync();

        // Assert
        rec.Should().NotBeNull();
        rec.MonthlySurplus.Should().BeGreaterThan(0);
        rec.RecommendedAmount.Should().BeGreaterThan(0);
        rec.ConservativeAmount.Should().BeLessThanOrEqualTo(rec.RecommendedAmount);
        rec.RecommendedAmount.Should().BeLessThanOrEqualTo(rec.AggressiveAmount);
    }

    [Fact]
    public async Task MockDebtRepository_CompareStrategiesAsync_CalculatesCorrectSavings()
    {
        // Act: Thử nghiệm với Extra Budget 1.000.000đ
        var comparison = await _mockRepo.CompareStrategiesAsync(1_000_000m);

        // Assert
        comparison.Should().NotBeNull();
        comparison.Snowball.IsDebtFree.Should().BeTrue();
        comparison.Avalanche.IsDebtFree.Should().BeTrue();
        comparison.Avalanche.TotalInterestPaid.Should().BeLessThanOrEqualTo(comparison.Snowball.TotalInterestPaid);
        comparison.Recommendation.Should().NotBeNullOrWhiteSpace();
        comparison.LaymanRecommendationText.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task DebtOptimizerViewModel_LoadInitialDataAsync_PopulatesGuideAndComparison()
    {
        // Act
        await _viewModel.LoadInitialDataAsync();

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.EducationalGuide.Should().NotBeNull();
        _viewModel.SafeBudgetRecommendation.Should().NotBeNull();
        _viewModel.ComparisonResult.Should().NotBeNull();
        _viewModel.LaymanAdvice.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void DebtOptimizerViewModel_SelectPersonality_SwitchesTargetStrategy()
    {
        // Act & Assert 1: Chọn STRESS -> Chuyển sang SNOWBALL
        _viewModel.SelectPersonality("STRESS");
        _viewModel.SelectedPersonality.Should().Be("STRESS");
        _viewModel.SelectedStrategy.Should().Be("SNOWBALL");

        // Act & Assert 2: Chọn SAVINGS -> Chuyển sang AVALANCHE
        _viewModel.SelectPersonality("SAVINGS");
        _viewModel.SelectedPersonality.Should().Be("SAVINGS");
        _viewModel.SelectedStrategy.Should().Be("AVALANCHE");
    }
}

