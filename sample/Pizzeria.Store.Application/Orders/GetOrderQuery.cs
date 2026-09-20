using System.Diagnostics.CodeAnalysis;
using Lewee.Application.Mediation.Requests;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderDto>
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<GetOrderQuery, QueryResult<OrderDto>>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly IAuthenticatedUserService authenticatedUserService;

        public Handler(
            IRepository<Order> orderRepository,
            IAuthenticatedUserService authenticatedUserService)
        {
            this.orderRepository = orderRepository;
            this.authenticatedUserService = authenticatedUserService;
        }

        public async Task<QueryResult<OrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await this.orderRepository.QueryOneAsync(
                new GetOrderQuerySpec(request.OrderId),
                cancellationToken);
            if (order is null)
            {
                return QueryResult<OrderDto>.Fail(ResultStatus.NotFound, $"Order {request.OrderId} not found");
            }

            if (!OrderOwnership.IsOwnedByCaller(order, this.authenticatedUserService))
            {
                return QueryResult<OrderDto>.Fail(ResultStatus.Unauthorized, "Order does not belong to the caller.");
            }

            return QueryResult<OrderDto>.Success(OrderDtoFactory.Create(order));
        }
    }
}
