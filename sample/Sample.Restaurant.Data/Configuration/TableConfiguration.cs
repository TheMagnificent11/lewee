using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Data.Configuration;

internal sealed class TableConfiguration : AggregateRootConfiguration<Table>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Table> builder)
    {
        builder.HasIndex(x => x.TableNumber)
            .IsUnique();

        builder.Ignore(x => x.CurrentOrder);

        builder.Metadata
            .FindNavigation(nameof(Table.Orders))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
