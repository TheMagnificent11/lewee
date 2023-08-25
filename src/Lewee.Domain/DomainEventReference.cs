using System.Reflection;
using System.Text.Json;

namespace Lewee.Domain;

/// <summary>
/// Domain Event Reference
/// </summary>
public class DomainEventReference
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventReference"/> class
    /// </summary>
    /// <param name="domainEvent">
    /// Underlying domain event
    /// </param>
    /// <param name="userId">
    /// User ID
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if type of the domain event cannot be determined
    /// </exception>
    public DomainEventReference(DomainEvent domainEvent, string? userId = null)
    {
        var (assemblyName, fullClassName, domainEventType) = domainEvent.GetBasicTypeInfo("Invalid domain event type");

        this.Id = Guid.NewGuid();
        this.DomainEventAssemblyName = assemblyName;
        this.DomainEventClassName = fullClassName;
        this.DomainEventJson = JsonSerializer.Serialize(domainEvent, domainEventType);
        this.PersistedAt = DateTime.UtcNow;
        this.Dispatched = false;
        this.UserId = userId;
    }

    // EF constructor
    private DomainEventReference()
    {
        this.DomainEventAssemblyName = string.Empty;
        this.DomainEventClassName = string.Empty;
        this.DomainEventJson = "{}";
        this.PersistedAt = DateTime.UtcNow;
        this.Dispatched = false;
    }

    /// <summary>
    /// Gets or sets the ID
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Gets or sets the assembly name of the underlying domain event
    /// </summary>
    public string DomainEventAssemblyName { get; protected set; }

    /// <summary>
    /// Gets or sets the class name of the underlying domain event
    /// </summary>
    public string DomainEventClassName { get; protected set; }

    /// <summary>
    /// Gets or sets the JSON representation of the underlying domain event
    /// </summary>
    public string DomainEventJson { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether the domain event has been dispatched
    /// </summary>
    public bool Dispatched { get; protected set; }

    /// <summary>
    /// Gets or sets the UTC date-time at which the domain event was persisted
    /// </summary>
    public DateTime PersistedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the UTC date-time at which the domain event was dispatched
    /// </summary>
    public DateTime? DispatchedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public string? UserId { get; protected set; }

    /// <summary>
    /// Coverts this <see cref="DomainEventReference"/> to a <see cref="DomainEvent"/>
    /// </summary>
    /// <returns>
    /// A <see cref="DomainEvent"/>
    /// </returns>
    public DomainEvent? ToDomainEvent()
    {
        if (string.IsNullOrWhiteSpace(this.DomainEventJson))
        {
            return null;
        }

        var assembly = Assembly.Load(this.DomainEventAssemblyName);
        var targetType = assembly.GetType(this.DomainEventClassName);

        if (targetType == null)
        {
            return null;
        }

        var objDomainEvent = JsonSerializer.Deserialize(this.DomainEventJson, targetType);
        if (objDomainEvent is not DomainEvent domainEvent)
        {
            return null;
        }

        domainEvent.UserId = this.UserId;

        return domainEvent;
    }

    /// <summary>
    /// Marks the underlying domain event as dispatched
    /// </summary>
    public void Dispatch()
    {
        this.Dispatched = true;
        this.DispatchedAt = DateTime.UtcNow;
    }
}
