using FluentAssertions;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for DomainExceptionBehavior using direct behavior testing
/// </summary>
public class DomainExceptionBehaviorTests
{
    [Fact]
    public async Task DomainExceptionBehavior_WithNormalExecution_ShouldCallNextAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<DomainExceptionBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new DomainExceptionBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test");
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
    public async Task DomainExceptionBehavior_WithDomainException_ShouldCatchAndReturnBadRequestAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<DomainExceptionBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new DomainExceptionBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test");
        var exceptionMessage = "Test domain exception";

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            throw new DomainException(exceptionMessage);
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Errors.Should().NotBeEmpty();
        result.Errors[0].ErrorMessage.Should().Be(exceptionMessage);

        // Should log Information message when domain exception is caught
        fakeLogCollector.Count.Should().Be(1);
        var logEntry = fakeLogCollector.GetSnapshot().Single();
        logEntry.Level.Should().Be(LogLevel.Information);
        logEntry.Message.Should().Contain("Domain exception caught");
        logEntry.Exception.Should().BeOfType<DomainException>();
        logEntry.Exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task DomainExceptionBehavior_WithNonDomainException_ShouldRethrowAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<DomainExceptionBehavior<TestCommand, CommandResult>>>();
        var fakeLogCollector = serviceProvider.GetRequiredService<FakeLogCollector>();

        var behavior = new DomainExceptionBehavior<TestCommand, CommandResult>(logger);
        var command = new TestCommand("Test");
        var exceptionMessage = "Test regular exception";

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            throw new InvalidOperationException(exceptionMessage);
        };

        // Act & Assert
        var act = () => behavior.Handle(command, next, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(exceptionMessage);

        // Should not log anything for non-domain exceptions (they pass through)
        fakeLogCollector.Count.Should().Be(0);
    }
}
