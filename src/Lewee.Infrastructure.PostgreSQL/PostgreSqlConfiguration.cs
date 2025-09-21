using System.Reflection;
using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.PostgreSQL;

/// <summary>
/// Configuration for Lewee PostgreSQL components
/// </summary>
public static class PostgreSqlConfiguration
{
    /// <summary>
    /// Configures the PostgreSQL database related to the <typeparamref name="T"/> database context
    /// </summary>
    /// <typeparam name="T">DB context type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Connection string</param>
    /// <param name="domainAssembly">Domain assembly associated the DB context</param>
    /// <param name="schema">Postgres DB schema of <typeparamref name="T"/></param>
    /// <returns>Updated service collection</returns>
    public static IServiceCollection AddLeweePostgreSQL<T>(
        this IServiceCollection services,
        string connectionString,
        Assembly domainAssembly,
        string? schema = null)
        where T : ApplicationDbContext<T>
    {
        services
            .AddDbContextFactory<T>((provider, options) =>
            {
                var authenticatedUserService = provider.GetRequiredService<IAuthenticatedUserService>();

                if (string.IsNullOrWhiteSpace(schema))
                {
                    options.UseNpgsql(connectionString);
                }
                else
                {
                    options.UseNpgsql(
                        connectionString,
                        x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema));
                }  

                options.AddAuditInterceptor(authenticatedUserService);
                options.AddDomainEventInterceptor<T>(authenticatedUserService);
            })
            .AddLeweeDatabaseServices<T>(domainAssembly);

        return services;
    }
}
