using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

[assembly: SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1649:File name should match first type name",
    Justification = "Test file contains multiple related test types",
    Scope = "type",
    Target = "~T:Lewee.Domain.Tests.Unit.EnumDescriptions")]

namespace Lewee.Domain.Tests.Unit;

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test helper class is tightly coupled with test enum and test class")]
internal static class EnumDescriptions
{
    public const string TestDescription1 = "Test Description 1";
    public const string TestDescription2 = "Test Description 2";
    public const string TestDescription3 = "Test Description 3";
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test enum is tightly coupled with test class and helper class")]
[SuppressMessage(
    "StyleCop.CSharp.OrderingRules",
    "SA1201:A enum should not follow a class",
    Justification = "Test enum is logically grouped with related test components")]
[SuppressMessage(
    "Minor Code Smell",
    "S2344:Enumeration type names should not have \"Flags\" or \"Enum\" suffixes",
    Justification = "Only for test purposes")]
internal enum TestEnum
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
        var enumEntity = Activator.CreateInstance(typeof(EnumEntity<TestEnum>), nonPublic: true) as EnumEntity<TestEnum>;

        // Assert
        enumEntity.Name.Should().Be("EF");
    }
}
