using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Lewee.Domain.Tests.Unit;

public partial class DomainEventReferenceTests
{
    public class TestingDomainEvent : DomainEvent
    {
        public TestingDomainEvent(
            string name,
            int count,
            DateTime createdAt,
            Guid correlationId)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Count = count;
            this.CreatedAt = createdAt;
            this.CorrelationId = correlationId;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonConstructor]
        public TestingDomainEvent(
            Guid id,
            string name,
            int count,
            DateTime createdAt,
            Guid correlationId)
        {
            this.Id = id;
            this.Name = name;
            this.Count = count;
            this.CreatedAt = createdAt;
            this.CorrelationId = correlationId;
        }

        public Guid Id { get; protected set; }

        public string Name { get; protected set; }

        public int Count { get; protected set; }

        public DateTime CreatedAt { get; protected set; }
    }
}
