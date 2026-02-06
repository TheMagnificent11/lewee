using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Pizzeria.Store.Contracts.Users;
using Pizzeria.Store.StateManagement;

namespace Pizzeria.Store.Web;

internal static class TokenValidatedContextExtensions
{
    public static async Task CreateCustomerOnFirstLoginAsync(this TokenValidatedContext context)
    {
        var externalUserId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(externalUserId))
        {
            return;
        }

        var apiClient = context.HttpContext.RequestServices.GetRequiredService<IStoreApiClient>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IStoreApiClient>>();

        try
        {
            await apiClient.CreateCustomerAsync(
                new CreateCustomerRequest { ExternalUserId = externalUserId },
                context.HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            // Log but don't fail authentication - user might already exist
            // or API might be temporarily unavailable
            logger.LogError(
                ex,
                "Failed to create customer during first login for user {ExternalUserId}",
                externalUserId);
        }
    }
}
