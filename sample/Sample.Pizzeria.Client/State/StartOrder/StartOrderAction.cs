using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Pizzeria.Client.State.StartOrder;

public record StartOrderAction : IRequestAction
{
    public StartOrderAction()
    {
        this.CorrelationId = Guid.NewGuid();
    }

    public Guid CorrelationId { get; }
}
