using Lewee.Infrastructure.AspNet.WebApi;
using Lewee.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Api.Controllers.V1;

[Route("api/v1/[controller]")]
[Produces(ApplicationJson)]
public sealed class TablesController : BaseApiController
{
    public TablesController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpGet(Name = "GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TableDto>))]
    public async Task<IActionResult> GetAll(
        [FromHeader(Name = LoggingConsts.CorrelationIdHeaderKey)] Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTablesQuery(correlationId);
        var result = await this.Mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{tableNumber}", Name = "GetDetails")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TableDetailsDto))]
    public async Task<IActionResult> GetOne(
        [FromHeader(Name = LoggingConsts.CorrelationIdHeaderKey)] Guid correlationId,
        [FromRoute] int tableNumber,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTableDetailsQuery(correlationId, tableNumber);
        var result = await this.Mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{tableNumber}", Name = "Use")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Use(
        [FromHeader(Name = LoggingConsts.CorrelationIdHeaderKey)] Guid correlationId,
        [FromRoute] int tableNumber,
        CancellationToken cancellationToken = default)
    {
        var command = new UseTableCommand(correlationId, tableNumber);
        var result = await this.Mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{tableNumber}/menu-items/{menuItemId}", Name = "OrderMenuItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToOrder(
        [FromHeader(Name = LoggingConsts.CorrelationIdHeaderKey)] Guid correlationId,
        [FromRoute] int tableNumber,
        [FromRoute] Guid menuItemId,
        CancellationToken cancellationToken = default)
    {
        var command = new AddMenuItemCommand(correlationId, tableNumber, menuItemId);
        var result = await this.Mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}
