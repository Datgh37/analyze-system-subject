using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating budget planning, 50/30/20, ZBB invariants, and rebalancing (UC-05.01 - 05.04)
public class BudgetServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BudgetService _budgetService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Category _needsCategory;
    private readonly Category _wantsCategory;

    public BudgetServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _budgetService = new BudgetService(_dbContext);

        _needsCategory = new Category("Thực phẩm & Đi chợ", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _wantsCategory = new Category("Cà phê & Giải trí", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Wants);

        _dbContext.Categories.AddRange(_needsCategory, _wantsCategory);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task CreateBudgetAsync_With503020Rule_CreatesActiveBudgetAndAutoAllocates()
    {
        // Arrange
        var request = new CreateBudgetRequest(
            Name: "Ngân sách Tháng 10",
            PeriodType: 1, // Monthly
            StartDate: new DateTime(2026, 10, 1),
            EndDate: new DateTime(2026, 10, 31),
            TotalAmount: 10_000_000m,
            Method: 1 // Rule 50/30/20
        );

        // Act
        var result = await _budgetService.CreateBudgetAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Ngân sách Tháng 10");
        result.Value.TotalAmount.Should().Be(10_000_000m);
        result.Value.Allocations.Should().HaveCount(2);

        var needsAlloc = result.Value.Allocations.FirstOrDefault(a => a.CategoryId == _needsCategory.Id);
        needsAlloc.Should().NotBeNull();
        needsAlloc!.AllocatedAmount.Should().Be(5_000_000m); // 50%

        var wantsAlloc = result.Value.Allocations.FirstOrDefault(a => a.CategoryId == _wantsCategory.Id);
        wantsAlloc.Should().NotBeNull();
        wantsAlloc!.AllocatedAmount.Should().Be(3_000_000m); // 30%
    }

    [Fact]
    public async Task CreateBudgetAsync_WithZBB_WhenAllocationsDoNotEqualTotal_ReturnsFailure()
    {
        // Arrange: Total = 10,000,000 but allocations sum to 9,000,000 != 10,000,000
        var request = new CreateBudgetRequest(
            Name: "Ngân sách ZBB",
            PeriodType: 1,
            StartDate: new DateTime(2026, 10, 1),
            EndDate: new DateTime(2026, 10, 31),
            TotalAmount: 10_000_000m,
            Method: 2, // Zero-Based Budgeting
            Allocations: new List<CreateBudgetAllocationRequest>
            {
                new(_needsCategory.Id, 6_000_000m),
                new(_wantsCategory.Id, 3_000_000m)
            }
        );

        // Act
        var result = await _budgetService.CreateBudgetAsync(_userId, request);

        // Assert (UC-05.01 E1)
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Ngân sách số không (ZBB) yêu cầu tổng phân bổ");
    }

    [Fact]
    public async Task CreateBudgetAsync_WithZBB_WhenAllocationsEqualTotal_Succeeds()
    {
        // Arrange: Total = 10,000,000 and allocations sum = 10,000,000
        var request = new CreateBudgetRequest(
            Name: "Ngân sách ZBB Chuẩn",
            PeriodType: 1,
            StartDate: new DateTime(2026, 10, 1),
            EndDate: new DateTime(2026, 10, 31),
            TotalAmount: 10_000_000m,
            Method: 2, // Zero-Based Budgeting
            Allocations: new List<CreateBudgetAllocationRequest>
            {
                new(_needsCategory.Id, 6_000_000m),
                new(_wantsCategory.Id, 4_000_000m)
            }
        );

        // Act
        var result = await _budgetService.CreateBudgetAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalAllocated.Should().Be(10_000_000m);
    }

    [Fact]
    public async Task RebalanceBudgetAsync_ShouldAdjustCategoryLimits()
    {
        // Arrange
        var budget = new Budget(_userId, "Tháng 10", 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        budget.AddAllocation(_needsCategory.Id, 5_000_000m);
        budget.AddAllocation(_wantsCategory.Id, 3_000_000m);
        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        var rebalanceReq = new RebalanceBudgetRequest(new List<RebalanceAllocationItem>
        {
            new(_needsCategory.Id, 6_000_000m),
            new(_wantsCategory.Id, 2_000_000m)
        });

        // Act
        var result = await _budgetService.RebalanceBudgetAsync(_userId, budget.Id, rebalanceReq);

        // Assert (UC-05.03)
        result.IsSuccess.Should().BeTrue();
        var needs = result.Value!.Allocations.First(a => a.CategoryId == _needsCategory.Id);
        needs.AllocatedAmount.Should().Be(6_000_000m);
        var wants = result.Value.Allocations.First(a => a.CategoryId == _wantsCategory.Id);
        wants.AllocatedAmount.Should().Be(2_000_000m);
    }

    [Fact]
    public async Task RecordExpenseAgainstBudgetAsync_IncreasesCurrentSpentInActiveBudget()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var budget = new Budget(_userId, "Tháng 10", 10_000_000m, today.AddDays(-5), today.AddDays(25));
        budget.AddAllocation(_needsCategory.Id, 5_000_000m);
        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        // Act
        await _budgetService.RecordExpenseAgainstBudgetAsync(_userId, _needsCategory.Id, 750_000m, today);

        // Assert
        var updatedBudget = await _dbContext.Budgets.Include(b => b.Allocations).FirstAsync(b => b.Id == budget.Id);
        var alloc = updatedBudget.Allocations.First(a => a.CategoryId == _needsCategory.Id);
        alloc.CurrentSpent.Should().Be(750_000m);
        alloc.RemainingAmount.Should().Be(4_250_000m);
    }
}
