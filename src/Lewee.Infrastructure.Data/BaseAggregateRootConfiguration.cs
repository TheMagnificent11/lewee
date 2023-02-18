using Lewee.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Aggregate Root Configuration
/// </summary>
/// <typeparam name="T">Aggregate root type</typeparam>
public abstract class BaseAggregateRootConfiguration<T> : BaseEntityConfiguration<T>
    where T : BaseAggregateRoot
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Ignore(x => x.DomainEvents);
    }
}
