using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Pizzas;

public sealed class GetPizzasQuery : IQuery<PizzaDto[]>
{
    public GetPizzasQuery(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }

    internal sealed class Handler : IRequestHandler<GetPizzasQuery, QueryResult<PizzaDto[]>>
    {
        private readonly IRepository<Pizza> repository;

        public Handler(IRepository<Pizza> repository)
        {
            this.repository = repository;
        }

        public async Task<QueryResult<PizzaDto[]>> Handle(GetPizzasQuery request, CancellationToken cancellationToken)
        {
            var pizzas = await this.repository.AllAsync(cancellationToken);

            var result = pizzas
                .Select(x => new PizzaDto(
                    x.Id,
                    x.Name,
                    x.Description,
                    x.Price))
                .ToArray();

            return QueryResult<PizzaDto[]>.Success(result);
        }
    }
}
