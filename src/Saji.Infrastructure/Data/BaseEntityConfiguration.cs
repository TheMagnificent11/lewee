using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saji.Domain;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Base Entity Configuration
/// </summary>
/// <typeparam name="TEntity">
/// Entity type to be configured
/// </typeparam>
/// <typeparam name="TId">
/// Entity key type
/// </typeparam>
public abstract class BaseEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TId>
{
    /// <summary>
    /// Configures the entity of type <see cref="IEntity{TId}"/>
    /// </summary>
    /// <param name="builder">
    /// The builder to be used to configure the entity type
    /// </param>
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

        builder.Ignore(x => x.DomainEvents);
    }
}
