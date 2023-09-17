using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.UseTable.Actions;

public record UseTableSuccessAction : IRequestSuccessAction
{
    public UseTableSuccessAction(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}
