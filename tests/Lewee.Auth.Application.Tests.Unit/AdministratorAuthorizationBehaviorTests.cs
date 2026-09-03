using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Lewee.Application.Mediation.Requests;
using Lewee.Auth.Domain;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Lewee.Auth.Application.Tests.Unit;

public class AdministratorAuthorizationBehaviorTests
{
    [Fact]
    public async Task Should_InvokeHandler_When_CallerIsSiteAdministratorAsync()
    {
        const string externalId = "external-id";
        var user = CreateUser(externalId, isSiteAdministrator: true);
        var behavior = CreateBehavior(externalId, user);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(new TestAdministratorCommand(), next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_CallerIsNotSiteAdministratorAsync()
    {
        const string externalId = "external-id";
        var user = CreateUser(externalId, isSiteAdministrator: false);
        var behavior = CreateBehavior(externalId, user);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(new TestAdministratorCommand(), next, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_CallerHasNoUserRecordAsync()
    {
        const string externalId = "external-id";
        var behavior = CreateBehavior(externalId, user: null);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(new TestAdministratorCommand(), next, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnUnauthenticated_When_NoCallerIsAuthenticatedAsync()
    {
        var behavior = CreateBehavior(externalId: null, user: null);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(new TestAdministratorCommand(), next, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthenticated);
        nextCalled.Should().BeFalse();
    }

    private static AdministratorAuthorizationBehavior<TestAdministratorCommand, CommandResult> CreateBehavior(
        string externalId,
        User user)
    {
        var authenticatedUserService = new Mock<IAuthenticatedUserService>();
        authenticatedUserService.SetupGet(item => item.UserId).Returns(externalId);

        var userRepository = new Mock<IRepository<User>>();
        userRepository
            .Setup(item => item.QueryOneAsync(
                It.IsAny<QuerySpecification<User>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        return new AdministratorAuthorizationBehavior<TestAdministratorCommand, CommandResult>(
            authenticatedUserService.Object,
            userRepository.Object,
            NullLogger<AdministratorAuthorizationBehavior<TestAdministratorCommand, CommandResult>>.Instance);
    }

    private static User CreateUser(string externalId, bool isSiteAdministrator)
    {
        var user = User.Create(externalId, Guid.NewGuid());

        if (isSiteAdministrator)
        {
            typeof(User)
                .GetProperty(nameof(User.IsSiteAdministrator), BindingFlags.Public | BindingFlags.Instance)
                .SetValue(user, true);
        }

        return user;
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model class grouped with test class for convenience")]
[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via mediation")]
internal sealed record TestAdministratorCommand : ICommand, IAdministratorRequest;
