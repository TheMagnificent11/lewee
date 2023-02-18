using Lewee.Infrastructure.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Api.Controllers.V1;

[Route("api/v1/[controller]")]
public sealed class TablesController : BaseApiController
{
    public TablesController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TableDto>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetTablesQuery(this.CorrelationId);
        var result = await this.Mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{tableNumber}")]
    [ProducesResponseType(200, Type = typeof(TableDetailsDto))]
    public async Task<IActionResult> GetOne([FromRoute] int tableNumber, CancellationToken cancellationToken = default)
    {
        var query = new GetTableDetailsQuery(this.CorrelationId, tableNumber);
        var result = await this.Mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{tableNumber}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Use([FromRoute] int tableNumber, CancellationToken cancellationToken = default)
    {
        var command = new UseTableCommand(this.CorrelationId, tableNumber);
        var result = await this.Mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{tableNumber}/menu-items/{menuItemId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddToOrder(
        [FromRoute] int tableNumber,
        [FromRoute] Guid menuItemId,
        CancellationToken cancellationToken = default)
    {
        var command = new AddMenuItemCommand(this.CorrelationId, tableNumber, menuItemId);
        var result = await this.Mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}
