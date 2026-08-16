using System.Diagnostics.CodeAnalysis;
using Lewee.Auth.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Auth.Infrastructure.Data;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by EF")]
internal sealed class UserConfiguration : AggregateRootConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.ExternalId)
            .IsRequired()
            .HasMaxLength(User.FieldLengths.ExternalId);
        builder.HasIndex(user => user.ExternalId).IsUnique();

        builder.OwnsMany(
            user => user.TenantMemberships,
            memberships =>
            {
                memberships.ToTable("UserTenantMemberships", AuthDbContext.SchemaName);
                memberships.WithOwner().HasForeignKey("UserId");
                memberships.HasKey(membership => membership.Id);
                memberships.HasIndex("UserId", nameof(TenantMembership.TenantId)).IsUnique();
                memberships.HasOne<Tenant>()
                    .WithMany()
                    .HasForeignKey(membership => membership.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
    }
}
