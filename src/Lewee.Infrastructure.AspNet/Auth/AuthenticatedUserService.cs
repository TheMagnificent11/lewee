using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Lewee.Domain;
using Microsoft.AspNetCore.Http;

namespace Lewee.Infrastructure.AspNet.Auth;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Use via DI")]
internal sealed class AuthenticatedUserService : IAuthenticatedUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => this.httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
