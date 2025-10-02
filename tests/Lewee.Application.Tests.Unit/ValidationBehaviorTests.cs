using FluentAssertions;
using FluentValidation;
using FreeMediator;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for ValidationBehavior using direct behavior testing
/// </summary>
public class ValidationBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_WithValidCommand_ShouldCallNextAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<IValidator<TestCommand>, TestCommand.Validator>();

        var serviceProvider = services.BuildServiceProvider();
        var validators = serviceProvider.GetServices<IValidator<TestCommand>>();

        var behavior = new ValidationBehavior<TestCommand, CommandResult>(validators);
        var validCommand = new TestCommand("Valid Name", Guid.NewGuid());
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        // Act
        var result = await behavior.Handle(validCommand, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task ValidationBehavior_WithInvalidCommand_ShouldReturnValidationErrorsAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<IValidator<TestCommand>, TestCommand.Validator>();

        var serviceProvider = services.BuildServiceProvider();
        var validators = serviceProvider.GetServices<IValidator<TestCommand>>();

        var behavior = new ValidationBehavior<TestCommand, CommandResult>(validators);
        var invalidCommand = new TestCommand(string.Empty, Guid.NewGuid()); // Empty name should fail validation
        var nextCalled = false;

        RequestHandlerDelegate<CommandResult> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(CommandResult.Success());
        };

        // Act
        var result = await behavior.Handle(invalidCommand, next, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        nextCalled.Should().BeFalse(); // Next should not be called when validation fails
    }

    [Fact]
    public async Task ValidationBehavior_WithNoValidators_ShouldCallNextAsync()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestCommand, CommandResult>(Enumerable.Empty<IValidator<TestCommand>>());
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
}
