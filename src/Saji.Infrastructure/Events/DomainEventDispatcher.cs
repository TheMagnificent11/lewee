using Microsoft.EntityFrameworkCore;

namespace Saji.Infrastructure.Events;

/// <summary>
/// Domain Event Dispatcher
/// </summary>
/// <typeparam name="TContext">
/// Databae context type
/// </typeparam>
public class DomainEventDispatcher<TContext>
    where TContext : DbContext
{
    /// <summary>
    /// Dispatch a domain events
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation toke
    /// </param>
    /// <returns>
    /// An async task
    /// </returns>
    public Task DispatchEvents(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
