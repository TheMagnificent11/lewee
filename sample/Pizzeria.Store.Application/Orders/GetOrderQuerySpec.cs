using Ardalis.Specification;
using Lewee.Domain;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

internal sealed class GetOrderQuerySpec : QuerySpecification<Order>
{
    public GetOrderQuerySpec(Guid orderId)
    {
        this.Query.Where(x => x.Id == orderId)
            .Include(x => x.Pizzas);
    }
}
