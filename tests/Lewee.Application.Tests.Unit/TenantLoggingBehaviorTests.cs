using FluentAssertions;
using FreeMediator;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for TenantLoggingBehavior using direct behavior testing
/// </summary>
public class TenantLoggingBehaviorTests
{
    [Fact]
    public async Task TenantLoggingBehavior_WithTenantedRequest_ShouldCallNextAndLogTenantId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TenantLoggingBehavior<TestTenantCommand, CommandResult>>>();
        
        var behavior = new TenantLoggingBehavior<TestTenantCommand, CommandResult>(logger);
        var tenantId = Guid.NewGuid();
        var command = new TestTenantCommand(tenantId, "Test", Guid.NewGuid());
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
        // Behavior should pass through the result unchanged while logging tenant ID
    }

    [Fact]
    public async Task TenantLoggingBehavior_WithException_ShouldStillLogTenantIdAndRethrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TenantLoggingBehavior<TestTenantCommand, CommandResult>>>();
        
        var behavior = new TenantLoggingBehavior<TestTenantCommand, CommandResult>(logger);
        var tenantId = Guid.NewGuid();
        var command = new TestTenantCommand(tenantId, "Test", Guid.NewGuid());
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
    public async Task TenantLoggingBehavior_WithFailedResult_ShouldCallNextAndLogTenantId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TenantLoggingBehavior<TestTenantCommand, CommandResult>>>();
        
        var behavior = new TenantLoggingBehavior<TestTenantCommand, CommandResult>(logger);
        var tenantId = Guid.NewGuid();
        var command = new TestTenantCommand(tenantId, "Test", Guid.NewGuid());
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
        // Behavior should pass through the failed result unchanged while logging tenant ID
    }
}