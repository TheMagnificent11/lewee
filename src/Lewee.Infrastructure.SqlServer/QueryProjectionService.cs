using Lewee.Application.Data;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.SqlServer;

internal class QueryProjectionService<TContext> : IQueryProjectionService
    where TContext : DbContext, IApplicationDbContext
{
    private readonly IDbContextFactory<TContext> dbContextFactory;

    public QueryProjectionService(IDbContextFactory<TContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<T?> RetrieveByKey<T>(string key, CancellationToken cancellationToken)
        where T : class, IQueryProjection
    {
        using (var context = this.dbContextFactory.CreateDbContext())
        {
            var exisiting = await Retrieve<T>(key, context, cancellationToken);
            if (exisiting == null)
            {
                return null;
            }

            return exisiting.ToQueryProjection() as T;
        }
    }

    public async Task AddOrUpdate<T>(T readModel, string key, CancellationToken cancellationToken)
        where T : class, IQueryProjection
    {
        using (var context = this.dbContextFactory.CreateDbContext())
        {
            var existing = await Retrieve<T>(key, context, cancellationToken);

            if (existing == null)
            {
                var newReference = new QueryProjectionReference(readModel, key);
                context.QueryProjectionReferences?.Add(newReference);

                await context.SaveChangesAsync(cancellationToken);

                return;
            }

            existing.UpdateJson(readModel);

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task<QueryProjectionReference?> Retrieve<T>(string key, TContext context, CancellationToken cancellationToken)
        where T : class, IQueryProjection
    {
        var type = typeof(T);
        if (type == null)
        {
            throw new InvalidOperationException("Invalid read model type");
        }

        if (context == null || context.QueryProjectionReferences == null)
        {
            throw new InvalidOperationException("Invalid DB context");
        }

        var assemblyName = type.Assembly.GetName().Name;
        var className = type.FullName;

        return await context.QueryProjectionReferences
            .Where(x => x.QueryProjectionAssemblyName == assemblyName)
            .Where(x => x.QueryProjectionClassName == className)
            .Where(x => x.Key == key)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
