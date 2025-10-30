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
    private readonly IDbContextFactory<TContext> dbContextFactory;
    private readonly IMediator mediator;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcher{TContext}"/> class
    /// </summary>
    /// <param name="dbContextFactory">DbContext factory for creating new context instances</param>
    /// <param name="mediator">MediatR mediator for publishing events</param>
    /// <param name="logger">Logger instance</param>
    public DomainEventDispatcher(
        IDbContextFactory<TContext> dbContextFactory,
        IMediator mediator,
        ILogger<DomainEventDispatcher<TContext>> logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.mediator = mediator;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        // Dispatch events in a fire-and-forget manner to avoid blocking the save operation
        // Use a separate task to avoid issues with the current context
        _ = Task.Run(
            async () =>
            {
                try
                {
                    await this.DispatchEventsAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error dispatching domain events");
                }
            },
            cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchEventsAsync(CancellationToken cancellationToken)
    {
        // Use a new DbContext instance to avoid conflicts with the original context
        await using var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken);

        var dbSet = context.Set<DomainEventReference>();
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

        // Save the dispatch status using the separate context
        await context.SaveChangesAsync(cancellationToken);
    }
}
