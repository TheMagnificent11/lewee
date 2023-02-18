using System.Reflection;
using MassTransit;
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
    public static IServiceCollection ConfigureServiceBusPublisher(
        this IServiceCollection services,
        ServiceBusSettings settings)
    {
        services.AddMassTransit(options =>
        {
            switch (settings.BusType)
            {
                case ServiceBusType.RabbitMQ:
                    ConfigureRabbitMq(settings, options);
                    break;

                default:
                    throw new InvalidOperationException("Invalid service bus type");
            }
        });

        // adds ServiceBusEventHandler to publish events to bus
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ServiceBusSettings).Assembly));

        return services;
    }

    /// <summary>
    /// Configures service bus consumers
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="settings">Service bus settings</param>
    /// <param name="consumerAssemblies">Assemblies containing service bus consumers</param>
    /// <returns>Services collection for chaining</returns>
    public static IServiceCollection ConfigureServiceBusConsumers(
        this IServiceCollection services,
        ServiceBusSettings settings,
        Assembly[] consumerAssemblies)
    {
        consumerAssemblies = consumerAssemblies.Distinct().ToArray();

        services.AddMassTransit(options =>
        {
            options.AddConsumers(consumerAssemblies);

            switch (settings.BusType)
            {
                case ServiceBusType.RabbitMQ:
                    ConfigureRabbitMq(settings, options);
                    break;

                default:
                    throw new InvalidOperationException("Invalid service bus type");
            }
        });

        return services;
    }

    private static void ConfigureRabbitMq(ServiceBusSettings settings, IBusRegistrationConfigurator options)
    {
        options.UsingRabbitMq((context, config) =>
        {
            config.Host(settings.ConnectionStringOrHost, "/");
            config.ConfigureEndpoints(context);
        });
    }
}
