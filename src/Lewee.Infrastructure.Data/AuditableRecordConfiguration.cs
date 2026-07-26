using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Auditable Record Configuration
/// </summary>
/// <typeparam name="TEntity">Entity type that extends AuditableRecord</typeparam>
public abstract class AuditableRecordConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableRecord
{
    /// <inheritdoc/>
    [SuppressMessage(
        "Performance",
        "CA1851:Possible multiple enumerations of 'IEnumerable' collection",
        Justification = "The enumerable is small in size the and performance impact is negligible")]
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => x.Id);
        builder.AddAuditUserProperties();

        // Add concurrency token as a shadow property
        // Detect database provider by looking at model annotations
        var model = builder.Metadata.Model;
        var annotations = model.GetAnnotations();

        if (annotations == null)
        {
            // No concurrency token configuration for unknown providers
            this.ConfigureEntity(builder);
            return;
        }

        var isPostgreSql = annotations
            .Any(a => a.Name.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) ||
                     a.Value?.ToString()?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true);

        var isSqlServer = annotations
            .Any(a => a.Name.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) ||
                     a.Value?.ToString()?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) == true);

        switch ((isPostgreSql, isSqlServer))
        {
            case (true, _):
                // PostgreSQL uses uint with xid type
                builder.Property<uint>("Version")
                    .IsRowVersion()
                    .HasColumnType("xid");
                break;

            case (false, true):
                // SQL Server uses byte[] with rowversion
                builder.Property<byte[]>("Version")
                    .IsRowVersion();
                break;

            default:
                // No concurrency token configuration for unknown providers
                break;
        }

        this.ConfigureEntity(builder);
    }

    /// <summary>
    /// Configures the database table for the entity type
    /// </summary>
    /// <param name="builder">
    /// The builder to be used to configure the entity type
    /// </param>
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
