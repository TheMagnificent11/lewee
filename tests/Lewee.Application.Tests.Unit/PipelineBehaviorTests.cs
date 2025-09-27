using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for pipeline behaviors integration and service registration
/// </summary>
public class PipelineBehaviorTests
{
    [Fact]
    public void ServiceProvider_ShouldHaveApplicationServicesRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
        
        services.AddApplication(applicationAssembly, domainAssembly);
        services.AddPipelineBehaviors();
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        serviceProvider.GetService<FreeMediator.IMediator>().Should().NotBeNull();
        serviceProvider.GetService<FluentValidation.IValidator<TestCommand>>().Should().NotBeNull();
    }

    [Fact]
    public void ServiceProvider_ShouldResolveBehaviorsWithoutError()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
        
        services.AddApplication(applicationAssembly, domainAssembly);
        services.AddPipelineBehaviors();
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var behaviors = serviceProvider.GetServices<FreeMediator.IPipelineBehavior<TestCommand, Lewee.Application.Mediation.Requests.CommandResult>>();
        behaviors.Should().NotBeEmpty();
    }

    [Fact]
    public void ServiceProvider_ShouldResolveQueryHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
        
        services.AddApplication(applicationAssembly, domainAssembly);
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var handler = serviceProvider.GetService<FreeMediator.IRequestHandler<TestQuery, Lewee.Application.Mediation.Requests.QueryResult<TestData>>>();
        handler.Should().NotBeNull();
    }
}