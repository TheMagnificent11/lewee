using Lewee.Domain;
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
    /// <param name="authenticatedUserService">Authentication service interface</param>
    /// <returns>The updated DB context options builder</returns>
    public static DbContextOptionsBuilder AddAuditInterceptor(
        this DbContextOptionsBuilder optionsBuilder,
        IAuthenticatedUserService authenticatedUserService)
    {
        optionsBuilder.AddInterceptors(new AuditDetailsSaveChangesInterceptor(authenticatedUserService));

        return optionsBuilder;
    }

    /// <summary>
    /// Adds the domain event interceptor to the database context options builder
    /// </summary>
    /// <typeparam name="TContext">DB context type</typeparam>
    /// <param name="optionsBuilder">DB context options builder</param>
    /// <param name="authenticatedUserService">Authentication service interface</param>
    /// <returns>The updated DB context options builder</returns>
    public static DbContextOptionsBuilder AddDomainEventInterceptor<TContext>(
        this DbContextOptionsBuilder optionsBuilder,
        IAuthenticatedUserService authenticatedUserService)
        where TContext : DbContext, IApplicationDbContext
    {
        optionsBuilder.AddInterceptors(new DomainEventSaveChangesInterceptor<TContext>(authenticatedUserService));

        return optionsBuilder;
    }

    /// <summary>
    /// Adds the domain event dispatcher interceptor to the database context options builder
    /// </summary>
    /// <typeparam name="TContext">DB context type</typeparam>
    /// <param name="optionsBuilder">DB context options builder</param>
    /// <param name="serviceProvider">Service provider for resolving dependencies</param>
    /// <returns>The updated DB context options builder</returns>
    public static DbContextOptionsBuilder AddDomainEventDispatcherInterceptor<TContext>(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider)
        where TContext : DbContext, IApplicationDbContext
    {
        // Create a lazy interceptor that resolves dependencies when first needed
        var lazyDispatcher = new LazyDomainEventDispatcherInterceptor<TContext>(serviceProvider);
        optionsBuilder.AddInterceptors(lazyDispatcher);

        return optionsBuilder;
    }
}
