using Lewee.Application.Data;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

internal class ReadModelService<TContext> : IReadModelService
    where TContext : DbContext, IApplicationDbContext
{
    private readonly IDbContextFactory<TContext> dbContextFactory;

    public ReadModelService(IDbContextFactory<TContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<T?> RetrieveByKey<T>(string key, CancellationToken cancellationToken)
        where T : class, IReadModel
    {
        using (var context = this.dbContextFactory.CreateDbContext())
        {
            var exisiting = await Retrieve<T>(key, context, cancellationToken);

            if (exisiting != null)
            {
                return null;
            }

            if (exisiting == null)
            {
                return null;
            }

            return exisiting.ToReadModel() as T;
        }
    }

    public async Task AddOrUpdate<T>(T readModel, string key, CancellationToken cancellationToken)
        where T : class, IReadModel
    {
        using (var context = this.dbContextFactory.CreateDbContext())
        {
            var existing = await Retrieve<T>(key, context, cancellationToken);

            if (existing == null)
            {
                var newReference = new ReadModelReference(readModel, key);
                context.ReadModelReferences?.Add(newReference);

                await context.SaveChangesAsync(cancellationToken);

                return;
            }

            existing.UpdateJson(readModel);

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task<ReadModelReference?> Retrieve<T>(string key, TContext context, CancellationToken cancellationToken)
        where T : class, IReadModel
    {
        var type = typeof(T);
        if (type == null)
        {
            throw new InvalidOperationException("Invalid read model type");
        }

        if (context == null || context.ReadModelReferences == null)
        {
            throw new InvalidOperationException("Invalid DB context");
        }

        var assemblyName = type.Assembly.GetName();
        var className = type.FullName;

        return await context.ReadModelReferences
            .Where(x => x.ReadModelAssemblyName.Equals(assemblyName))
            .Where(x => x.ReadModelClassName.Equals(className))
            .Where(x => x.Key.Equals(key))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
