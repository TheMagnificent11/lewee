using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.SqlServer;

internal class QueryProjectionReferenceConfiguration : IEntityTypeConfiguration<QueryProjectionReference>
{
    public void Configure(EntityTypeBuilder<QueryProjectionReference> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QueryProjectionAssemblyName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.QueryProjectionClassName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Key)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.QueryProjectionJson)
            .HasMaxLength(8000)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ModifiedAtUtc)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRowVersion();

        builder.HasIndex(x => new { x.QueryProjectionAssemblyName, x.QueryProjectionClassName, x.Key })
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
