using Lewee.Infrastructure.Fluxor;
using Lewee.Tests.Contracts;

namespace Lewee.Blazor.Tests.Web;

internal sealed record OrderCreatedAction : IMessageReceivedAction<PizzaOrder>
{
    public PizzaOrder Data { get; init; }

    public Guid CorrelationId { get; init; }
}
