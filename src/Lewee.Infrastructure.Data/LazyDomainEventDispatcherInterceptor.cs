using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Lazy wrapper for DomainEventDispatcher that defers dependency resolution
/// </summary>
/// <typeparam name="TContext">The database context type</typeparam>
internal sealed class LazyDomainEventDispatcherInterceptor<TContext> : SaveChangesInterceptor
    where TContext : DbContext, IApplicationDbContext
{
    private readonly IServiceProvider serviceProvider;
    private readonly object lockObj = new();
    private DomainEventDispatcher<TContext>? dispatcher;

    public LazyDomainEventDispatcherInterceptor(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var resolvedDispatcher = this.GetDispatcher();
        return await resolvedDispatcher.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private DomainEventDispatcher<TContext> GetDispatcher()
    {
        if (this.dispatcher == null)
        {
            lock (this.lockObj)
            {
                if (this.dispatcher == null)
                {
                    this.dispatcher = this.serviceProvider.GetRequiredService<DomainEventDispatcher<TContext>>();
                }
            }
        }

        return this.dispatcher;
    }
}
