using Lewee.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Pizzeria.Store.Web.Infrastructure.ServerSentEvents;

public static class SseConfiguration
{
    public static IServiceCollection AddSseMessageReceiver<TMapper>(this IServiceCollection services)
        where TMapper : class, IMessageToActionMapper
    {
        services.AddClientEventBroadcaster();
        services.AddScoped<IMessageToActionMapper, TMapper>();

        return services;
    }
}
