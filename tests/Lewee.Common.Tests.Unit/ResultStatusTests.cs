using FluentAssertions;
using Xunit;

namespace Lewee.Common.Tests.Unit;

public class ResultStatusTests
{
    [Theory]
    [InlineData(ResultStatus.NotApplicable, 0)]
    [InlineData(ResultStatus.Success, 1)]
    [InlineData(ResultStatus.Unauthenticated, 2)]
    [InlineData(ResultStatus.Unauthorized, 3)]
    [InlineData(ResultStatus.BadRequest, 4)]
    [InlineData(ResultStatus.NotFound, 5)]
    public void ResultStatus_ShouldHaveCorrectIntValue(ResultStatus status, int expectedValue)
    {
        ((int)status).Should().Be(expectedValue);
    }

    [Fact]
    public void ResultStatus_ShouldHaveExactlyExpectedValues()
    {
        var values = Enum.GetValues<ResultStatus>();
        values.Should().HaveCount(6);
    }

    [Fact]
    public void NotApplicable_ShouldBeDefault()
    {
        default(ResultStatus).Should().Be(ResultStatus.NotApplicable);
    }
}
