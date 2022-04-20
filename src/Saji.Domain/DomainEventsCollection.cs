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
    /// Clears domain events
    /// </summary>
    public void Clear()
    {
        lock (this.syncLock)
        {
            this.domainEvents.Clear();
        }
    }
}
