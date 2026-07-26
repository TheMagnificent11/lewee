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
/// Tests for TenantLoggingBehavior using direct behavior testing
/// </summary>
public class TenantLoggingBehaviorTests
{
    [Fact]
    public async Task TenantLoggingBehavior_WithTenantedRequest_ShouldCallNextAndLogTenantIdAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TenantLoggingBehavior<TestTenantCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new TenantLoggingBehavior<TestTenantCommand, CommandResult>(logger);
        var tenantId = Guid.NewGuid();
        var command = new TestTenantCommand(tenantId, "Test");
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            // Log something within the scope to test tenant ID scope
            logger.LogInformation("Test log within tenant scope");
            return Task.FromResult(CommandResult.Success());
        };

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
        logEntry.Message.Should().Contain("Test log within tenant scope");

        // Should have tenant ID added to logging scope
        logEntry.Scopes.Should().NotBeEmpty("because TenantLoggingBehavior should add tenant ID to logging scope");

        // Assert that the tenant ID value is correctly set in the scope
        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();
        var tenantIdScope = scopeDict.FirstOrDefault(kvp => kvp.Key == LoggingConsts.TenantId);
        tenantIdScope.Should().NotBeNull();
        tenantIdScope.Value.ToString().Should().Be(tenantId.ToString());
    }

    [Fact]
    public async Task TenantLoggingBehavior_WithException_ShouldStillLogTenantIdAndRethrowAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TenantLoggingBehavior<TestTenantCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new TenantLoggingBehavior<TestTenantCommand, CommandResult>(logger);
        var tenantId = Guid.NewGuid();
        var command = new TestTenantCommand(tenantId, "Test");
        var exceptionMessage = "Test exception";

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            // Log something before throwing to test tenant ID scope
            logger.LogError("Test error log before exception");
            throw new InvalidOperationException(exceptionMessage);
        };

        // Act & Assert
        var act = () => behavior.Handle(command, next, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(exceptionMessage);

        // Should have the log message from before the exception
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Error);
        logEntry.Message.Should().Contain("Test error log before exception");

        // Should have tenant ID added to logging scope even when exception occurs
        logEntry.Scopes.Should().NotBeEmpty("because TenantLoggingBehavior should add tenant ID to logging scope");

        // Assert that the tenant ID value is correctly set in the scope
        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();
        var tenantIdScope = scopeDict.FirstOrDefault(kvp => kvp.Key == LoggingConsts.TenantId);
        tenantIdScope.Should().NotBeNull();
        tenantIdScope.Value.ToString().Should().Be(tenantId.ToString());
    }

    [Fact]
    public async Task TenantLoggingBehavior_WithFailedResult_ShouldCallNextAndLogTenantIdAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TenantLoggingBehavior<TestTenantCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new TenantLoggingBehavior<TestTenantCommand, CommandResult>(logger);
        var tenantId = Guid.NewGuid();
        var command = new TestTenantCommand(tenantId, "Test");
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            // Log something within the scope to test tenant ID scope
            logger.LogWarning("Test warning log within tenant scope");
            return Task.FromResult(CommandResult.Fail(ResultStatus.BadRequest, "Test failure"));
        };

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
        logEntry.Message.Should().Contain("Test warning log within tenant scope");

        // Should have tenant ID added to logging scope even with failed result
        logEntry.Scopes.Should().NotBeEmpty("because TenantLoggingBehavior should add tenant ID to logging scope");

        // Assert that the tenant ID value is correctly set in the scope
        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();
        var tenantIdScope = scopeDict.FirstOrDefault(kvp => kvp.Key == LoggingConsts.TenantId);
        tenantIdScope.Should().NotBeNull();
        tenantIdScope.Value.ToString().Should().Be(tenantId.ToString());
    }
}
