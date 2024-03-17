using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Pizzeria.Client.Orders;

public record GetOrdersErrorAction : IRequestErrorAction
{
    public GetOrdersErrorAction(string errorMessage, Guid correlationId)
    {
        this.ErrorMessage = errorMessage;
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }

    public string ErrorMessage { get; }
}
