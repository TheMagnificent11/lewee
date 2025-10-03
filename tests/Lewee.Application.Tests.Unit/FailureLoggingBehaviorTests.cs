using FluentAssertions;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for FailureLoggingBehavior using direct behavior testing
/// </summary>
public class FailureLoggingBehaviorTests
{
    [Fact]
    public async Task FailureLoggingBehavior_WithSuccessfulExecution_ShouldCallNextAndNotLogAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<FailureLoggingBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

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

        // Should not log anything for successful execution
        fakeLogCollector.Count.Should().Be(0);
    }

    [Fact]
    public async Task FailureLoggingBehavior_WithFailedResult_ShouldCallNextAndLogFailureAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<FailureLoggingBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

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

        // Should log Information for BadRequest (status < 500)
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Information);
        logEntry.Message.Should().Contain("Bad request");
    }

    [Fact]
    public async Task FailureLoggingBehavior_WithException_ShouldRethrowAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<FailureLoggingBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

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

        // Should not log anything when exception is thrown (exception handling is done by UnhandledExceptionBehavior)
        fakeLogCollector.Count.Should().Be(0);
    }

    [Fact]
    public async Task FailureLoggingBehavior_WithServerErrorResult_ShouldLogAsErrorAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<FailureLoggingBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new FailureLoggingBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test", Guid.NewGuid());
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            // Create a result with status >= 500 to trigger error logging
            var serverErrorStatus = (ResultStatus)500;
            return Task.FromResult(CommandResult.Fail(serverErrorStatus, "Server error"));
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        nextCalled.Should().BeTrue();

        // Should log Error for status >= 500
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Error);
        logEntry.Message.Should().Contain("Unexpected error occurred");
    }
}
