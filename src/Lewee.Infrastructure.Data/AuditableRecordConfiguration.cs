using System;
using System.Linq;
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
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.AddAuditUserProperties();

        // Add concurrency token as a shadow property
        // Check if using PostgreSQL by looking at model annotations
        var model = builder.Metadata.Model;
        var isPostgreSql = model.GetAnnotations()
            .Any(a => a.Name.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) ||
                     a.Value?.ToString()?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true);

        if (isPostgreSql)
        {
            // PostgreSQL uses uint with xid type
            builder.Property<uint>("Version")
                .IsRowVersion()
                .HasColumnType("xid");
        }
        else
        {
            // SQL Server uses byte[] with rowversion
            builder.Property<byte[]>("Version")
                .IsRowVersion();
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
