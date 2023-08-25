using System.Diagnostics.CodeAnalysis;
using Correlate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.WebApi;

/// <summary>
/// API Controller Base
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Application/JSON
    /// </summary>
    protected const string ApplicationJson = "application/json";

    /// <summary>
    /// Gets the mediator
    /// </summary>
    [NotNull]
    protected IMediator Mediator => this.HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    protected Guid CorrelationId
    {
        get
        {
            var cid = this.CorrelationContextAccessor?.CorrelationContext?.CorrelationId;

            if (cid == null || !Guid.TryParse(cid, out var correlationId))
            {
                return Guid.NewGuid();
            }

            return correlationId;
        }
    }

    [NotNull]
    private ICorrelationContextAccessor CorrelationContextAccessor
        => this.HttpContext.RequestServices.GetRequiredService<ICorrelationContextAccessor>();
}
