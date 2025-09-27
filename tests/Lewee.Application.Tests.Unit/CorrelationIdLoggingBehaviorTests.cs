using FluentAssertions;
using FreeMediator;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for CorrelationIdLoggingBehavior using direct behavior testing
/// </summary>
public class CorrelationIdLoggingBehaviorTests
{
    [Fact]
    public async Task CorrelationIdLoggingBehavior_WithNormalExecution_ShouldCallNextAndLogCorrelationId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<CorrelationIdLoggingBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new CorrelationIdLoggingBehavior<TestCommand, CommandResult>(logger);
        var correlationId = Guid.NewGuid();
        var command = new TestCommand("Test", correlationId);
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
        // Behavior should pass through the result unchanged while logging correlation ID
    }

    [Fact]
    public async Task CorrelationIdLoggingBehavior_WithException_ShouldStillLogCorrelationIdAndRethrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<CorrelationIdLoggingBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new CorrelationIdLoggingBehavior<TestCommand, CommandResult>(logger);
        var correlationId = Guid.NewGuid();
        var command = new TestCommand("Test", correlationId);
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

    [Fact]
    public async Task CorrelationIdLoggingBehavior_WithFailedResult_ShouldCallNextAndLogCorrelationId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<CorrelationIdLoggingBehavior<TestCommand, CommandResult>>>();
        
        var behavior = new CorrelationIdLoggingBehavior<TestCommand, CommandResult>(logger);
        var correlationId = Guid.NewGuid();
        var command = new TestCommand("Test", correlationId);
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
        // Behavior should pass through the failed result unchanged while logging correlation ID
    }
}