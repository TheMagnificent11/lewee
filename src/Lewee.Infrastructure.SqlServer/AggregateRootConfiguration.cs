using Lewee.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.SqlServer;

/// <summary>
/// Aggregate Root Configuration
/// </summary>
/// <typeparam name="T">Aggregate root type</typeparam>
public abstract class AggregateRootConfiguration<T> : EntityConfiguration<T>
    where T : AggregateRoot
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Ignore(x => x.DomainEvents);
    }
}
