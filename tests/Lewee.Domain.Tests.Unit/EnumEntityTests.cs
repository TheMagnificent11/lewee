using System.ComponentModel;
using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public static class EnumDescriptions
{
    public const string TestDescription1 = "Test Description 1";
    public const string TestDescription2 = "Test Description 2";
    public const string TestDescription3 = "Test Description 3";
}

public enum TestEnum
{
    [Description(EnumDescriptions.TestDescription1)]
    Value1 = 1,

    [Description(EnumDescriptions.TestDescription2)]
    Value2 = 2,

    [Description(EnumDescriptions.TestDescription3)]
    Value3 = 3,
}

public class EnumEntityTests
{
    [Fact]
    public void EnumEntity_ConstructorWithId_SetsIdAndNameCorrectly()
    {
        // Arrange
        var testEnumValue = TestEnum.Value2;

        // Act
        var enumEntity = new EnumEntity<TestEnum>(testEnumValue);

        // Assert
        enumEntity.Id.Should().Be(testEnumValue);
        enumEntity.Name.Should().Be(EnumDescriptions.TestDescription2);
    }

    [Fact]
    public void EnumEntity_DefaultConstructor_SetsNameToEF()
    {
        // Arrange & Act
        var enumEntity = Activator.CreateInstance(typeof(EnumEntity<TestEnum>), true) as EnumEntity<TestEnum>;

        // Assert
        enumEntity.Name.Should().Be("EF");
    }
}
