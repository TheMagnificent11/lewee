using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saji.Infrastructure.Data;
using Sample.Customers.Domain.Entities;

namespace Sample.Customers.Infrastructure.Data;

public sealed class CustomerEntityConfiguration : BaseEntityConfiguration<Customer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(Customer.MaxFieldLengths.UserId);

        builder.Property(x => x.GivenName)
            .IsRequired()
            .HasMaxLength(Customer.MaxFieldLengths.GivenName);

        builder.Property(x => x.Surname)
            .IsRequired()
            .HasMaxLength(Customer.MaxFieldLengths.Surname);
    }
}
