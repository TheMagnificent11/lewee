using Lewee.Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Lewee.Infrastructure.Events;

internal class DomainEventDispatcherService<TContext> : BackgroundService
    where TContext : DbContext, IDbContext
{
    public DomainEventDispatcherService(DomainEventDispatcher<TContext> domainEventDispatcher)
    {
        this.DomainEventDispatcher = domainEventDispatcher;
    }

    public DomainEventDispatcher<TContext> DomainEventDispatcher { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await this.DomainEventDispatcher.DispatchEvents(stoppingToken);

            await Task.Delay(2000, stoppingToken);
        }
    }
}
