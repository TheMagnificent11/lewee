using Lewee.Shared;

namespace Lewee.Domain;

/// <summary>
/// Enum Entity
/// </summary>
/// <typeparam name="TKey">
/// Enum entity key type
/// </typeparam>
public class EnumEntity<TKey>
    where TKey : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumEntity{TKey}"/> class
    /// </summary>
    /// <param name="id">
    /// ID
    /// </param>
    public EnumEntity(TKey id)
    {
        this.Id = id;
        this.Name = id.GetDescription();
    }

    // EF Core constructor
    private EnumEntity()
    {
        this.Name = "EF";
    }

    /// <summary>
    /// Gets or sets the ID of the enum entity
    /// </summary>
    public TKey Id { get; protected set; }

    /// <summary>
    /// Gets or sets the enum item's name
    /// </summary>
    public string Name { get; protected set; }
}
