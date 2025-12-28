using Lewee.Infrastructure.Fluxor;
using Lewee.Tests.Contracts;

namespace Lewee.Blazor.Tests.Integration.Infrastructure;

internal sealed record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;
