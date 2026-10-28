using System.Collections.Concurrent;

namespace Pizzeria.Ordering.StateManagement.Orders;

public sealed class OrderCommandCorrelationTracker : IOrderCommandCorrelationTracker
{
    private readonly ConcurrentDictionary<Guid, OrderCommandType> pendingCommands = new();

    public void Register(Guid correlationId, OrderCommandType commandType)
    {
        this.pendingCommands[correlationId] = commandType;
    }

    public bool TryGetAndRemove(Guid correlationId, out OrderCommandType commandType)
    {
        return this.pendingCommands.TryRemove(correlationId, out commandType);
    }
}
