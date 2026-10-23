using System;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Nudges;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class NudgeEngineServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly NudgeEngineService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Category _foodCategory;
    private readonly Category _shoppingCategory;

    public NudgeEngineServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new NudgeEngineService(_dbContext, NullLogger<NudgeEngineService>.Instance);

        _foodCategory = new Category("Ăn uống", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _shoppingCategory = new Category("Mua sắm tùy chọn", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Wants);

        _dbContext.Categories.AddRange(_foodCategory, _shoppingCategory);
        _dbContext.SaveChanges();
    }

    // UC-09.01 Test 1: Get active nudges returns only active nudges
    [Fact]
    public async Task GetActiveNudges_ShouldReturnActiveNudgesOnly()
    {
        // Arrange
        var activeNudge = new FinancialNudge(_userId, NudgeType.DsaThreshold, "Nudge 1", "Nội dung 1", "Trigger1");
        var resolvedNudge = new FinancialNudge(_userId, NudgeType.PayYourselfFirst, "Nudge 2", "Nội dung 2", "Trigger2");
        resolvedNudge.Approve();

        _dbContext.FinancialNudges.AddRange(activeNudge, resolvedNudge);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetActiveNudgesAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Title.Should().Be("Nudge 1");
    }

    // UC-09.02 Test 2: Structured XAI rationale generation
    [Fact]
    public async Task GetNudgeRationale_WhenNudgeExists_ShouldReturnStructuredXaiRationale()
    {
        // Arrange
        var nudge = new FinancialNudge(_userId, NudgeType.DsaThreshold, "Cảnh báo DSA", "Đã tiêu 90% DSA", "ExpenseCreated");
        _dbContext.FinancialNudges.Add(nudge);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetNudgeRationaleAsync(_userId, nudge.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.MathematicalFormula.Should().Contain("DSA_t");
        result.Value.TriggerContext.Should().Contain("ExpenseCreated");
    }

    // UC-09.03 Test 3: Dynamic budget rebalance proposal preserves budget invariant
    [Fact]
    public async Task GenerateRebalanceProposal_ShouldPreserveTotalBudgetAllocationInvariant()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var budget = new Budget(_userId, "Ngân sách tháng", 10_000_000m, today.AddDays(-10), today.AddDays(20), BudgetPeriodType.Monthly, BudgetMethod.Rule503020);

        // Food (Needs): allocated 4M, spent 4.5M (deficit 500k)
        budget.AddAllocation(_foodCategory.Id, 4_000_000m);
        budget.RecordExpense(_foodCategory.Id, 4_500_000m);

        // Shopping (Wants): allocated 3M, spent 1M (surplus 2M)
        budget.AddAllocation(_shoppingCategory.Id, 3_000_000m);
        budget.RecordExpense(_shoppingCategory.Id, 1_000_000m);

        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GenerateRebalanceProposalAsync(_userId, _foodCategory.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.SourceCategoryId.Should().Be(_shoppingCategory.Id);
        result.Value.TargetCategoryId.Should().Be(_foodCategory.Id);
        result.Value.TransferAmount.Should().Be(500_000m);
        result.Value.SourceCategoryNewLimit.Should().Be(2_500_000m);
        result.Value.TargetCategoryNewLimit.Should().Be(4_500_000m);

        // Verify Invariant: Total allocated before == Total allocated after
        (result.Value.SourceCategoryNewLimit + result.Value.TargetCategoryNewLimit)
            .Should().Be(result.Value.SourceCategoryOldLimit + result.Value.TargetCategoryOldLimit);
    }

    // UC-09.04 Test 4: Approving a nudge with rebalance proposal adjusts database allocations
    [Fact]
    public async Task ApproveNudge_WithRebalanceProposal_ShouldAdjustCategoryLimitsAtomically()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var budget = new Budget(_userId, "Ngân sách tháng", 10_000_000m, today.AddDays(-5), today.AddDays(25), BudgetPeriodType.Monthly, BudgetMethod.Rule503020);
        budget.AddAllocation(_foodCategory.Id, 4_000_000m);
        budget.AddAllocation(_shoppingCategory.Id, 3_000_000m);
        _dbContext.Budgets.Add(budget);

        var proposalDto = new DynamicRebalanceProposalDto
        {
            ProposalId = Guid.NewGuid(),
            SourceCategoryId = _shoppingCategory.Id,
            TargetCategoryId = _foodCategory.Id,
            TransferAmount = 500_000m
        };

        var nudge = new FinancialNudge(
            _userId,
            NudgeType.DeficitRebalance,
            "Đề xuất tái cân đối",
            "Chuyển 500k từ mua sắm sang ăn uống",
            "RebalanceTrigger",
            rebalanceProposalJson: System.Text.Json.JsonSerializer.Serialize(proposalDto));

        _dbContext.FinancialNudges.Add(nudge);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.ApproveNudgeAsync(_userId, nudge.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        nudge.Status.Should().Be(NudgeStatus.Resolved);
        nudge.IsApprovedByUser.Should().BeTrue();

        // Check updated allocations in DB
        var freshFood = await _dbContext.BudgetAllocations.FirstAsync(a => a.CategoryId == _foodCategory.Id && a.BudgetId == budget.Id);
        var freshShop = await _dbContext.BudgetAllocations.FirstAsync(a => a.CategoryId == _shoppingCategory.Id && a.BudgetId == budget.Id);
        freshFood.AllocatedAmount.Should().Be(4_500_000m);
        freshShop.AllocatedAmount.Should().Be(2_500_000m);
    }

    // UC-09.01 Test 5: Anti-spam prevents duplicate nudges within 4 hours
    [Fact]
    public async Task EvaluateTriggersOnExpense_WhenRecentNudgeExists_ShouldSuppressDuplicateNudge()
    {
        // Arrange: Existing recent nudge created 1 hour ago
        var recentNudge = new FinancialNudge(_userId, NudgeType.DsaThreshold, "Cảnh báo vừa gửi", "Nội dung", "ExpenseCreated");
        _dbContext.FinancialNudges.Add(recentNudge);
        await _dbContext.SaveChangesAsync();

        // Act: Another expense occurs
        await _service.EvaluateTriggersOnExpenseAsync(_userId, 200_000m, _foodCategory.Id);

        // Assert: Still only 1 nudge in DB
        var count = await _dbContext.FinancialNudges.CountAsync(n => n.UserId == _userId);
        count.Should().Be(1);
    }
}
