using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Pizzeria.Client.State.StartOrder;

public record StartOrderSuccessAction : IRequestSuccessAction
{
    public StartOrderSuccessAction(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}
