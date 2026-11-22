using Lewee.Domain;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

internal sealed class GetOrdersQuerySpec : QuerySpecification<Order>
{
    public GetOrdersQuerySpec(OrderListFilter filter)
    {
        if (filter == OrderListFilter.Past)
        {
            this.Query.Where(x => x.CompletedDateTime != null);
        }
        else
        {
            this.Query.Where(x => x.SubmittedDateTime != null && x.CompletedDateTime == null);
        }

        this.Query
            .Include(x => x.Pizzas)
            .ThenInclude<OrderPizza, Pizza>(x => x.Pizza);
    }
}
