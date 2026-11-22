using System.Diagnostics.CodeAnalysis;
using Lewee.Auth.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Auth.Infrastructure.Data;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by EF")]
internal sealed class RoleConfiguration : AggregateRootConfiguration<Role>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Role> builder)
    {
        builder.Property(role => role.Code)
            .IsRequired()
            .HasMaxLength(Role.FieldLengths.Code);

        builder.HasIndex(role => role.Code)
            .IsUnique();

        builder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(Role.FieldLengths.Name);
    }
}
