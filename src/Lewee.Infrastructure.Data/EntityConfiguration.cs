using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Entity Configuration
/// </summary>
/// <typeparam name="TEntity">
/// Entity type to be configured
/// </typeparam>
public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    /// <inheritdoc/>
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ModifiedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Timestamp)
            .IsRowVersion();

        builder.HasQueryFilter(x => !x.IsDeleted);

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
