using Lewee.Domain;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Domain;

public class OrderQueryProjection : IQueryProjection
{
    public Guid CorrelationId { get; init; }
    public OrderDto Order { get; init; }
}
