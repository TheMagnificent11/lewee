using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Saji.Domain.Tests.Unit;

public class DomainEventReferenceTests
{
    public class TestingDomainEvent : IDomainEvent
    {
        public TestingDomainEvent(
            string name,
            int count,
            DateTime createdAt)
            : base()
        {
            this.Name = name;
            this.Count = count;
            this.CreatedAt = createdAt;
        }

        protected TestingDomainEvent()
        {
            this.Id = Guid.NewGuid();
            this.CorrelationId = Guid.NewGuid();
        }

        public Guid Id { get; protected set; }

        public string Name { get; protected set; }

        public int Count { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public Guid CorrelationId { get; set; }

        [Fact]
        public void ShouldCreateUsingFrom()
        {
            // Arrange
            var domainEvent = new TestingDomainEvent(
                "name",
                1,
                DateTime.UtcNow);
            var expectedType = domainEvent.GetType().FullName;
            var expectedJson = JsonSerializer.Serialize(domainEvent);
            var creationTime = DateTime.UtcNow;

            // Act
            var result = DomainEventReference.From(domainEvent);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.DomainEventType.Should().Be(expectedType);
            result.DomainEventJson.Should().Be(expectedJson);
            result.Dispatched.Should().BeFalse();
            result.PersistedAt.Should().BeAfter(creationTime);
            result.DispatchedAt.Should().BeNull();
        }
    }
}
