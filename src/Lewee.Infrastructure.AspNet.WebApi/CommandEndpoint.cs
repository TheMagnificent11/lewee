using System.Diagnostics.CodeAnalysis;
using Correlate;
using FastEndpoints;
using Lewee.Application.Mediation.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.WebApi;

/// <summary>
/// Command Endpoint
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
public abstract class CommandEndpoint<TRequest> : Endpoint<TRequest, EmptyResponse>
    where TRequest : notnull
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
    /// Gets the endpoint route
    /// </summary>
    protected abstract string Route { get; }

    /// <summary>
    /// Gets the command type
    /// </summary>
    protected abstract CommandType CommandType { get; }

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
        switch (this.CommandType)
        {
            case CommandType.Put:
                this.Put(this.Route);
                break;

            case CommandType.Patch:
                this.Patch(this.Route);
                break;

            case CommandType.Delete:
                this.Delete(this.Route);
                break;

            default:
                this.Post(this.Route);
                break;
        }

        if (this.IsAnonymousAllowed)
        {
            this.AllowAnonymous();
        }

        this.Description(x =>
        {
            var builder = x
                .WithName(this.Name)
                .Produces(200);
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
    /// Executes the the FastEndpoint based on the command result
    /// </summary>
    /// <param name="result">Command result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task</returns>
    /// <exception cref="NotSupportedException">Thrown when result status is not valid for this type of request</exception>
    protected async Task ToResponse(CommandResult result, CancellationToken cancellationToken)
    {
        switch (result.Status)
        {
            case ResultStatus.Success:
                await this.SendOkAsync(cancellationToken);
                return;

            case ResultStatus.BadRequest:
                result.Errors.ForEach(x => this.AddError(x));
                this.ThrowIfAnyErrors();
                break;

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
                throw new NotSupportedException($"Command result status '{result.Status}' is not supported.");
        }
    }
}
