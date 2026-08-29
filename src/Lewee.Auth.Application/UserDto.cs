namespace Lewee.Auth.Application;

/// <summary>
/// User data sent to clients.
/// </summary>
public sealed class UserDto
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the external identity.
    /// </summary>
    public string ExternalId { get; set; }
}
