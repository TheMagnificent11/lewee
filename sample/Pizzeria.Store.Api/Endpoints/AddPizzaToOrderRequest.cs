using Pizzeria.Common;

namespace Pizzeria.Store.Api.Endpoints;

internal sealed record AddPizzaToOrderRequest
{
    public Guid OrderId { get; init; }

    public Guid PizzaId { get; init; }
}
