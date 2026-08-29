namespace Lewee.Domain.Tests.Unit;

internal sealed class TestingDomainEvent : DomainEvent
{
    public TestingDomainEvent(
        string name,
        int count,
        DateTime createdAt,
        Guid correlationId)
        : base(correlationId)
    {
        this.Name = name;
        this.Count = count;
        this.CreatedAt = createdAt;
        this.CorrelationId = correlationId;
    }

    public string Name { get; }

    public int Count { get; }

    public DateTime CreatedAt { get; }
}
