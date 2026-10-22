using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Smoke test to verify test harness and assertions setup
public class InfrastructureSmokeTests
{
    [Fact]
    public void TestFramework_WhenExecutingSmokeTest_ShouldPassSuccessfully()
    {
        // Arrange
        const bool isReady = true;

        // Act & Assert
        isReady.Should().BeTrue();
    }
}
