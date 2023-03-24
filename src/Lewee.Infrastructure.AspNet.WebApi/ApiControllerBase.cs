using Lewee.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    /// Initializes a new instance of the <see cref="ApiControllerBase"/> class
    /// </summary>
    /// <param name="mediator">Mediator</param>
    protected ApiControllerBase(IMediator mediator)
    {
        this.Mediator = mediator;
    }

    /// <summary>
    /// Gets the mediator
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// Gets the correlation ID from a HTTP header if it exists and create it if it does not exist
    /// </summary>
    protected Guid CorrelationId
    {
        get
        {
            if (this.HttpContext?.Request?.Headers == null)
            {
                return Guid.NewGuid();
            }

            if (this.HttpContext.Request.Headers.ContainsKey(LoggingConsts.CorrelationIdHeaderKey) &&
                Guid.TryParse(
                    this.HttpContext.Request.Headers[LoggingConsts.CorrelationIdHeaderKey],
                    out var correlationId))
            {
                return correlationId;
            }

            return Guid.NewGuid();
        }
    }
}
