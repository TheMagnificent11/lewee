using System.Diagnostics.CodeAnalysis;
using Lewee.Application.Mediation.Requests;
using Lewee.Auth.Application;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Pizzeria.Common;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public sealed record GetOrdersQuery(OrderListFilter Filter = OrderListFilter.Current)
    : IQuery<IEnumerable<OrderDto>>, ITenantRoleRequest
{
    public Guid TenantId => PizzaStore.Tenant.Id;

    public IReadOnlyCollection<string> Roles { get; } =
        [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode];

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<GetOrdersQuery, QueryResult<IEnumerable<OrderDto>>>
    {
        private readonly IRepository<Order> orderRepository;

        public Handler(IRepository<Order> orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<QueryResult<IEnumerable<OrderDto>>> Handle(
            GetOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await this.orderRepository.QueryAsync(
                new GetOrdersQuerySpec(request.Filter),
                cancellationToken);

            var result = orders
                .OrderBy(x => x.SubmittedDateTime)
                .Select(OrderDtoFactory.Create)
                .ToArray();

            return QueryResult<IEnumerable<OrderDto>>.Success(result);
        }
    }
}
