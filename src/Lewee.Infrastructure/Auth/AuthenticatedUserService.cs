using System.Security.Claims;
using Lewee.Application.Auth;
using Microsoft.AspNetCore.Http;

namespace Lewee.Infrastructure.Auth;

/// <summary>
/// Authenticated User Service
/// </summary>
public class AuthenticatedUserService : IAuthenticatedUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticatedUserService"/> class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the user ID of the authenticated user if a user is authenticated, otherwise return null
    /// </summary>
    public string? UserId => this.httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
