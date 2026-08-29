using Lewee.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Entity Configuration
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class EntityConfiguration<TEntity> : AuditableRecordConfiguration<TEntity>
    where TEntity : Entity
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
