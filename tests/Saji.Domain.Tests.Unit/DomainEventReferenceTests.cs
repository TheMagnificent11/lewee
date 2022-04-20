using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Saji.Domain.Tests.Unit;

public partial class DomainEventReferenceTests
{
    [Fact]
    public static void Ctor()
    {
        // Arrange
        var domainEvent = new TestingDomainEvent(
            "hello",
            17,
            DateTime.UtcNow,
            Guid.NewGuid());
        var creationTime = DateTime.UtcNow;
        var expectedType = domainEvent.GetType();
        var expectedJson = JsonSerializer.Serialize(domainEvent);

        // Act
        var result = new DomainEventReference(domainEvent);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.DomainEventAssemblyName.Should().Be(expectedType.Assembly.GetName().Name);
        result.DomainEventClassName.Should().Be(expectedType.FullName);
        result.DomainEventJson.Should().Be(expectedJson);
        result.Dispatched.Should().BeFalse();
        result.PersistedAt.Should().BeAfter(creationTime);
        result.DispatchedAt.Should().BeNull();
    }

    [Fact]
    public static void ToDomainEvent()
    {
        // Arrange
        var domainEvent = new TestingDomainEvent(
            "hello",
            17,
            DateTime.UtcNow,
            Guid.NewGuid());
        var reference = new DomainEventReference(domainEvent);

        // Act
        var result = (TestingDomainEvent)reference.ToDomainEvent();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(domainEvent.Id);
        result.CorrelationId.Should().Be(domainEvent.CorrelationId);
        result.Name.Should().Be(domainEvent.Name);
        result.Count.Should().Be(domainEvent.Count);
        result.CreatedAt.Should().Be(domainEvent.CreatedAt);
    }
}
