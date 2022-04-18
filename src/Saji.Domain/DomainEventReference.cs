using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
    /// Gets or sets the domain event type (full class path)
    /// </summary>
    public string DomainEventType { get; protected set; }

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
        var reference = new DomainEventReference
        {
            Id = Guid.NewGuid(),
            DomainEventJson = JsonSerializer.Serialize(domainEvent),
            DomainEventType = domainEvent.GetType().FullName,
            PersistedAt = DateTime.UtcNow,
            Dispatched = false,
            DispatchedAt = null
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

        var targetType = Type.GetType(this.DomainEventType);

        var domainEvent = JsonSerializer.Deserialize(this.DomainEventJson, targetType);

        return (IDomainEvent)domainEvent;
    }
}
