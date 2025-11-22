using FluentAssertions;
using Xunit;

namespace Lewee.Shared.Tests.Unit;

public static class EnumExtensionTests
{
    public enum TableStatus
    {
        Available = 0,

        Booked = 1,

        [System.ComponentModel.Description("In Use")]
        InUse = 2
    }

    [Theory]
    [InlineData(TableStatus.Available, "Available")]
    [InlineData(TableStatus.Booked, "Booked")]
    [InlineData(TableStatus.InUse, "In Use")]
    public static void GetDescriptionReturnCorrectDescription(TableStatus value, string expectedDescription)
    {
        var result = value.GetDescription();

        result.Should().NotBeNull();
        result.Should().Be(expectedDescription);
    }
}
