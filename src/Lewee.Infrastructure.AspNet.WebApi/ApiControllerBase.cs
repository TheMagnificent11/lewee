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
}
