using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Lewee.Infrastructure.Data.Tests.Integration;

/// <summary>
/// Test aggregate root for domain event testing
/// </summary>
internal sealed class TestOrder : AggregateRoot
{
    public TestOrder(Guid id, string orderNumber)
        : base(id)
    {
        this.OrderNumber = orderNumber;
    }

    [SuppressMessage(
        "Major Code Smell",
        "S1144:Unused private types or members should be removed",
        Justification = "Needed for EF")]
    private TestOrder()
        : base(Guid.NewGuid())
    {
        this.OrderNumber = string.Empty;
    }

    public string OrderNumber { get; private set; }

    public void Submit(Guid correlationId)
    {
        this.DomainEvents.Raise(new TestOrderSubmittedEvent(correlationId, this.Id));
    }
}
