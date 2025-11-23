using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Relationship Configuration
/// </summary>
/// <typeparam name="T">Relationship entity type</typeparam>
public abstract class RelationshipConfiguration<T> : IEntityTypeConfiguration<T>
    where T : Relationship
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<T> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.AddAuditUserProperties();

        this.ConfigureRelationship(builder);
    }

    /// <summary>
    /// Configures the relationships for the entity type
    /// </summary>
    /// <param name="builder">Entity builder</param>
    protected abstract void ConfigureRelationship(EntityTypeBuilder<T> builder);
}
