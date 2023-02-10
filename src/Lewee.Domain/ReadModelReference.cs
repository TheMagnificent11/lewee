using System.Reflection;
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
        var (assemblyName, fullCalssName, readModelType) = readModel.GetBasicTypeInfo("Invalid read model type");

        this.Id = Guid.NewGuid();
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
    /// Gets or sets the ID
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Gets or sets the read model assembly name
    /// </summary>
    public string ReadModelAssemblyName { get; protected set; }

    /// <summary>
    /// Gets or sets the read model class name
    /// </summary>
    public string ReadModelClassName { get; protected set; }

    /// <summary>
    /// Gets or sets the read model key
    /// </summary>
    public string Key { get; protected set; }

    /// <summary>
    /// Gets or sets the JSON representation of the read model
    /// </summary>
    public string ReadModelJson { get; protected set; }

    /// <summary>
    /// Gets or sets the date-time in UTC at which the entity was created
    /// </summary>
    public DateTime CreatedAtUtc { get; protected set; }

    /// <summary>
    /// Gets or sets the date-time in UTC at which the entity was last updated
    /// </summary>
    public DateTime ModifiedAtUtc { get; protected set; }

    /// <inheritdoc/>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    public byte[] Timestamp { get; protected set; } = default!;

    /// <summary>
    /// Converts this <see cref="ReadModelReference"/> to a <see cref="IReadModel"/>
    /// </summary>
    /// <returns>A <see cref="IReadModel"/></returns>
    public IReadModel? ToReadModel()
    {
        if (string.IsNullOrWhiteSpace(this.ReadModelJson))
        {
            return null;
        }

        var assembly = Assembly.Load(this.ReadModelAssemblyName);
        var targetType = assembly.GetType(this.ReadModelClassName);

        if (targetType == null)
        {
            return null;
        }

        var readModel = JsonSerializer.Deserialize(this.ReadModelJson, targetType);

        return readModel as IReadModel;
    }

    /// <summary>
    /// Updates the JSON
    /// </summary>
    /// <param name="readModel">New version of the read model</param>
    public void UpdateJson(IReadModel readModel)
    {
        this.ReadModelJson = JsonSerializer.Serialize(readModel, readModel.GetType());
        this.ModifiedAtUtc = DateTime.UtcNow;
    }
}
