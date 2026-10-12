using System.Data;
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
        if (!await this.DomainEventReferenceTableExistsAsync(cancellationToken))
        {
            // The commit that triggered this dispatch may be part of applying
            // migrations (e.g. EF Core creating the domain event table),
            // in which case the domain event table may not exist yet.
            return;
        }

        var eventsToDispatch = await this.ThereAreEventsToDispatchAsync(cancellationToken);

        while (eventsToDispatch && !cancellationToken.IsCancellationRequested)
        {
            await this.DispatchBatchAsync(cancellationToken);

            eventsToDispatch = await this.ThereAreEventsToDispatchAsync(cancellationToken);
        }
    }

    private async Task<bool> DomainEventReferenceTableExistsAsync(CancellationToken token)
    {
        using var dbContext = await this.dbContextFactory.CreateDbContextAsync(token);

        var entityType = dbContext.Model.FindEntityType(typeof(DomainEventReference));
        if (entityType == null)
        {
            return false;
        }

        var tableName = entityType.GetTableName();
        if (string.IsNullOrWhiteSpace(tableName))
        {
            return false;
        }

        var schema = entityType.GetSchema();
        var connection = dbContext.Database.GetDbConnection();
        var closeConnection = connection.State != ConnectionState.Open;

        if (closeConnection)
        {
            await connection.OpenAsync(token);
        }

        try
        {
            var tables = await connection.GetSchemaAsync("Tables", token);

            foreach (DataRow row in tables.Rows)
            {
                var existingTableName = row["TABLE_NAME"] as string;
                if (!string.Equals(existingTableName, tableName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(schema))
                {
                    return true;
                }

                var existingSchema = row["TABLE_SCHEMA"] as string;
                if (string.Equals(existingSchema, schema, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        finally
        {
            if (closeConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private async Task<bool> ThereAreEventsToDispatchAsync(CancellationToken token)
    {
        using var dbContext = await this.dbContextFactory.CreateDbContextAsync(token);

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

    private async Task DispatchBatchAsync(CancellationToken token)
    {
        using var scope = await this.dbContextFactory.CreateDbContextAsync(token);

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
                this.logger.LogCouldNotDeserializeDomainEventReference(domainEventReference.Id);
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
