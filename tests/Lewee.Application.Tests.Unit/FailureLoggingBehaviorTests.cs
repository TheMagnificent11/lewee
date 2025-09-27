using FluentAssertions;
using FreeMediator;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for FailureLoggingBehavior using direct behavior testing
/// </summary>
public class FailureLoggingBehaviorTests
{
    [Fact]
    public async Task FailureLoggingBehavior_WithSuccessfulExecution_ShouldCallNextAndNotLog()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<FailureLoggingBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new FailureLoggingBehavior<TestCommand, CommandResult>(logger);
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
    public async Task FailureLoggingBehavior_WithFailedResult_ShouldCallNextAndLogFailure()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<FailureLoggingBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new FailureLoggingBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test", Guid.NewGuid());
        var nextCalled = false;
        
        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Fail(ResultStatus.BadRequest, "Test failure"));
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task FailureLoggingBehavior_WithException_ShouldRethrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<FailureLoggingBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new FailureLoggingBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test", Guid.NewGuid());
        var exceptionMessage = "Test exception";
        
        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            throw new InvalidOperationException(exceptionMessage);
        };

        // Act & Assert
        var act = () => behavior.Handle(command, next, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(exceptionMessage);
    }
}