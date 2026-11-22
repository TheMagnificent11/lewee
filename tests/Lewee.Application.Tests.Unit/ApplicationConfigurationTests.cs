using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentValidation;
using Lewee.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.Application.Tests.Unit;

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
        var act = () => services.AddPipelineBehaviors([]);
        act.Should().NotThrow();
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test helper class grouped with test class for convenience")]
[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via mediation/DI")]
internal sealed class TestCustomBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return next(cancellationToken);
    }
}
