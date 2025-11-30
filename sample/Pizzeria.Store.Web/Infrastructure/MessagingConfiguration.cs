using Lewee.Blazor;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class MessagingConfiguration
{
    public static IServiceCollection AddSignalRMessaging(this IServiceCollection services, bool isDevelopment)
    {
        services.AddLeweeBlazor<MessageToActionMapper>(
            ApiCientConfiguration.ApiClientName,
            isDevelopment);

        return services;
    }
}
