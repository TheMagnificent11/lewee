using System.Diagnostics.CodeAnalysis;
using Lewee.Auth.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Auth.Infrastructure.Data;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by EF")]
internal sealed class TenantConfiguration : AggregateRootConfiguration<Tenant>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Tenant> builder)
    {
        builder.Property(tenant => tenant.Code)
            .IsRequired()
            .HasMaxLength(Tenant.FieldLengths.Code);

        builder.HasIndex(tenant => tenant.Code)
            .IsUnique();

        builder.Property(tenant => tenant.Name)
            .IsRequired()
            .HasMaxLength(Tenant.FieldLengths.Name);
    }
}
