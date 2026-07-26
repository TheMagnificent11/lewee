using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class DomainEventsCollectionTests
{
    [Fact]
    public void Raise_AddsDomainEvent()
    {
        // Arrange
        var collection = new DomainEventsCollection();
        var domainEvent = new TestDomainEvent(Guid.NewGuid());

        // Act
        collection.Raise(domainEvent);
        var events = collection.GetAndClear();

        // Assert
        events.Should().HaveCount(1);
        events[0].Should().BeSameAs(domainEvent);
    }

    [Fact]
    public void Raise_MultipleTimes_AddsAllEvents()
    {
        // Arrange
        var collection = new DomainEventsCollection();
        var event1 = new TestDomainEvent(Guid.NewGuid());
        var event2 = new TestDomainEvent(Guid.NewGuid());
        var event3 = new TestDomainEvent(Guid.NewGuid());

        // Act
        collection.Raise(event1);
        collection.Raise(event2);
        collection.Raise(event3);
        var events = collection.GetAndClear();

        // Assert
        events.Should().HaveCount(3);
        events[0].Should().BeSameAs(event1);
        events[1].Should().BeSameAs(event2);
        events[2].Should().BeSameAs(event3);
    }

    [Fact]
    public void GetAndClear_EmptyCollection_ReturnsEmptyArray()
    {
        // Arrange
        var collection = new DomainEventsCollection();

        // Act
        var events = collection.GetAndClear();

        // Assert
        events.Should().BeEmpty();
    }

    [Fact]
    public void GetAndClear_ClearsCollection()
    {
        // Arrange
        var collection = new DomainEventsCollection();
        var domainEvent = new TestDomainEvent(Guid.NewGuid());
        collection.Raise(domainEvent);

        // Act
        var events1 = collection.GetAndClear();
        var events2 = collection.GetAndClear();

        // Assert
        events1.Should().HaveCount(1);
        events2.Should().BeEmpty();
    }

    [Fact]
    public void GetAndClear_ReturnsArrayCopy()
    {
        // Arrange
        var collection = new DomainEventsCollection();
        var domainEvent = new TestDomainEvent(Guid.NewGuid());
        collection.Raise(domainEvent);

        // Act
        var events1 = collection.GetAndClear();
        var events2 = collection.GetAndClear();

        // Assert
        events1.Should().NotBeSameAs(events2);
    }

    [Fact]
    public async Task Raise_ConcurrentCalls_AllEventsAreAddedAsync()
    {
        // Arrange
        var collection = new DomainEventsCollection();
        var tasks = new List<Task>();
        var eventCount = 100;

        // Act
        for (int i = 0; i < eventCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var domainEvent = new TestDomainEvent(Guid.NewGuid());
                collection.Raise(domainEvent);
            }));
        }

        await Task.WhenAll(tasks);
        var events = collection.GetAndClear();

        // Assert
        events.Should().HaveCount(eventCount);
    }

    private sealed class TestDomainEvent : DomainEvent
    {
        public TestDomainEvent(Guid correlationId)
            : base(correlationId)
        {
        }
    }
}
