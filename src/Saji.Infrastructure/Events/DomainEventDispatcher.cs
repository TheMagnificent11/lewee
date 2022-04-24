using MediatR;
using Microsoft.EntityFrameworkCore;
using Saji.Domain;
using Serilog;

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
    private readonly TContext context;
    private readonly IMediator mediator;
    private readonly int batchSize;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcher{TContext}"/> class
    /// </summary>
    /// <param name="context">
    /// Database context
    /// </param>
    /// <param name="mediator">
    /// Mediator
    /// </param>
    /// <param name="batchSize">
    /// Maximum size of batch on domain events that will be dispatched for each cycle
    /// </param>
    /// <param name="logger">
    /// Logger
    /// </param>
    public DomainEventDispatcher(
        TContext context,
        IMediator mediator,
        int batchSize,
        ILogger logger)
    {
        this.context = context;
        this.mediator = mediator;
        this.batchSize = batchSize;
        this.logger = logger.ForContext<DomainEventDispatcher<TContext>>();
    }

    /// <summary>
    /// Dispatch a domain events
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation toke
    /// </param>
    /// <returns>
    /// An async task
    /// </returns>
    public async Task DispatchEvents(CancellationToken cancellationToken)
    {
        var eventsToDispatch = await this.ThereAreEventsToDispatch(cancellationToken);

        while (eventsToDispatch && !cancellationToken.IsCancellationRequested)
        {
            await this.DispatchBatch(cancellationToken);

            eventsToDispatch = await this.ThereAreEventsToDispatch(cancellationToken);
        }
    }

    private async Task<bool> ThereAreEventsToDispatch(CancellationToken token)
    {
        var dbSet = this.context.Set<DomainEventReference>();

        if (dbSet == null)
        {
            return false;
        }

        return await dbSet
            .Where(x => x.Dispatched == false)
            .OrderBy(x => x.PersistedAt)
            .AnyAsync(token);
    }

    private async Task DispatchBatch(CancellationToken token)
    {
        var dbSet = this.context.Set<DomainEventReference>();
        if (dbSet == null)
        {
            return;
        }

        var events = await dbSet
            .Where(x => !x.Dispatched)
            .OrderBy(x => x.PersistedAt)
            .Take(this.batchSize)
            .ToArrayAsync(token);

        var domainEvents = new List<IDomainEvent>();

        foreach (var domainEventReference in events)
        {
            domainEventReference.Dispatch();

            var domainEvent = domainEventReference.ToDomainEvent();

            if (domainEvent == null)
            {
                this.logger.Warning(
                    "Could not deserialize DomainEventReference {Id}",
                    domainEventReference.Id);
            }
            else
            {
                domainEvents.Add(domainEvent);
            }
        }

        if (domainEvents.Any())
        {
            foreach (var domainEvent in domainEvents)
            {
                await this.mediator.Publish(domainEvent, token);
            }
        }
    }
}
