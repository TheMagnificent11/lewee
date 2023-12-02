using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class ValueObjectTests
{
    [Fact]
    public void Equals_ReturnsTrue_WhenPropertiesAreEqual()
    {
        // Arrange
        var address1 = new Address(123, "Main St", "Suburb", "State", "12345");
        var address2 = new Address(123, "Main St", "Suburb", "STATE", "12345");

        // Act
        var result = address1.Equals(address2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenPropertiesAreNotEqual()
    {
        // Arrange
        var address1 = new Address(123, "Main St", "Suburb", "State", "12345");
        var address2 = new Address(124, "Main St", "Suburb", "State", "12345");

        // Act
        var result = address1.Equals(address2);

        // Assert
        result.Should().BeFalse();
    }
}
