using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Saji.Domain.Tests.Unit;

public partial class DomainEventReferenceTests
{
    [Fact]
    public static void ShouldCreateAppropriateObject()
    {
        // Arrange
        var domainEvent = new TestingDomainEvent(
            "name",
            1,
            DateTime.UtcNow);
        var creationTime = DateTime.UtcNow;
        var expectedType = domainEvent.GetType();

        // Act
        var result = DomainEventReference.From(domainEvent);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.DomainEventAssemblyName.Should().Be(expectedType.Assembly.GetName().Name);
        result.DomainEventClassName.Should().Be(expectedType.FullName);
        result.Dispatched.Should().BeFalse();
        result.PersistedAt.Should().BeAfter(creationTime);
        result.DispatchedAt.Should().BeNull();

        // Re-act
        var derserializedDomainEvent = (TestingDomainEvent)result.ToDomainEvent();

        // Assert
        derserializedDomainEvent.Should().NotBeNull();
        derserializedDomainEvent.Id.Should().Be(domainEvent.Id);
        derserializedDomainEvent.CorrelationId.Should().Be(domainEvent.CorrelationId);
        derserializedDomainEvent.Name.Should().Be(domainEvent.Name);
        derserializedDomainEvent.Count.Should().Be(domainEvent.Count);
        derserializedDomainEvent.CreatedAt.Should().Be(domainEvent.CreatedAt);
    }
}
