using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Enum Entity Configuration
/// </summary>
/// <typeparam name="TEnum">
/// Enum type
/// </typeparam>
public abstract class BaseEnumEntityConfiguration<TEnum> : IEntityTypeConfiguration<EnumEntity<TEnum>>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Gets the table name
    /// </summary>
    public abstract string TableName { get; }

    /// <summary>
    /// Configures enum entity lookup database table
    /// </summary>
    /// <param name="builder">
    /// Table builder
    /// </param>
    public virtual void Configure(EntityTypeBuilder<EnumEntity<TEnum>> builder)
    {
        builder.ToTable(this.TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        foreach (var item in Enum.GetValues<TEnum>())
        {
            builder.HasData(new EnumEntity<TEnum>(item));
        }
    }
}
