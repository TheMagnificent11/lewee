using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Pizzeria.Client.State.StartOrder;

public record StartOrderErrorAction : IRequestErrorAction
{
    public StartOrderErrorAction(string errorMessage, Guid correlationId)
    {
        this.ErrorMessage = errorMessage;
        this.CorrelationId = correlationId;
    }

    public string ErrorMessage { get; }

    public Guid CorrelationId { get; }
}
