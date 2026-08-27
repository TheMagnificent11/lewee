using System.Security.Claims;
using Lewee.Auth.Api;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Pizzeria.Store.StateManagement;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class TokenValidatedContextExtensions
{
    public static async Task CreateUserOnFirstLoginAsync(this TokenValidatedContext context)
    {
        var externalUserId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(externalUserId))
        {
            return;
        }

        var apiClient = context.HttpContext.RequestServices.GetRequiredService<IBffApiClient>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IBffApiClient>>();

        try
        {
            await apiClient.CreateUserAsync(
                new CreateUserRequest { ExternalUserId = externalUserId },
                context.HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            // Log but don't fail authentication - user might already exist
            // or API might be temporarily unavailable
            logger.LogError(
                ex,
                "Failed to create user during first login for external user {ExternalUserId}",
                externalUserId);
        }
    }
}
