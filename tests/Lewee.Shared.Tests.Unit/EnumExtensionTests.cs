using FluentAssertions;
using Lewee.Shared;
using NUnit.Framework;

namespace Lewee.Util.Shared.Unit;

[TestFixture]
public static class EnumExtensionTests
{
    public enum TableStatus
    {
        Available = 0,

        Reserved = 1,

        [System.ComponentModel.Description("In Use")]
        InUse = 2
    }

    [Test]
    [TestCase(TableStatus.Available, "Available")]
    [TestCase(TableStatus.Reserved, "Reserved")]
    [TestCase(TableStatus.InUse, "In Use")]
    public static void GetDescription_ReturnCorrectDescription(TableStatus value, string expectedDescription)
    {
        var result = value.GetDescription();

        result.Should().NotBeNull();
        result.Should().Be(expectedDescription);
    }
}
