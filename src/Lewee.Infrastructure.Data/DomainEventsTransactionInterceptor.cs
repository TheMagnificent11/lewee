using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Data;

internal class DomainEventsTransactionInterceptor<TContext> : DbTransactionInterceptor
    where TContext : DbContext, IApplicationDbContext
{
    private readonly IServiceProvider serviceProvider;

    public DomainEventsTransactionInterceptor(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public override async Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await using var scope = this.serviceProvider.CreateAsyncScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<DomainEventDispatcher<TContext>>();

        await dispatcher.DispatchEventsAsync(cancellationToken);
    }
}
