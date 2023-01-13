using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Events;

/// <summary>
/// Service Bus Configuration
/// </summary>
public static class ServiceBusConfiguration
{
    /// <summary>
    /// Configures a service bus publisher
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="settings">Service bus settings</param>
    /// <returns>Services collection for chaining</returns>
    public static IServiceCollection ConfigureServiceBusPublisher(this IServiceCollection services, ServiceBusSettings settings)
    {
        services.AddMassTransit(options =>
        {
            switch (settings.BusType)
            {
                case ServiceBusType.RabbitMQ:
                    options.UsingRabbitMq((context, config) =>
                    {
                        config.Host(settings.ConnectionStringOrHost, "/");
                        config.ConfigureEndpoints(context);
                    });

                    break;

                default:
                    throw new InvalidOperationException("Invalid service bus type");
            }
        });

        services.AddMediatR(typeof(ServiceBusSettings).Assembly); // adds ServiceBusEventHandler to publish events to bus

        return services;
    }
}
