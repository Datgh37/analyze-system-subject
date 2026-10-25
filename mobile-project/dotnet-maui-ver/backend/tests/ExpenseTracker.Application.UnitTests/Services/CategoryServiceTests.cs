using ExpenseTracker.Application.DTOs.Categories;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating hierarchical categories and domain constraints
public class CategoryServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryService _categoryService;
    private readonly Guid _userId = Guid.NewGuid();

    public CategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _categoryService = new CategoryService(_dbContext);
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnHierarchicalTree()
    {
        // Arrange
        var rootCat = new Category("Thu nhập chính", TransactionType.Income, _userId);
        await _dbContext.Categories.AddAsync(rootCat);
        await _dbContext.SaveChangesAsync();

        var subCat = new Category("Lương cứng", TransactionType.Income, _userId, parentId: rootCat.Id);
        await _dbContext.Categories.AddAsync(subCat);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.GetCategoriesAsync(_userId, TransactionType.Income);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().ContainSingle(c => c.Id == rootCat.Id);
        
        var root = result.Value!.First(c => c.Id == rootCat.Id);
        root.SubCategories.Should().NotBeNull();
        root.SubCategories!.Should().ContainSingle(c => c.Id == subCat.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var request = new CreateCategoryRequest("Tiền thưởng dự án", TransactionType.Income, IconKey: "gift", ColorHex: "#10B981");

        // Act
        var result = await _categoryService.CreateCategoryAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Tiền thưởng dự án");
        result.Value.IsSystem.Should().BeFalse();

        var saved = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == result.Value.Id);
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenSettingSelfAsParent_ShouldFail()
    {
        // Arrange
        var cat = new Category("Đầu tư", TransactionType.Income, _userId);
        await _dbContext.Categories.AddAsync(cat);
        await _dbContext.SaveChangesAsync();

        var updateReq = new UpdateCategoryRequest("Đầu tư sửa", ParentId: cat.Id);

        // Act
        var result = await _categoryService.UpdateCategoryAsync(_userId, cat.Id, updateReq);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("vòng lặp phân cấp bất hợp lệ");
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenSystemCategory_ShouldFail()
    {
        // Arrange
        var systemCat = new Category("Lương hệ thống", TransactionType.Income, isSystem: true);
        await _dbContext.Categories.AddAsync(systemCat);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.DeleteCategoryAsync(_userId, systemCat.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("danh mục mặc định");
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenDuplicateNameInSameParent_ShouldFail()
    {
        // Arrange
        var existingCat = new Category("Cổ tức", TransactionType.Income, _userId);
        await _dbContext.Categories.AddAsync(existingCat);
        await _dbContext.SaveChangesAsync();

        var request = new CreateCategoryRequest("Cổ tức", TransactionType.Income);

        // Act
        var result = await _categoryService.CreateCategoryAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Tên danh mục này đã tồn tại trong cùng nhóm");
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenCycleDetected_ShouldFail()
    {
        // Arrange: Cat A -> Cat B
        var catA = new Category("Nhóm A", TransactionType.Income, _userId);
        await _dbContext.Categories.AddAsync(catA);
        await _dbContext.SaveChangesAsync();

        var catB = new Category("Nhóm B", TransactionType.Income, _userId, parentId: catA.Id);
        await _dbContext.Categories.AddAsync(catB);
        await _dbContext.SaveChangesAsync();

        // Act: Try to make Cat B parent of Cat A (Cycle: A -> B -> A)
        var updateReq = new UpdateCategoryRequest("Nhóm A", ParentId: catB.Id);
        var result = await _categoryService.UpdateCategoryAsync(_userId, catA.Id, updateReq);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("vòng lặp phân cấp bất hợp lệ");
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenExceedingThreeLevelDepth_ShouldFail()
    {
        // Arrange: Level 1 -> Level 2 -> Level 3
        var level1 = new Category("Cấp 1", TransactionType.Income, _userId);
        await _dbContext.Categories.AddAsync(level1);
        await _dbContext.SaveChangesAsync();

        var level2 = new Category("Cấp 2", TransactionType.Income, _userId, parentId: level1.Id);
        await _dbContext.Categories.AddAsync(level2);
        await _dbContext.SaveChangesAsync();

        var level3 = new Category("Cấp 3", TransactionType.Income, _userId, parentId: level2.Id);
        await _dbContext.Categories.AddAsync(level3);
        await _dbContext.SaveChangesAsync();

        // Act: Try to create Level 4 under Level 3
        var reqLevel4 = new CreateCategoryRequest("Cấp 4", TransactionType.Income, ParentId: level3.Id);
        var result = await _categoryService.CreateCategoryAsync(_userId, reqLevel4);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Hệ thống chỉ hỗ trợ tối đa 3 tầng phân cấp danh mục");
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenHasTransactionsAndTargetCategoryProvided_ShouldReassignTransactionsAndSucceed()
    {
        // Arrange
        var catToDelete = new Category("Danh mục cũ", TransactionType.Income, _userId);
        var targetCat = new Category("Danh mục mới", TransactionType.Income, _userId);
        var wallet = new Wallet(_userId, "Ví Test", WalletType.Cash, 1_000_000m);

        await _dbContext.Categories.AddRangeAsync(catToDelete, targetCat);
        await _dbContext.Wallets.AddAsync(wallet);
        await _dbContext.SaveChangesAsync();

        var tx = new Transaction(_userId, wallet.Id, catToDelete.Id, 500_000m, TransactionType.Income, DateTime.UtcNow);
        await _dbContext.Transactions.AddAsync(tx);
        await _dbContext.SaveChangesAsync();

        // Act: Delete catToDelete and reassign transactions to targetCat
        var result = await _categoryService.DeleteCategoryAsync(_userId, catToDelete.Id, targetCategoryId: targetCat.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedTx = await _dbContext.Transactions.FindAsync(tx.Id);
        updatedTx!.CategoryId.Should().Be(targetCat.Id);

        var deletedCat = await _dbContext.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == catToDelete.Id);
        deletedCat!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task PatchCategoryAsync_WithPartialProperties_ShouldOnlyUpdateProvidedFields()
    {
        // Arrange
        var cat = new Category("Cà phê", TransactionType.Expense, _userId, iconKey: "coffee", colorHex: "#795548", displayOrder: 1);
        await _dbContext.Categories.AddAsync(cat);
        await _dbContext.SaveChangesAsync();

        var patchReq = new PatchCategoryRequest(ColorHex: "#3E2723", DisplayOrder: 5);

        // Act
        var result = await _categoryService.PatchCategoryAsync(_userId, cat.Id, patchReq);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Cà phê"); // unchanged
        result.Value.IconKey.Should().Be("coffee"); // unchanged
        result.Value.ColorHex.Should().Be("#3E2723"); // patched
        result.Value.DisplayOrder.Should().Be(5); // patched
    }
}
