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
        var result = reference.ToDomainEvent();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<TestingDomainEvent>();

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        var domainEventResult = (TestingDomainEvent)result;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        domainEventResult.Id.Should().Be(domainEvent.Id);
        domainEventResult.CorrelationId.Should().Be(domainEvent.CorrelationId);
        domainEventResult.Name.Should().Be(domainEvent.Name);
        domainEventResult.Count.Should().Be(domainEvent.Count);
        domainEventResult.CreatedAt.Should().Be(domainEvent.CreatedAt);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}
