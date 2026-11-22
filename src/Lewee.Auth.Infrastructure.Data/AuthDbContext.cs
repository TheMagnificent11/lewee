using Lewee.Auth.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Auth.Infrastructure.Data;

/// <summary>
/// Authentication database context.
/// </summary>
public sealed class AuthDbContext : ApplicationDbContext<AuthDbContext>
{
    /// <summary>
    /// Authentication schema name.
    /// </summary>
    public const string SchemaName = "auth";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthDbContext"/> class.
    /// </summary>
    /// <param name="options">Database context options.</param>
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc />
    public override string Schema => SchemaName;

    /// <summary>
    /// Gets or sets tenants.
    /// </summary>
    public DbSet<Tenant> Tenants { get; set; }

    /// <summary>
    /// Gets or sets users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets roles.
    /// </summary>
    public DbSet<Role> Roles { get; set; }
}
