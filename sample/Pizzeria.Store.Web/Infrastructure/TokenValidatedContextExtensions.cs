using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Pizzeria.Store.Application.Customers;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class TokenValidatedContextExtensions
{
    public static async Task CreateCustomerOnFirstLoginAsync(this TokenValidatedContext context)
    {
        var externalUserId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(externalUserId))
        {
            var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
            var createCustomerCommand = new CreateCustomerCommand(
                externalUserId,
                CorrelationId: Guid.NewGuid());

            try
            {
                _ = await mediator.Send(createCustomerCommand, context.HttpContext.RequestAborted);
            }
            catch
            {
                // Ignore errors - user might already exist or API might be temporarily unavailable
                // This shouldn't prevent the user from accessing the application
            }
        }
    }
}
