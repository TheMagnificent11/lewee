using Lewee.Blazor.Tests.Contracts;
using Lewee.Contracts.StateManagement;

namespace Lewee.Blazor.Tests.Integration;

internal sealed record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;
