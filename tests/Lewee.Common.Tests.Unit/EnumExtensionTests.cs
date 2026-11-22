using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Lewee.Common.Tests.Unit;

public static class EnumExtensionTests
{
    [SuppressMessage(
        "Maintainability",
        "CA1515:Consider making public types internal",
        Justification = "False positive (needs to be public because it's used in a public method below)")]
    public enum TableStatus
    {
        Available = 0,

        Booked = 1,

        [Description("In Use")]
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
