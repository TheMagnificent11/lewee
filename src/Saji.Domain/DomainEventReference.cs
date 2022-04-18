using System.Reflection;
using System.Text.Json;

namespace Saji.Domain;

/// <summary>
/// Domain Event Reference
/// </summary>
public class DomainEventReference
{
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
    /// Gets or sets teh UTC date-time at which the domain event was dispatched
    /// </summary>
    public DateTime? DispatchedAt { get; protected set; }

    /// <summary>
    /// Converts the provided <see cref="IDomainEvent"/> into a <see cref="DomainEventReference"/>
    /// </summary>
    /// <param name="domainEvent">
    /// The domain event to convert
    /// </param>
    /// <returns>
    /// A <see cref="DomainEventReference"/>
    /// </returns>
    public static DomainEventReference From(IDomainEvent domainEvent)
    {
        var domainEventType = domainEvent.GetType();

        var reference = new DomainEventReference
        {
            Id = Guid.NewGuid(),
            DomainEventAssemblyName = domainEventType.Assembly.GetName().Name,
            DomainEventClassName = domainEventType.FullName,
            DomainEventJson = JsonSerializer.Serialize(domainEvent, domainEventType),
            PersistedAt = DateTime.UtcNow,
            Dispatched = false
        };

        return reference;
    }

    /// <summary>
    /// Coverts this <see cref="DomainEventReference"/> to a <see cref="IDomainEvent"/>
    /// </summary>
    /// <returns>
    /// A <see cref="IDomainEvent"/>
    /// </returns>
    public IDomainEvent ToDomainEvent()
    {
        if (string.IsNullOrWhiteSpace(this.DomainEventJson))
        {
            return null;
        }

        var assembly = Assembly.Load(this.DomainEventAssemblyName);
        var targetType = assembly.GetType(this.DomainEventClassName);

        var domainEvent = JsonSerializer.Deserialize(this.DomainEventJson, targetType);

        return domainEvent as IDomainEvent;
    }
}
