using Lewee.Blazor.Fluxor.Actions;

namespace Lewee.Blazor.Tests.Integration;

public record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;
