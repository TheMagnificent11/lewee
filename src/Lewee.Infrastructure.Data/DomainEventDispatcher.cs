using Lewee.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// SaveChanges interceptor that dispatches domain events after a successful save
/// </summary>
/// <typeparam name="TContext">The database context type</typeparam>
internal class DomainEventDispatcher<TContext> : SaveChangesInterceptor
    where TContext : DbContext, IApplicationDbContext
{
    private readonly IMediator mediator;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcher{TContext}"/> class
    /// </summary>
    /// <param name="mediator">MediatR mediator for publishing events</param>
    /// <param name="logger">Logger instance</param>
    public DomainEventDispatcher(
        IMediator mediator,
        ILogger<DomainEventDispatcher<TContext>> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await this.DispatchEventsAsync(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchEventsAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is not TContext typedContext)
        {
            return;
        }

        var dbSet = typedContext.Set<DomainEventReference>();
        if (dbSet == null)
        {
            return;
        }

        var events = await dbSet
            .Where(x => !x.Dispatched)
            .OrderBy(x => x.PersistedAt)
            .ToListAsync(cancellationToken);

        if (events.Count == 0)
        {
            return;
        }

        foreach (var domainEventReference in events)
        {
            domainEventReference.Dispatch();

            var domainEvent = domainEventReference.ToDomainEvent();

            if (domainEvent == null)
            {
                this.logger.LogWarning(
                    "Could not deserialize DomainEventReference {Id}",
                    domainEventReference.Id);
                continue;
            }

            await this.mediator.Publish(domainEvent, cancellationToken);
        }

        await typedContext.SaveChangesAsync(cancellationToken);
    }
}
