using FluentAssertions;
using FreeMediator;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for UnhandledExceptionBehavior using direct behavior testing
/// </summary>
public class UnhandledExceptionBehaviorTests
{
    [Fact]
    public async Task UnhandledExceptionBehavior_WithNormalExecution_ShouldCallNextAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<UnhandledExceptionBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

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

        // Should not log anything for successful execution
        fakeLogCollector.Count.Should().Be(0);
    }

    [Fact]
    public async Task UnhandledExceptionBehavior_WithException_ShouldLogErrorAndRethrowAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<UnhandledExceptionBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

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

        // Should log error message when exception occurs
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Error);
        logEntry.Message.Should().Contain("Unhandled Exception for Request");
        logEntry.Message.Should().Contain("TestCommand");
        logEntry.Exception.Should().BeOfType<InvalidOperationException>();
        logEntry.Exception!.Message.Should().Be(exceptionMessage);
    }
}
