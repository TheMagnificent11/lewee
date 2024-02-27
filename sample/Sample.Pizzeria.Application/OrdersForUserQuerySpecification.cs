using Ardalis.Specification;
using Lewee.Domain;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Application;

internal class OrdersForUserQuerySpecification : QuerySpecification<Order>
{
    public OrdersForUserQuerySpecification(string userId)
    {
        this.Query
            .Where(x => x.UserId == userId)
            .Include(x => x.Status)
            .Include(x => x.Pizzas)
            .OrderByDescending(x => x.CreatedAtUtc);
    }
}
