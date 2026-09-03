using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Lewee.Application.Mediation.Requests;
using Lewee.Auth.Domain;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lewee.Auth.Application.Tests.Unit;

public class ApplicationAuthConfigurationTests
{
    [Fact]
    public void AddLeweeApplicationAuth_ShouldRegisterAuthorizationBehaviors()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<IAuthenticatedUserService>());
        services.AddSingleton(Mock.Of<IRepository<User>>());
        services.AddSingleton(Mock.Of<IQueryProjectionService>());

        services.AddLeweeApplicationAuth();
        var serviceProvider = services.BuildServiceProvider();

        var tenantRoleBehaviors = serviceProvider
            .GetServices<IPipelineBehavior<TestTenantRoleAuthConfigCommand, CommandResult>>()
            .ToList();
        tenantRoleBehaviors.Should().Contain(b => b.GetType().Name.Contains("TenantLoggingBehavior"));
        tenantRoleBehaviors.Should().Contain(b => b.GetType().Name.Contains("TenantRoleAuthorizationBehavior"));

        var administratorBehaviors = serviceProvider
            .GetServices<IPipelineBehavior<TestAdministratorAuthConfigCommand, CommandResult>>()
            .ToList();
        administratorBehaviors.Should().Contain(b => b.GetType().Name.Contains("AdministratorAuthorizationBehavior"));
    }

    [Fact]
    public void AddLeweeApplicationAuth_ShouldReturnServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddLeweeApplicationAuth();

        result.Should().BeSameAs(services);
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via mediation")]
internal sealed record TestTenantRoleAuthConfigCommand(Guid TenantId, IReadOnlyCollection<string> SatisfyingRoles)
    : ICommand, ITenantRoleRequest;

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via mediation")]
internal sealed record TestAdministratorAuthConfigCommand : ICommand, IAdministratorRequest;
