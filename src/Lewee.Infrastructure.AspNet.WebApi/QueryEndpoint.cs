using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Correlate;
using FastEndpoints;
using Lewee.Application.Mediation.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.WebApi;

/// <summary>
/// Query Endpoint
/// </summary>
/// <typeparam name="T">Response type</typeparam>
public abstract class QueryEndpoint<T> : EndpointWithoutRequest<T>
    where T : class
{
    /// <summary>
    /// Gets the mediator
    /// </summary>
    [NotNull]
    protected IMediator Mediator => this.HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    protected Guid CorrelationId => this.CorrelationContextAccessor.GetCorrelationId();

    /// <summary>
    /// Gets the user ID of the authenticated user if authenticated, otherwise null
    /// </summary>
    protected string? UserId => this.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <summary>
    /// Gets the endpoint route
    /// </summary>
    protected abstract string Route { get; }

    /// <summary>
    /// Gets the endpoint name
    /// </summary>
    protected abstract string Name { get; }

    /// <summary>
    /// Gets a value indicating whether anonymous requests are allowed
    /// </summary>
    protected abstract bool IsAnonymousAllowed { get; }

    [NotNull]
    private ICorrelationContextAccessor CorrelationContextAccessor
        => this.HttpContext.RequestServices.GetRequiredService<ICorrelationContextAccessor>();

    /// <inheritdoc />
    public override void Configure()
    {
        this.Get(this.Route);

        if (this.IsAnonymousAllowed)
        {
            this.AllowAnonymous();
        }

        this.Description(x =>
        {
            var builder = x
                .WithName(this.Name)
                .Produces<T>();
            this.ConfigureErrorRouteHandling(builder);
        });
    }

    /// <summary>
    /// Configures the error route handling
    /// </summary>
    /// <param name="builder">Route handler builder</param>
    protected virtual void ConfigureErrorRouteHandling(RouteHandlerBuilder builder)
    {
    }

    /// <summary>
    /// Executes the the FastEndpoint based on the query result
    /// </summary>
    /// <param name="result">Query result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task</returns>
    /// <exception cref="NotSupportedException">Thrown when result status is not valid for this type of request</exception>
    protected async Task ToResponse(QueryResult<T> result, CancellationToken cancellationToken)
    {
        switch (result.Status)
        {
            case ResultStatus.Success:
                await this.SendOkAsync(result.Data!, cancellationToken);
                return;

            case ResultStatus.NotFound:
                await this.SendNotFoundAsync(cancellationToken);
                return;

            case ResultStatus.Unauthenticated:
                await this.SendUnauthorizedAsync(cancellationToken);
                return;

            case ResultStatus.Unauthorized:
                await this.SendForbiddenAsync(cancellationToken);
                return;

            default:
                throw new NotSupportedException($"Query result status '{result.Status}' is not supported.");
        }
    }
}
