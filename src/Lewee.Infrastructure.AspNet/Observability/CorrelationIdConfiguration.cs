﻿using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using Lewee.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.Observability;

/// <summary>
/// Correlation ID Configuration
/// </summary>
public static class CorrelationIdConfiguration
{
    /// <summary>
    /// Adds `Correlate` to services collection
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddCorrelationIdServices(this IServiceCollection services)
    {
        services.AddCorrelate(options => options.RequestHeaders = new[] { RequestHeaders.CorrelationId });

        return services;
    }

    /// <summary>
    /// Adds `Correlate` middleware
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>The updated application builder</returns>
    public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app)
    {
        app.UseCorrelate();

        return app;
    }
}
