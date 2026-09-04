using System.Diagnostics.CodeAnalysis;
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

public class TenantRoleAuthorizationBehaviorTests
{
    [Fact]
    public async Task Should_InvokeHandler_When_CallerHoldsASatisfyingRoleAsync()
    {
        var tenantId = Guid.NewGuid();
        const string externalId = "external-id";
        var projection = new TenantMembershipRolesQueryProjection { IsMember = true, RoleCodes = ["Manager"] };
        var behavior = CreateBehavior(externalId, tenantId, projection);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(
            new TestTenantRoleCommand(tenantId, ["Manager", "Owner"]),
            next,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_CallerIsNotAMemberOfTheTenantAsync()
    {
        var tenantId = Guid.NewGuid();
        const string externalId = "external-id";
        var behavior = CreateBehavior(externalId, tenantId, projection: null);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(
            new TestTenantRoleCommand(tenantId, ["Manager"]),
            next,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_CallerHoldsNoneOfTheRequiredRolesAsync()
    {
        var tenantId = Guid.NewGuid();
        const string externalId = "external-id";
        var projection = new TenantMembershipRolesQueryProjection { IsMember = true, RoleCodes = ["Staff"] };
        var behavior = CreateBehavior(externalId, tenantId, projection);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(
            new TestTenantRoleCommand(tenantId, ["Manager", "Owner"]),
            next,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnUnauthenticated_When_NoCallerIsAuthenticatedAsync()
    {
        var tenantId = Guid.NewGuid();
        var behavior = CreateBehavior(externalId: null, tenantId, projection: null);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(
            new TestTenantRoleCommand(tenantId, ["Manager"]),
            next,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthenticated);
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_InvokeHandler_When_CallerIsASiteAdministratorAsync()
    {
        var tenantId = Guid.NewGuid();
        const string externalId = "external-id";

        // No tenant membership/roles at all - the site administrator override should still allow access.
        var behavior = CreateBehavior(externalId, tenantId, projection: null, isSiteAdministrator: true);
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        var result = await behavior.Handle(
            new TestTenantRoleCommand(tenantId, ["Manager"]),
            next,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();
    }

    private static TenantRoleAuthorizationBehavior<TestTenantRoleCommand, CommandResult> CreateBehavior(
        string externalId,
        Guid tenantId,
        TenantMembershipRolesQueryProjection projection,
        bool isSiteAdministrator = false)
    {
        var authenticatedUserService = new Mock<IAuthenticatedUserService>();
        authenticatedUserService.SetupGet(item => item.UserId).Returns(externalId);

        var userRepository = new Mock<IRepository<User>>();

        if (externalId != null)
        {
            var user = User.Create(externalId, Guid.NewGuid());
            user.IsSiteAdministrator = isSiteAdministrator;

            userRepository
                .Setup(item => item.QueryOneAsync(
                    It.Is<UserByExternalIdSpecification>(spec => spec.ExternalId == externalId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
        }

        var queryProjectionService = new Mock<IQueryProjectionService>();

        if (externalId != null && !isSiteAdministrator)
        {
            queryProjectionService
                .Setup(item => item.RetrieveByKeyAsync<TenantMembershipRolesQueryProjection>(
                    TenantMembershipRolesQueryProjection.BuildKey(tenantId, externalId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(projection);
        }

        return new TenantRoleAuthorizationBehavior<TestTenantRoleCommand, CommandResult>(
            authenticatedUserService.Object,
            userRepository.Object,
            queryProjectionService.Object,
            NullLogger<TenantRoleAuthorizationBehavior<TestTenantRoleCommand, CommandResult>>.Instance);
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
internal sealed record TestTenantRoleCommand(Guid TenantId, IReadOnlyCollection<string> Roles)
    : ICommand, ITenantRoleRequest;
