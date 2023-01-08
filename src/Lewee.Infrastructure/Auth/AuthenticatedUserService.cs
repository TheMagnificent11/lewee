using System.Security.Claims;
using Lewee.Application.Auth;
using Microsoft.AspNetCore.Http;

namespace Lewee.Infrastructure.Auth;

internal class AuthenticatedUserService : IAuthenticatedUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => this.httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
