using Lewee.Application.Data;
using Lewee.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Lewee.Infrastructure.Events;

internal class DomainEventDispatcher<TContext>
    where TContext : DbContext, IApplicationDbContext
{
    private const int BatchSize = 50;

    private readonly IDbContextFactory<TContext> dbContextFactory;
    private readonly IMediator mediator;
    private readonly ILogger logger;

    public DomainEventDispatcher(
        IDbContextFactory<TContext> dbContextFactory,
        IMediator mediator,
        ILogger logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.mediator = mediator;
        this.logger = logger.ForContext<DomainEventDispatcher<TContext>>();
    }

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
        using (var scope = this.dbContextFactory.CreateDbContext())
        {
            var dbSet = scope.Set<DomainEventReference>();

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

    private async Task DispatchBatch(CancellationToken token)
    {
        using (var scope = this.dbContextFactory.CreateDbContext())
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

            await scope.SaveChangesAsync(token);
        }
    }
}
