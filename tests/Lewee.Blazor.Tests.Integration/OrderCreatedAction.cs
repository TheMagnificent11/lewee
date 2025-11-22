using Lewee.Blazor.Fluxor.Actions;

namespace Lewee.Blazor.Tests.Integration;

internal sealed record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;
