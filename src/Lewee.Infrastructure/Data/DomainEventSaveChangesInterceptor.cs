using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Domain Event Save Changes Interceptor
/// </summary>
/// <typeparam name="TContext">Database context type</typeparam>
public class DomainEventSaveChangesInterceptor<TContext> : SaveChangesInterceptor
    where TContext : BaseApplicationDbContext<TContext>
{
    /// <summary>
    /// Saves changes interceptor
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <param name="result">Result</param>
    /// <returns>Interception result</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StoreDomainEvents(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Saves changes interceptor
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <param name="result">Result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asynchronous task contains an interception result</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StoreDomainEvents(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void StoreDomainEvents(DbContext? context)
    {
        if (context == null || context is not TContext)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            StoreDomainEventsForEntry((TContext)context, entry);
        }
    }

    private static void StoreDomainEventsForEntry(TContext context, EntityEntry entry)
    {
        if (entry.Entity is not BaseEntity baseEntity)
        {
            return;
        }

        var events = baseEntity.DomainEvents.GetAndClear();

        foreach (var domainEvent in events)
        {
            if (domainEvent == null)
            {
                continue;
            }

            var reference = new DomainEventReference(domainEvent);

            context.DomainEventReferences?.Add(reference);
        }
    }
}
