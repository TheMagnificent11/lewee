using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

internal class ReadModelReferenceConfiguration : IEntityTypeConfiguration<ReadModelReference>
{
    public void Configure(EntityTypeBuilder<ReadModelReference> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReadModelAssemblyName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ReadModelClassName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Key)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ReadModelJson)
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

        builder.HasIndex(x => new { x.ReadModelAssemblyName, x.ReadModelClassName, x.Key })
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
