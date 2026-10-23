using ExpenseTracker.Domain.Entities;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Unit tests validating BaseEntity audit timestamps and soft delete lifecycle
public class BaseEntityTests
{
    [Fact]
    public void MarkAsDeleted_ShouldSetIsDeletedTrue_AndStampDeletedAt()
    {
        // Arrange
        var user = new User("John Doe", "john@example.com", "HashedPass123");

        // Act
        user.MarkAsDeleted();

        // Assert
        Assert.True(user.IsDeleted);
        Assert.NotNull(user.DeletedAt);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void Restore_ShouldSetIsDeletedFalse_AndResetDeletedAt()
    {
        // Arrange
        var user = new User("John Doe", "john@example.com", "HashedPass123");
        user.MarkAsDeleted();

        // Act
        user.Restore();

        // Assert
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.NotNull(user.UpdatedAt);
    }
}
