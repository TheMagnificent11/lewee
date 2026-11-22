using Lewee.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Extensions for configuring the <see cref="IApplicationDbContext" />
/// </summary>
public static class ApplicationDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds the audit interceptor to the database context options builder
    /// </summary>
    /// <param name="optionsBuilder">DB context options builder</param>
    /// <param name="serviceProvider">Service provider to resolve dependencies</param>
    /// <returns>The updated DB context options builder</returns>
    public static DbContextOptionsBuilder AddAuditInterceptor(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        var authenticatedUserService = serviceProvider.GetRequiredService<IAuthenticatedUserService>();

        optionsBuilder.AddInterceptors(new AuditDetailsSaveChangesInterceptor(authenticatedUserService));

        return optionsBuilder;
    }

    /// <summary>
    /// Adds the domain event interceptor to the database context options builder
    /// </summary>
    /// <typeparam name="TContext">DB context type</typeparam>
    /// <param name="optionsBuilder">DB context options builder</param>
    /// <param name="serviceProvider">Service provider to resolve dependencies</param>
    /// <returns>The updated DB context options builder</returns>
    public static DbContextOptionsBuilder AddDomainEventInterceptors<TContext>(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider)
        where TContext : DbContext, IApplicationDbContext
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        var authenticatedUserService = serviceProvider.GetRequiredService<IAuthenticatedUserService>();
        var domainEventSaveChangesInterceptor = new DomainEventSaveChangesInterceptor<TContext>(authenticatedUserService);
        var domainEventDispatcherInterceptor = new DomainEventsTransactionInterceptor<TContext>(serviceProvider);

        optionsBuilder.AddInterceptors(domainEventSaveChangesInterceptor);
        optionsBuilder.AddInterceptors(domainEventDispatcherInterceptor);

        return optionsBuilder;
    }
}
