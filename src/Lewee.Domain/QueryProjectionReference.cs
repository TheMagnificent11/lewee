using System.Reflection;
using System.Text.Json;

namespace Lewee.Domain;

/// <summary>
/// Query Projection Reference
/// </summary>
public class QueryProjectionReference : ISoftDeleteEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryProjectionReference"/> class
    /// </summary>
    /// <param name="readModel">Query projection</param>
    /// <param name="key">Query projection key</param>
    public QueryProjectionReference(IQueryProjection readModel, string key)
    {
        var (assemblyName, fullCalssName, readModelType) = readModel.GetBasicTypeInfo("Invalid query projection type");

        this.Id = Guid.NewGuid();
        this.Key = key;
        this.QueryProjectionAssemblyName = assemblyName;
        this.QueryProjectionClassName = fullCalssName;
        this.QueryProjectionJson = JsonSerializer.Serialize(readModel, readModelType);
        this.CreatedAtUtc = DateTime.UtcNow;
        this.ModifiedAtUtc = this.CreatedAtUtc;
    }

    // EF constructor
    private QueryProjectionReference()
    {
        this.QueryProjectionAssemblyName = string.Empty;
        this.QueryProjectionClassName = string.Empty;
        this.Key = string.Empty;
        this.QueryProjectionJson = "{}";
    }

    /// <summary>
    /// Gets or sets the ID
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Gets or sets the query projection assembly name
    /// </summary>
    public string QueryProjectionAssemblyName { get; protected set; }

    /// <summary>
    /// Gets or sets the query projection class name
    /// </summary>
    public string QueryProjectionClassName { get; protected set; }

    /// <summary>
    /// Gets or sets the query projection key
    /// </summary>
    public string Key { get; protected set; }

    /// <summary>
    /// Gets or sets the JSON representation of the query projection
    /// </summary>
    public string QueryProjectionJson { get; protected set; }

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
    /// Converts this <see cref="QueryProjectionReference"/> to a <see cref="IQueryProjection"/>
    /// </summary>
    /// <returns>A <see cref="IQueryProjection"/></returns>
    public IQueryProjection? ToQueryProjection()
    {
        if (string.IsNullOrWhiteSpace(this.QueryProjectionJson))
        {
            return null;
        }

        var assembly = Assembly.Load(this.QueryProjectionAssemblyName);
        var targetType = assembly.GetType(this.QueryProjectionClassName);

        if (targetType == null)
        {
            return null;
        }

        var readModel = JsonSerializer.Deserialize(this.QueryProjectionJson, targetType);

        return readModel as IQueryProjection;
    }

    /// <summary>
    /// Updates the JSON
    /// </summary>
    /// <param name="readModel">New version of the query projection</param>
    public void UpdateJson(IQueryProjection readModel)
    {
        this.QueryProjectionJson = JsonSerializer.Serialize(readModel, readModel.GetType());
        this.ModifiedAtUtc = DateTime.UtcNow;
    }
}
