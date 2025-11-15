using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

internal sealed class CustomerConfiguration : AggregateRootConfiguration<Customer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.ExternalId)
            .IsRequired()
            .HasMaxLength(Customer.FieldLengths.ExternalId);

        builder.HasIndex(x => x.ExternalId)
            .IsUnique();
    }
}
