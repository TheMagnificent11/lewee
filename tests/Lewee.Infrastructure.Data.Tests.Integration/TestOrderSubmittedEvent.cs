using Lewee.Domain;

namespace Lewee.Infrastructure.Data.Tests.Integration;

/// <summary>
/// Test domain event for integration testing
/// </summary>
internal sealed class TestOrderSubmittedEvent : DomainEvent
{
    public TestOrderSubmittedEvent(Guid correlationId, Guid orderId)
        : base(correlationId)
    {
        this.OrderId = orderId;
    }

    public Guid OrderId { get; }
}
