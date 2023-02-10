using System.Text.Json;

namespace Lewee.Domain;

/// <summary>
/// Read Model Reference
/// </summary>
public class ReadModelReference : ISoftDeleteEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadModelReference"/> class
    /// </summary>
    /// <param name="readModel">Read model</param>
    /// <param name="key">Read model key</param>
    public ReadModelReference(IReadModel readModel, string key)
    {
        var (assemblyName, fullCalssName, readModelType) = readModel.GetAssemblyInfo("Invalid read model type");

        this.Key = key;
        this.ReadModelAssemblyName = assemblyName;
        this.ReadModelClassName = fullCalssName;
        this.ReadModelJson = JsonSerializer.Serialize(readModel, readModelType);
        this.CreatedAtUtc = DateTime.UtcNow;
        this.ModifiedAtUtc = this.CreatedAtUtc;
    }

    // EF constructor
    private ReadModelReference()
    {
        this.ReadModelAssemblyName = string.Empty;
        this.ReadModelClassName = string.Empty;
        this.Key = string.Empty;
        this.ReadModelJson = string.Empty;
    }

    /// <summary>
    /// Gets the read model assembly name
    /// </summary>
    public string ReadModelAssemblyName { get; }

    /// <summary>
    /// Gets the read model class name
    /// </summary>
    public string ReadModelClassName { get; }

    /// <summary>
    /// Gets the read model key
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the JSON representation of the read model
    /// </summary>
    public string ReadModelJson { get; }

    /// <summary>
    /// Gets the date-time in UTC at which the entity was created
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the date-time in UTC at which the entity was last updated
    /// </summary>
    public DateTime ModifiedAtUtc { get; private set; }

    /// <inheritdoc/>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    public byte[] Timestamp { get; protected set; } = default!;
}
