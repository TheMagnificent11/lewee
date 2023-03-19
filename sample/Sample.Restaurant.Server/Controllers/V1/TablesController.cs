using Lewee.Infrastructure.AspNet.WebApi;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Server.Controllers.V1;

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
        [FromHeader] Guid? correlationId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTablesQuery(correlationId ?? this.CorrelationId);
        var result = await this.Mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{tableNumber}", Name = "GetDetails")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TableDetailsDto))]
    public async Task<IActionResult> GetOne(
        [FromRoute] int tableNumber,
        [FromHeader] Guid? correlationId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTableDetailsQuery(correlationId ?? this.CorrelationId, tableNumber);
        var result = await this.Mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{tableNumber}", Name = "Use")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Use(
        [FromRoute] int tableNumber,
        [FromHeader] Guid? correlationId,
        CancellationToken cancellationToken = default)
    {
        var command = new UseTableCommand(correlationId ?? this.CorrelationId, tableNumber);
        var result = await this.Mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{tableNumber}/menu-items/{menuItemId}", Name = "OrderMenuItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToOrder(
        [FromRoute] int tableNumber,
        [FromRoute] Guid menuItemId,
        [FromHeader] Guid? correlationId,
        CancellationToken cancellationToken = default)
    {
        var command = new AddMenuItemCommand(
            correlationId ?? this.CorrelationId,
            tableNumber,
            menuItemId);
        var result = await this.Mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{tableNumber}/menu-items/{menuItemId}", Name = "RemoveMenuItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromToOrder(
        [FromRoute] int tableNumber,
        [FromRoute] Guid menuItemId,
        [FromHeader] Guid? correlationId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveMenuItemCommand(
            correlationId ?? this.CorrelationId,
            tableNumber,
            menuItemId);
        var result = await this.Mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}
