using Lewee.Blazor.Tests.Contracts;
using Lewee.StateManagement;

namespace Lewee.Blazor.Tests.Integration.Infrastructure;

internal sealed record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;
