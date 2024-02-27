using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Sample.Pizzeria.Contracts;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Application;

public class GetUserOrdersQuery : IQuery<OrderDto[]>
{
    public GetUserOrdersQuery(string userId, Guid correlationId)
    {
        this.UserId = userId;
        this.CorrelationId = correlationId;
    }

    public string UserId { get; }
    public Guid CorrelationId { get; }

    internal class Handler : IRequestHandler<GetUserOrdersQuery, QueryResult<OrderDto[]>>
    {
        private readonly IRepository<Order> repository;

        public Handler(IRepository<Order> repository)
        {
            this.repository = repository;
        }

        public async Task<QueryResult<OrderDto[]>> Handle(
            GetUserOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await this.repository.Query(
                new OrdersForUserQuerySpecification(request.UserId),
                cancellationToken);

            if (orders.Count == 0)
            {
                return QueryResult<OrderDto[]>.Success([]);
            }

            var result = orders
                .Select(x => x.ToDto())
                .ToArray();

            return QueryResult<OrderDto[]>.Success(result);
        }
    }
}
