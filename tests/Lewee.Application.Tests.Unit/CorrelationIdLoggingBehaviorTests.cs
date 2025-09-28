using FluentAssertions;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
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
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<CorrelationIdLoggingBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();
        
        var behavior = new CorrelationIdLoggingBehavior<TestCommand, CommandResult>(logger);
        var correlationId = Guid.NewGuid();
        var command = new TestCommand("Test", correlationId);
        var nextCalled = false;

        Task<CommandResult> next(CancellationToken ct = default)
        {
            nextCalled = true;
            // Log something within the scope to test correlation ID scope
            logger.LogInformation("Test log within correlation scope");
            return Task.FromResult(CommandResult.Success());
        }

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();
        
        // Should have one log message from within the scope
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Information);
        logEntry.Message.Should().Contain("Test log within correlation scope");
        
        // Should have correlation ID added to logging scope
        logEntry.Scopes.Should().NotBeEmpty("because CorrelationIdLoggingBehavior should add correlation ID to logging scope");
        
        // Assert that the correlation ID value is correctly set in the scope
        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();
        var correlationIdScope = scopeDict.FirstOrDefault(kvp => kvp.Key == LoggingConsts.CorrelationId);
        correlationIdScope.Should().NotBeNull();
        correlationIdScope.Value.ToString().Should().Be(correlationId.ToString());
    }

    [Fact]
    public async Task CorrelationIdLoggingBehavior_WithException_ShouldStillLogCorrelationIdAndRethrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<CorrelationIdLoggingBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();
        
        var behavior = new CorrelationIdLoggingBehavior<TestCommand, CommandResult>(logger);
        var correlationId = Guid.NewGuid();
        var command = new TestCommand("Test", correlationId);
        var exceptionMessage = "Test exception";

        Task<CommandResult> next(CancellationToken ct = default)
        {
            // Log something before throwing to test correlation ID scope
            logger.LogInformation("Test log before exception");
            throw new InvalidOperationException(exceptionMessage);
        }

        // Act & Assert
        var act = () => behavior.Handle(command, next, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(exceptionMessage);
            
        // Should have the log message from before the exception
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Information);
        logEntry.Message.Should().Contain("Test log before exception");
        
        // Should have correlation ID added to logging scope even when exception occurs
        logEntry.Scopes.Should().NotBeEmpty("because CorrelationIdLoggingBehavior should add correlation ID to logging scope");
        
        // Assert that the correlation ID value is correctly set in the scope
        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();
        var correlationIdScope = scopeDict.FirstOrDefault(kvp => kvp.Key == LoggingConsts.CorrelationId);
        correlationIdScope.Should().NotBeNull();
        correlationIdScope.Value.ToString().Should().Be(correlationId.ToString());
    }

    [Fact]
    public async Task CorrelationIdLoggingBehavior_WithFailedResult_ShouldCallNextAndLogCorrelationId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<CorrelationIdLoggingBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();
        
        var behavior = new CorrelationIdLoggingBehavior<TestCommand, CommandResult>(logger);
        var correlationId = Guid.NewGuid();
        var command = new TestCommand("Test", correlationId);
        var nextCalled = false;

        Task<CommandResult> next(CancellationToken ct = default)
        {
            nextCalled = true;
            // Log something within the scope to test correlation ID scope
            logger.LogWarning("Test warning log within scope");
            return Task.FromResult(CommandResult.Fail(ResultStatus.BadRequest, "Test failure"));
        }

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        nextCalled.Should().BeTrue();
        
        // Should have one log message from within the scope
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Warning);
        logEntry.Message.Should().Contain("Test warning log within scope");
        
        // Should have correlation ID added to logging scope even with failed result
        logEntry.Scopes.Should().NotBeEmpty("because CorrelationIdLoggingBehavior should add correlation ID to logging scope");
        
        // Assert that the correlation ID value is correctly set in the scope
        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();
        var correlationIdScope = scopeDict.FirstOrDefault(kvp => kvp.Key == LoggingConsts.CorrelationId);
        correlationIdScope.Should().NotBeNull();
        correlationIdScope.Value.ToString().Should().Be(correlationId.ToString());
    }
}
