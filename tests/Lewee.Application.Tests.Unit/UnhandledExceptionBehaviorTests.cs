using FluentAssertions;
using FreeMediator;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for UnhandledExceptionBehavior using direct behavior testing
/// </summary>
public class UnhandledExceptionBehaviorTests
{
    [Fact]
    public async Task UnhandledExceptionBehavior_WithNormalExecution_ShouldCallNext()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<UnhandledExceptionBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new UnhandledExceptionBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test", Guid.NewGuid());
        var nextCalled = false;
        
        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task UnhandledExceptionBehavior_WithException_ShouldRethrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<UnhandledExceptionBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new UnhandledExceptionBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test", Guid.NewGuid());
        var exceptionMessage = "Test unhandled exception";
        
        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            throw new InvalidOperationException(exceptionMessage);
        };

        // Act & Assert
        // UnhandledExceptionBehavior rethrows exceptions, it doesn't convert them to CommandResult
        var act = () => behavior.Handle(command, next, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(exceptionMessage);
    }
}