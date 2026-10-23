using ExpenseTracker.Application.DTOs.AutoRules;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating AutoCategorizationService keyword and regex matching logic (UC-04.07)
public class AutoCategorizationServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AutoCategorizationService _autoRuleService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Category _category;

    public AutoCategorizationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _autoRuleService = new AutoCategorizationService(_dbContext);

        _category = new Category("Đi chợ siêu thị", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(_category);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task CreateRuleAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var request = new CreateAutoRuleRequest("WinMart", _category.Id, PriorityOrder: 1);

        // Act
        var result = await _autoRuleService.CreateRuleAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.KeywordPattern.Should().Be("WinMart");
        result.Value.TargetCategoryName.Should().Be("Đi chợ siêu thị");

        var inDb = await _dbContext.AutoCategorizationRules.FirstOrDefaultAsync(r => r.Id == result.Value.Id);
        inDb.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateRuleAsync_WhenDuplicatePatternForSameCategory_ShouldFail()
    {
        // Arrange
        var existing = new AutoCategorizationRule(_userId, "WinMart", _category.Id);
        await _dbContext.AutoCategorizationRules.AddAsync(existing);
        await _dbContext.SaveChangesAsync();

        var request = new CreateAutoRuleRequest("winmart", _category.Id);

        // Act
        var result = await _autoRuleService.CreateRuleAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("đã được thiết lập trước đó");
    }

    [Fact]
    public async Task MatchCategoryAsync_WhenKeywordMatchesCaseInsensitive_ShouldReturnCategory()
    {
        // Arrange: WinMart -> Đi chợ siêu thị
        var rule = new AutoCategorizationRule(_userId, "WinMart", _category.Id, priorityOrder: 1, isCaseSensitive: false);
        await _dbContext.AutoCategorizationRules.AddAsync(rule);
        await _dbContext.SaveChangesAsync();

        // Act: Match against "Mua đồ tại siêu thị winmart cộng"
        var result = await _autoRuleService.MatchCategoryAsync(_userId, "Mua đồ tại siêu thị winmart cộng");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CategoryId.Should().Be(_category.Id);
        result.Value.CategoryName.Should().Be("Đi chợ siêu thị");
        result.Value.MatchedPattern.Should().Be("WinMart");
    }

    [Fact]
    public async Task MatchCategoryAsync_WhenRegexMatches_ShouldReturnCategory()
    {
        // Arrange: Regex rule @"(grab|shopee)food" -> Ăn uống
        var foodCategory = new Category("Ăn uống ngoài", TransactionType.Expense, _userId);
        await _dbContext.Categories.AddAsync(foodCategory);
        await _dbContext.SaveChangesAsync();

        var rule = new AutoCategorizationRule(_userId, @"(grab|shopee)food", foodCategory.Id, priorityOrder: 1, isRegex: true);
        await _dbContext.AutoCategorizationRules.AddAsync(rule);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _autoRuleService.MatchCategoryAsync(_userId, "Đặt cơm trưa qua GrabFood");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CategoryId.Should().Be(foodCategory.Id);
        result.Value.CategoryName.Should().Be("Ăn uống ngoài");
    }

    [Fact]
    public async Task MatchCategoryAsync_WhenNoRuleMatches_ShouldReturnNullCategory()
    {
        // Act
        var result = await _autoRuleService.MatchCategoryAsync(_userId, "Tiền đổ xăng xe máy");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CategoryId.Should().BeNull();
    }
}
