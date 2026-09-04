using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// Finds a user by external identity.
/// </summary>
public sealed class UserByExternalIdSpecification : QuerySpecification<User>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserByExternalIdSpecification"/> class.
    /// </summary>
    /// <param name="externalId">External identity.</param>
    public UserByExternalIdSpecification(string externalId)
    {
        this.ExternalId = externalId;
        this.Query.Where(user => user.ExternalId == externalId);
    }

    /// <summary>
    /// Gets the external identity being queried for.
    /// </summary>
    public string ExternalId { get; }
}
