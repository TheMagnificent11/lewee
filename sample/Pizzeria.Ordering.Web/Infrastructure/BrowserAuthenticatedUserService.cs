using Lewee.Common;

namespace Pizzeria.Ordering.Web.Infrastructure;

internal sealed class BrowserAuthenticatedUserService : IAuthenticatedUserService
{
    public string? UserId { get; private set; }

    public void SetUserId(string? userId)
    {
        this.UserId = userId;
    }
}
