using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lewee.Infrastructure.Data;

internal sealed class DomainEventDispatcherService<TContext> : BackgroundService
    where TContext : DbContext, IApplicationDbContext
{
    private readonly IServiceProvider serviceProvider;

    public DomainEventDispatcherService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var domainEventDispatcher = scope.ServiceProvider.GetRequiredService<DomainEventDispatcher<TContext>>();
                await domainEventDispatcher.DispatchEventsAsync(stoppingToken);
            }

            await Task.Delay(2500, stoppingToken);
        }
    }
}
