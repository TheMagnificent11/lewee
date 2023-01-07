using System.ComponentModel;
using FluentAssertions;
using Xunit;

namespace Lewee.Util.Tests.Unit;

public static class EnumExtensionTests
{
    public enum TableStatus
    {
        Available = 0,

        Reserved = 1,

        [Description("In Use")]
        InUse = 2
    }

    [Theory]
    [InlineData(TableStatus.Available, "Available")]
    [InlineData(TableStatus.Reserved, "Reserved")]
    [InlineData(TableStatus.InUse, "In Use")]
    public static void GetDescription_ReturnCorrectDescription(TableStatus value, string expectedDescription)
    {
        var result = value.GetDescription();

        result.Should().NotBeNull();
        result.Should().Be(expectedDescription);
    }
}
