using System.Reflection;
using Correlate.DependencyInjection;
using FluentValidation;
using Lewee.Application.Mediation.Behaviors;
using MediatR;
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
        var leweeApplicationAssembly = typeof(ApplicationConfiguration).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(
            applicationAssembly,
            domainAssembly,
            leweeApplicationAssembly));
        services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);
        services.AddPipelineBehaviors();

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
        services.AddCorrelate();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CorrelationIdLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FailureLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        if (additionalBehaviors != null && additionalBehaviors.Length > 0)
        {
            foreach (var item in additionalBehaviors)
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), item);
            }
        }

        return services;
    }
}
