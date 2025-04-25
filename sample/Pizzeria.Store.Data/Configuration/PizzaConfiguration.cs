using Lewee.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

internal class PizzaConfiguration : AggregateRootConfiguration<Pizza>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Pizza> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(Pizza.FieldLengths.Name);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(Pizza.FieldLengths.Description);

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(5, 2);
    }
}
