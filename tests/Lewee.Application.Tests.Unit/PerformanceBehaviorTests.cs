using FluentAssertions;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for PerformanceBehavior using direct behavior testing
/// </summary>
public class PerformanceBehaviorTests
{
    [Fact]
    public async Task PerformanceBehavior_WithNormalExecution_ShouldCallNextAndLogTimingAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<PerformanceBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new PerformanceBehavior<TestCommand, CommandResult>(logger);
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

        // Should log beginning and completion messages
        fakeLogCollector.Count.Should().Be(2);
        var logEntries = fakeLogCollector.GetSnapshot().ToList();

        logEntries[0].Level.Should().Be(LogLevel.Information);
        logEntries[0].Message.Should().Contain("Beginning operation");
        logEntries[0].Message.Should().Contain("TestCommand Handler");

        logEntries[1].Level.Should().Be(LogLevel.Information);
        logEntries[1].Message.Should().Contain("Completed operation");
        logEntries[1].Message.Should().Contain("TestCommand Handler");
        logEntries[1].Message.Should().Contain("ms");
    }

    [Fact]
    public async Task PerformanceBehavior_WithSlowExecution_ShouldCallNextAndLogTimingAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<PerformanceBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new PerformanceBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test", Guid.NewGuid());
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = async (ct) =>
        {
            await Task.Delay(10, ct); // Small delay to simulate work
            nextCalled = true;
            return CommandResult.Success();
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();

        // Should log beginning and completion messages with timing
        fakeLogCollector.Count.Should().Be(2);
        var logEntries = fakeLogCollector.GetSnapshot().ToList();

        logEntries[0].Level.Should().Be(LogLevel.Information);
        logEntries[0].Message.Should().Contain("Beginning operation");

        logEntries[1].Level.Should().Be(LogLevel.Information);
        logEntries[1].Message.Should().Contain("Completed operation");
        logEntries[1].Message.Should().Contain("ms");

        // The exact timing is hard to parse from formatted logs, but we know it took at least 10ms
        // The important part is that timing messages are logged correctly
    }

    [Fact]
    public async Task PerformanceBehavior_WithException_ShouldStillLogTimingAndRethrowAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<PerformanceBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new PerformanceBehavior<TestCommand, CommandResult>(logger);
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

        // Should log beginning and completion messages even when exception occurs
        fakeLogCollector.Count.Should().Be(2);
        var logEntries = fakeLogCollector.GetSnapshot().ToList();

        logEntries[0].Level.Should().Be(LogLevel.Information);
        logEntries[0].Message.Should().Contain("Beginning operation");

        logEntries[1].Level.Should().Be(LogLevel.Information);
        logEntries[1].Message.Should().Contain("Completed operation");
        logEntries[1].Message.Should().Contain("ms");
    }
}
