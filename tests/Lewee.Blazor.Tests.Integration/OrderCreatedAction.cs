using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Tests.Contracts;

namespace Lewee.Blazor.Tests.Integration;

internal sealed record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;
