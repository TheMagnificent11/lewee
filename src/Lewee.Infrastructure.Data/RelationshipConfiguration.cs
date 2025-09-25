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
        builder.HasKey(x => x.Id);

        // Use ValueGeneratedOnAdd instead of ValueGeneratedNever for better EF Core tracking
        // This allows EF Core to properly track new entities as Added rather than Modified
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.AddAuditUserProperties();

        this.ConfigureRelationship(builder);
    }

    /// <summary>
    /// Configures the relationships for the entity type
    /// </summary>
    /// <param name="builder">Entity builder</param>
    protected abstract void ConfigureRelationship(EntityTypeBuilder<T> builder);
}
