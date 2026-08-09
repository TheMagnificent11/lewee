using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public static class DomainEventReferenceTests
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

        var domainEventResult = (TestingDomainEvent)result;
        domainEventResult.CorrelationId.Should().Be(domainEvent.CorrelationId);
        domainEventResult.Name.Should().Be(domainEvent.Name);
        domainEventResult.Count.Should().Be(domainEvent.Count);
        domainEventResult.CreatedAt.Should().Be(domainEvent.CreatedAt);
    }

    [Fact]
    public static void Ctor_WithUserId_SetsUserId()
    {
        // Arrange
        var domainEvent = new TestingDomainEvent(
            "test",
            1,
            DateTime.UtcNow,
            Guid.NewGuid());
        var userId = "user123";

        // Act
        var result = new DomainEventReference(domainEvent, userId);

        // Assert
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public static void Dispatch_MarksAsDispatched()
    {
        // Arrange
        var domainEvent = new TestingDomainEvent(
            "test",
            1,
            DateTime.UtcNow,
            Guid.NewGuid());
        var reference = new DomainEventReference(domainEvent);
        var beforeDispatch = DateTime.UtcNow;

        // Act
        reference.Dispatch();

        // Assert
        reference.Dispatched.Should().BeTrue();
        reference.DispatchedAt.Should().NotBeNull();
        reference.DispatchedAt.Should().BeOnOrAfter(beforeDispatch);
    }

    [Fact]
    public static void ToDomainEvent_SetsUserId()
    {
        // Arrange
        var domainEvent = new TestingDomainEvent(
            "test",
            1,
            DateTime.UtcNow,
            Guid.NewGuid());
        var userId = "user456";
        var reference = new DomainEventReference(domainEvent, userId);

        // Act
        var result = reference.ToDomainEvent();

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
    }
}
