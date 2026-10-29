using System.Collections.Immutable;
using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

public record OrderState : RequestState<OrderDto>
{
    public ImmutableDictionary<Guid, OrderCommandType> PendingCommands { get; init; } =
        ImmutableDictionary<Guid, OrderCommandType>.Empty;

    public ImmutableHashSet<Guid> InFlightCommands { get; init; } =
        ImmutableHashSet<Guid>.Empty;

    public bool HasPendingCommands => this.PendingCommands.Count > 0 || this.InFlightCommands.Count > 0;
}
