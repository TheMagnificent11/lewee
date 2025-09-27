using System.Reflection;
using FluentAssertions;
using FluentValidation;
using FreeMediator;
using Lewee.Application;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for ApplicationConfiguration extension methods
/// </summary>
public class ApplicationConfigurationTests
{
    [Fact]
    public void AddApplication_ShouldRegisterMediatorAndValidators()
    {
        // Arrange
        var services = new ServiceCollection();
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;

        // Act
        services.AddApplication(applicationAssembly, domainAssembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IMediator>().Should().NotBeNull();
        serviceProvider.GetService<IValidator<TestCommand>>().Should().NotBeNull();
    }

    [Fact]
    public void AddApplication_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;

        // Act
        var result = services.AddApplication(applicationAssembly, domainAssembly);

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddPipelineBehaviors_ShouldRegisterAllDefaultBehaviors()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services for behaviors

        // Act
        services.AddPipelineBehaviors();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TestCommand, CommandResult>>().ToList();
        
        behaviors.Should().Contain(b => b.GetType().Name.Contains("CorrelationIdLoggingBehavior"));
        behaviors.Should().Contain(b => b.GetType().Name.Contains("PerformanceBehavior"));
        behaviors.Should().Contain(b => b.GetType().Name.Contains("FailureLoggingBehavior"));
        behaviors.Should().Contain(b => b.GetType().Name.Contains("UnhandledExceptionBehavior"));
        behaviors.Should().Contain(b => b.GetType().Name.Contains("DomainExceptionBehavior"));
        behaviors.Should().Contain(b => b.GetType().Name.Contains("ValidationBehavior"));
    }

    [Fact]
    public void AddPipelineBehaviors_WithAdditionalBehaviors_ShouldRegisterAllBehaviors()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services for behaviors
        var additionalBehavior = typeof(TestCustomBehavior<,>);

        // Act
        services.AddPipelineBehaviors(additionalBehavior);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TestCommand, CommandResult>>().ToList();
        behaviors.Should().Contain(b => b.GetType().Name.Contains("TestCustomBehavior"));
    }

    [Fact]
    public void AddPipelineBehaviors_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddPipelineBehaviors();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddPipelineBehaviors_WithNullAdditionalBehaviors_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var act = () => services.AddPipelineBehaviors(null);
        act.Should().NotThrow();
    }

    [Fact]
    public void AddPipelineBehaviors_WithEmptyAdditionalBehaviors_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var act = () => services.AddPipelineBehaviors(Array.Empty<Type>());
        act.Should().NotThrow();
    }
}

/// <summary>
/// Test custom behavior for testing additional behaviors registration
/// </summary>
public class TestCustomBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return next(cancellationToken);
    }
}