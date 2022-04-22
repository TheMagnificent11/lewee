namespace Saji.Domain;

/// <summary>
/// Domain Events Collection
/// </summary>
public class DomainEventsCollection
{
    private readonly List<IDomainEvent> domainEvents = new();
    private readonly object syncLock = new();

    /// <summary>
    /// Raises a domain event
    /// </summary>
    /// <typeparam name="T">
    /// Domain event type
    /// </typeparam>
    /// <param name="domainEvent">
    /// Domain event
    /// </param>
    public void Raise<T>(T domainEvent)
        where T : IDomainEvent
    {
        lock (this.syncLock)
        {
            this.domainEvents.Add(domainEvent);
        }
    }

    /// <summary>
    /// Gets the domains events in the collection and then clears the collection
    /// </summary>
    /// <returns>
    /// An array of domain events
    /// </returns>
    public IDomainEvent[] GetAndClear()
    {
        lock (this.syncLock)
        {
            var events = this.domainEvents.ToArray();
            this.domainEvents.Clear();

            return events;
        }
    }
}
