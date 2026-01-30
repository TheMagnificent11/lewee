using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Pizzeria.Store.Infrastructure;

internal static class TokenValidatedContextExtensions
{
    public static async Task CreateCustomerOnFirstLoginAsync(this TokenValidatedContext context)
    {
        var externalUserId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(externalUserId))
        {
            var apiClient = context.HttpContext.RequestServices.GetRequiredService<IStoreApiClient>();

            try
            {
                await apiClient.CreateCustomerAsync(
                    new CreateCustomerApiRequest(externalUserId),
                    context.HttpContext.RequestAborted);
            }
            catch
            {
                // Ignore errors - user might already exist or API might be temporarily unavailable
                // This shouldn't prevent the user from accessing the application
            }
        }
    }
}
