using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

internal class QueryProjectionService<TContext> : IQueryProjectionService
    where TContext : DbContext, IApplicationDbContext
{
    private readonly TContext dbContext;

    public QueryProjectionService(TContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<T?> RetrieveByKeyAsync<T>(string key, CancellationToken cancellationToken)
        where T : class, IQueryProjection
    {
        var exisiting = await this.RetrieveAsync<T>(key, cancellationToken);
        if (exisiting == null)
        {
            return null;
        }

        return exisiting.ToQueryProjection() as T;
    }

    public async Task AddOrUpdateAsync<T>(T queryProjection, string key, CancellationToken cancellationToken)
        where T : class, IQueryProjection
    {
        var existing = await this.RetrieveAsync<T>(key, cancellationToken);

        if (existing == null)
        {
            var newReference = new QueryProjectionReference(queryProjection, key);
            this.dbContext.QueryProjectionReferences?.Add(newReference);

            await this.dbContext.SaveChangesAsync(cancellationToken);

            return;
        }

        existing.UpdateJson(queryProjection);

        await this.dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<QueryProjectionReference?> RetrieveAsync<T>(string key, CancellationToken cancellationToken)
        where T : class, IQueryProjection
    {
        var type = typeof(T);

        if (this.dbContext.QueryProjectionReferences == null)
        {
            throw new InvalidOperationException("Invalid DB context");
        }

        var assemblyName = type.Assembly.GetName().Name;
        var className = type.FullName;

        return await this.dbContext.QueryProjectionReferences
            .Where(x => x.QueryProjectionAssemblyName == assemblyName)
            .Where(x => x.QueryProjectionClassName == className)
            .Where(x => x.Key == key)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
