using Lewee.Blazor.Fluxor.Actions;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client.Orders;

public record GetOrdersSuccessAction : IQuerySuccessAction<OrderDto[]>
{
    public GetOrdersSuccessAction(OrderDto[] data, Guid correlationId)
    {
        this.Data = data;
        this.CorrelationId = correlationId;
    }

    public OrderDto[] Data { get; }

    public Guid CorrelationId { get; }
}
