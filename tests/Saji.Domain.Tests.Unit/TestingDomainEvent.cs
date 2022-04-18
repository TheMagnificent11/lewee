namespace Saji.Domain.Tests.Unit;

public partial class DomainEventReferenceTests
{
    public class TestingDomainEvent : IDomainEvent
    {
        public TestingDomainEvent(
            string name,
            int count,
            DateTime createdAt)
        {
            this.Id = Guid.NewGuid();
            this.CorrelationId = Guid.NewGuid();
            this.Name = name;
            this.Count = count;
            this.CreatedAt = createdAt;
        }

        public Guid Id { get; protected set; }

        public string Name { get; protected set; }

        public int Count { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public Guid CorrelationId { get; set; }
    }
}
