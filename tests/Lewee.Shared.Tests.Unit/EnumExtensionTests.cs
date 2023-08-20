using FluentAssertions;
using Lewee.Shared;
using Xunit;

namespace Lewee.Util.Shared.Unit;

public static class EnumExtensionTests
{
    public enum TableStatus
    {
        Available = 0,

        Reserved = 1,

        [System.ComponentModel.Description("In Use")]
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
