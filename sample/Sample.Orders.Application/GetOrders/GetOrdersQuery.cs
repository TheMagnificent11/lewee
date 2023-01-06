using Lewee.Application.Mediation;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Orders.Domain;

namespace Sample.Orders.Application.GetOrders;

public class GetOrdersQuery : IQuery<IEnumerable<OrderDto>>
{
    public GetOrdersQuery(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
    public Guid? TenantId { get; }

    internal class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrdersDbContext dbContext;
        private readonly IMapper mapper;

        public GetOrdersQueryHandler(IOrdersDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await this.dbContext
                .AggregateRoot<Order>()
                .ToListAsync(cancellationToken);

            return this.mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}
