using Lewee.Blazor.Tests.Contracts;
using Lewee.StateManagement;

namespace Lewee.Blazor.Tests.Web;

internal sealed record OrderCreatedAction : IMessageReceivedAction<PizzaOrder>
{
    public PizzaOrder Data { get; init; }

    public Guid CorrelationId { get; init; }
}
