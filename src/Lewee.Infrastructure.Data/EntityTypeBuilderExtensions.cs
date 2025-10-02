using Lewee.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

internal static class EntityTypeBuilderExtensions
{
    public static void AddAuditUserProperties<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableRecord
    {
        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ModifiedBy)
            .IsRequired()
            .HasMaxLength(255);
    }
}
