using Lewee.Domain;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Domain;

public class OrderQueryProjection : IQueryProjection
{
    public Guid CorrelationId { get; set; }
    public OrderDto Order { get; set; } = null!;
}
