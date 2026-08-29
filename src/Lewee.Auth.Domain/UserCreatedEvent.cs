using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// Raised when a user is created.
/// </summary>
public sealed class UserCreatedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCreatedEvent"/> class.
    /// </summary>
    /// <param name="userEntityId">User entity ID.</param>
    /// <param name="externalId">External identity.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public UserCreatedEvent(Guid userEntityId, string externalId, Guid correlationId)
        : base(correlationId)
    {
        this.UserEntityId = userEntityId;
        this.ExternalId = externalId;
    }

    /// <summary>
    /// Gets the user entity ID.
    /// </summary>
    public Guid UserEntityId { get; init; }

    /// <summary>
    /// Gets the external identity.
    /// </summary>
    public string ExternalId { get; init; }
}
