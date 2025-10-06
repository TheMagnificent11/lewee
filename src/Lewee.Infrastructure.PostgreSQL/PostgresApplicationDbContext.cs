using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.PostgreSQL;

/// <summary>
/// PostgreSQL Application Database Context with exception handling
/// </summary>
/// <typeparam name="TContext">
/// The type of this database context
/// </typeparam>
public abstract class PostgresApplicationDbContext<TContext> : ApplicationDbContext<TContext>
    where TContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    protected PostgresApplicationDbContext(DbContextOptions<TContext> options)
        : base(options)
    {
    }
}
