using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

public class NetWorthSnapshotTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCalculateNetWorthAccurately()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = new DateTime(2026, 10, 23);
        var totalAssets = 150_000_000m;
        var totalLiabilities = 30_000_000m;
        var notes = "Snapshot kiểm thử quý 4";

        // Act
        var snapshot = new NetWorthSnapshot(userId, date, totalAssets, totalLiabilities, notes);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.UserId.Should().Be(userId);
        snapshot.SnapshotDate.Should().Be(date.Date);
        snapshot.TotalAssets.Should().Be(totalAssets);
        snapshot.TotalLiabilities.Should().Be(totalLiabilities);
        snapshot.NetWorth.Should().Be(120_000_000m);
        snapshot.Notes.Should().Be(notes);
        snapshot.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Constructor_WithNegativeAssetsOrLiabilities_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = DateTime.UtcNow;

        // Act & Assert
        var actEmptyUser = () => new NetWorthSnapshot(Guid.Empty, date, 100m, 10m);
        actEmptyUser.Should().Throw<ArgumentException>()
            .WithMessage("*Mã người dùng không hợp lệ*");

        var actNegativeAssets = () => new NetWorthSnapshot(userId, date, -50m, 10m);
        actNegativeAssets.Should().Throw<ArgumentException>()
            .WithMessage("*Tổng tài sản không được là số âm*");

        var actNegativeLiabilities = () => new NetWorthSnapshot(userId, date, 100m, -20m);
        actNegativeLiabilities.Should().Throw<ArgumentException>()
            .WithMessage("*Tổng nợ không được là số âm*");
    }

    [Fact]
    public void NegativeNetWorth_WhenLiabilitiesExceedAssets_ShouldBeAllowed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = new DateTime(2026, 10, 23);
        var totalAssets = 20_000_000m;
        var totalLiabilities = 50_000_000m;

        // Act
        var snapshot = new NetWorthSnapshot(userId, date, totalAssets, totalLiabilities);

        // Assert
        snapshot.NetWorth.Should().Be(-30_000_000m);
        snapshot.TotalAssets.Should().Be(totalAssets);
        snapshot.TotalLiabilities.Should().Be(totalLiabilities);
    }

    [Fact]
    public void UpdateNotes_And_Recalculate_ShouldUpdateValuesAndTimestamp()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var snapshot = new NetWorthSnapshot(userId, DateTime.UtcNow, 100_000_000m, 20_000_000m);

        // Act - Update notes
        snapshot.UpdateNotes("Ghi chú bổ sung");
        snapshot.Notes.Should().Be("Ghi chú bổ sung");
        snapshot.UpdatedAt.Should().NotBeNull();

        // Act - Recalculate
        snapshot.Recalculate(150_000_000m, 10_000_000m, "Tái tính toán thành công");

        // Assert
        snapshot.TotalAssets.Should().Be(150_000_000m);
        snapshot.TotalLiabilities.Should().Be(10_000_000m);
        snapshot.NetWorth.Should().Be(140_000_000m);
        snapshot.Notes.Should().Be("Tái tính toán thành công");
    }
}
