using FluentAssertions;
using Xunit;

namespace Pizzeria.Store.Domain.Tests;

public sealed class UserTests
{
    [Fact]
    public void Create_Should_Create_Valid_User()
    {
        // Arrange
        var externalId = Guid.NewGuid().ToString();
        var correlationId = Guid.NewGuid();

        // Act
        var user = User.Create(externalId, correlationId);

        // Assert
        user.Id.Should().NotBeEmpty();
        user.ExternalId.Should().Be(externalId);
    }

    [Fact]
    public void Create_Should_Raise_UserCreatedEvent()
    {
        // Arrange
        var externalId = Guid.NewGuid().ToString();
        var correlationId = Guid.NewGuid();

        // Act
        var user = User.Create(externalId, correlationId);
        var events = user.DomainEvents.GetAndClear();

        // Assert
        events.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedEvent>()
            .Which.Should().Match<UserCreatedEvent>(e =>
                e.UserEntityId == user.Id &&
                e.ExternalId == externalId &&
                e.CorrelationId == correlationId);
    }
}
