using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.SqlServer;

internal class DomainEventReferenceConfiguration : IEntityTypeConfiguration<DomainEventReference>
{
    public void Configure(EntityTypeBuilder<DomainEventReference> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DomainEventAssemblyName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.DomainEventClassName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.DomainEventJson)
            .HasMaxLength(8000)
            .IsRequired();

        builder.Property(x => x.Dispatched)
            .IsRequired();

        builder.Property(x => x.PersistedAt)
            .IsRequired();

        builder.Property(x => x.DispatchedAt)
            .IsRequired(false);

        builder.Property(x => x.UserId)
            .HasMaxLength(50);

        builder.HasIndex(
            nameof(DomainEventReference.Dispatched),
            nameof(DomainEventReference.PersistedAt));
    }
}
