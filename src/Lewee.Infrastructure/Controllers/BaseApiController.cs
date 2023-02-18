using Lewee.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lewee.Infrastructure.Controllers;

/// <summary>
/// Base API Controller
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApiController"/> class
    /// </summary>
    /// <param name="mediator">Mediator</param>
    protected BaseApiController(IMediator mediator)
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
