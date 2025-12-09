using Lewee.Blazor;
using Pizzeria.Common;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class MessagingConfiguration
{
    public static IServiceCollection AddSignalRMessaging(this IServiceCollection services, bool isDevelopment)
    {
        services.AddLeweeBlazor<MessageToActionMapper>(
            ServiceNames.PizzaStoreApi,
            isDevelopment);

        return services;
    }
}
