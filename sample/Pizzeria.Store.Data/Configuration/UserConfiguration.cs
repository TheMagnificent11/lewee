using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

internal sealed class UserConfiguration : AggregateRootConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(x => x.ExternalId)
            .IsRequired()
            .HasMaxLength(User.FieldLengths.ExternalId);

        builder.HasIndex(x => x.ExternalId)
            .IsUnique();
    }
}
