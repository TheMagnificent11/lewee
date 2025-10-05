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
