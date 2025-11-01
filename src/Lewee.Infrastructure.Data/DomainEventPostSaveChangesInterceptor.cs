using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Interceptor that dispatches domain events after successful save changes
/// </summary>
/// <typeparam name="TContext">The database context type</typeparam>
internal sealed class DomainEventPostSaveChangesInterceptor<TContext> : SaveChangesInterceptor
    where TContext : DbContext, IApplicationDbContext
{
    public DomainEventPostSaveChangesInterceptor()
    {
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        // For synchronous SaveChanges, we need to call the async method
        // This is acceptable in this context as we're in an interceptor
        DispatchDomainEventsAsync(eventData.Context, CancellationToken.None)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        return base.SavedChanges(eventData, result);
    }
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

    private static async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is not TContext)
        {
            return;
        }

        var dispatcher = context.GetService<DomainEventDispatcher<TContext>>();

        if (dispatcher == null)
        {
            return;
        }

        await dispatcher.DispatchEventsAsync(cancellationToken);
    }
}
