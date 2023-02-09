namespace Lewee.Domain;

/// <summary>
/// Read Model Reference
/// </summary>
public class ReadModelReference : ISoftDeleteEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadModelReference"/> class
    /// </summary>
    /// <param name="type">Read model type</param>
    /// <param name="key">Read model key</param>
    /// <param name="readModelJson">The JSON representation of the read model</param>
    public ReadModelReference(string type, string key, string readModelJson)
    {
        this.Type = type;
        this.Key = key;
        this.ReadModelJson = readModelJson;
    }

    // EF constructor
    private ReadModelReference()
    {
        this.Type = string.Empty;
        this.Key = string.Empty;
        this.ReadModelJson = string.Empty;
    }

    /// <summary>
    /// Gets the read model type
    /// </summary>
    public string Type { get; }

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
