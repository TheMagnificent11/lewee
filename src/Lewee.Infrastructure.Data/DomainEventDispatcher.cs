using Lewee.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Data;

internal sealed class DomainEventDispatcher<TContext>
    where TContext : DbContext, IApplicationDbContext
{
    private const int BatchSize = 50;

    private readonly IDbContextFactory<TContext> dbContextFactory;
    private readonly IMediator mediator;
    private readonly ILogger logger;

    public DomainEventDispatcher(
        IDbContextFactory<TContext> dbContextFactory,
        IMediator mediator,
        ILogger<DomainEventDispatcher<TContext>> logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task DispatchEventsAsync(CancellationToken cancellationToken)
    {
        var eventsToDispatch = await this.ThereAreEventsToDispatchAsync(cancellationToken);

        while (eventsToDispatch && !cancellationToken.IsCancellationRequested)
        {
            await this.DispatchBatchAsync(cancellationToken);

            eventsToDispatch = await this.ThereAreEventsToDispatchAsync(cancellationToken);
        }
    }

    private async Task<bool> ThereAreEventsToDispatchAsync(CancellationToken token)
    {
        using (var dbContext = await this.dbContextFactory.CreateDbContextAsync(token))
        {
            var dbSet = dbContext.Set<DomainEventReference>();

            if (dbSet == null)
            {
                return false;
            }

            return await dbSet
                .Where(x => !x.Dispatched)
                .OrderBy(x => x.PersistedAt)
                .AnyAsync(token);
        }
    }

    private async Task DispatchBatchAsync(CancellationToken token)
    {
        using (var scope = await this.dbContextFactory.CreateDbContextAsync(token))
        {
            var dbSet = scope.Set<DomainEventReference>();
            if (dbSet == null)
            {
                return;
            }

            var events = await dbSet
                .Where(x => !x.Dispatched)
                .OrderBy(x => x.PersistedAt)
                .Take(BatchSize)
                .ToArrayAsync(token);

            var domainEvents = new List<DomainEvent>();

            foreach (var domainEventReference in events)
            {
                domainEventReference.Dispatch();

                var domainEvent = domainEventReference.ToDomainEvent();

                if (domainEvent == null)
                {
                    this.logger.LogWarning(
                        "Could not deserialize DomainEventReference {Id}",
                        domainEventReference.Id);
                }
                else
                {
                    domainEvents.Add(domainEvent);
                }
            }

            if (domainEvents.Count > 0)
            {
                foreach (var domainEvent in domainEvents)
                {
                    await this.mediator.Publish(domainEvent, token);
                }
            }

            await scope.SaveChangesAsync(token);
        }
    }
}
