using Lewee.Application.Data;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lewee.Infrastructure.SqlServer;

internal class DomainEventSaveChangesInterceptor<TContext> : SaveChangesInterceptor
    where TContext : DbContext, IApplicationDbContext
{
    private readonly IAuthenticatedUserService authenticatedUserService;

    public DomainEventSaveChangesInterceptor(IAuthenticatedUserService authenticatedUserService)
    {
        this.authenticatedUserService = authenticatedUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        this.StoreDomainEvents(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        this.StoreDomainEvents(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StoreDomainEvents(DbContext? context)
    {
        if (context == null || context is not TContext)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            this.StoreDomainEventsForEntry((TContext)context, entry);
        }
    }

    private void StoreDomainEventsForEntry(TContext context, EntityEntry entry)
    {
        if (entry.Entity is not AggregateRoot aggregateRootEntity)
        {
            return;
        }

        var events = aggregateRootEntity.DomainEvents.GetAndClear();

        foreach (var domainEvent in events)
        {
            if (domainEvent == null)
            {
                continue;
            }

            var reference = new DomainEventReference(domainEvent, this.authenticatedUserService.UserId);

            context.DomainEventReferences?.Add(reference);
        }
    }
}
