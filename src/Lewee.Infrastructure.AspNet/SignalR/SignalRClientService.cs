using Lewee.Domain;
using Microsoft.AspNetCore.Http;

namespace Lewee.Infrastructure.AspNet.SignalR;

internal class SignalRClientService : IClientService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public SignalRClientService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string? ClientId => this.httpContextAccessor
        ?.HttpContext
        ?.Items?[SignalRClientIdMiddleware.ClientIdHttpContextId]
        ?.ToString();
}
