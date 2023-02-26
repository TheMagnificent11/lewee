using System.Reflection;
using FluentValidation;
using Lewee.Application.Mediation.Behaviors;
using Mapster;
using MapsterMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Application;

/// <summary>
/// Application Configuration
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Adds application dependencies
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="applicationAssembly">Application assembly</param>
    /// <param name="domainAssembly">Domain assembly</param>
    /// <returns>Service collection (for chaining)</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Assembly applicationAssembly,
        Assembly domainAssembly)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(applicationAssembly, domainAssembly));
        services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);

        return services;
    }

    /// <summary>
    /// Adds pipeline behaviors
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="additionalBehaviors">Additional behaviors</param>
    /// <returns>Service collection (for chaining)</returns>
    public static IServiceCollection AddPipelineBehaviors(
        this IServiceCollection services,
        params Type[] additionalBehaviors)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CorrelationIdLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        if (additionalBehaviors != null && additionalBehaviors.Any())
        {
            foreach (var item in additionalBehaviors)
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), item);
            }
        }

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FailureLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds type mapper
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Mapster type addapter config</returns>
    public static TypeAdapterConfig AddMapper(this IServiceCollection services)
    {
        services.AddTransient<IMapper, ServiceMapper>();

        var config = new TypeAdapterConfig();
        services.AddSingleton(config);

        return config;
    }
}
