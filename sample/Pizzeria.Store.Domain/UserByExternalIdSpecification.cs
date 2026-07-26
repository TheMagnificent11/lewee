using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public sealed class UserByExternalIdSpecification : QuerySpecification<User>
{
    public UserByExternalIdSpecification(string externalId)
    {
        this.Query.Where(u => u.ExternalId == externalId);
    }
}
