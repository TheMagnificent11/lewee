using FluentAssertions;
using Lewee.Common;
using Xunit;

namespace Lewee.Auth.Application.Tests.Unit;

public class AuthorizationResultFactoryTests
{
    [Fact]
    public void CreateFailure_ShouldCreateCommandResult()
    {
        var result = AuthorizationResultFactory.CreateFailure<CommandResult>(
            ResultStatus.Unauthorized,
            "Not authorized");

        result.Status.Should().Be(ResultStatus.Unauthorized);
        result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be("Not authorized");
    }

    [Fact]
    public void CreateFailure_ShouldCreateQueryResult()
    {
        var result = AuthorizationResultFactory.CreateFailure<QueryResult<string>>(
            ResultStatus.Unauthenticated,
            "Not authenticated");

        result.Status.Should().Be(ResultStatus.Unauthenticated);
        result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be("Not authenticated");
    }

    [Fact]
    public void CreateFailure_ShouldThrow_WhenResponseDoesNotExposeFailFactory()
    {
        var act = () => AuthorizationResultFactory.CreateFailure<ResultWithoutFailFactory>(
            ResultStatus.Unauthorized,
            "Not authorized");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*does not expose a static Fail*");
    }

    private sealed class ResultWithoutFailFactory : Result
    {
        public ResultWithoutFailFactory()
            : base(ResultStatus.Success, null)
        {
        }
    }
}
