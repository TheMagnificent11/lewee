using Lewee.Application.Mediation;
using Lewee.Application.Mediation.Responses;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application.Menu;

public sealed class GetMenuItemsQuery : IQuery<QueryResult<IEnumerable<MenuItemDto>>>
{
    public GetMenuItemsQuery(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }

    internal class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, QueryResult<IEnumerable<MenuItemDto>>>
    {
        private readonly IRestaurantDbContext dbContext;
        private readonly IMapper mapper;

        public GetMenuItemsQueryHandler(IRestaurantDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<QueryResult<IEnumerable<MenuItemDto>>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var entities = await this.dbContext
                .AggregateRoot<MenuItem>()
                .OrderBy(x => x.ItemTypeId)
                .ThenBy(x => x.Name)
                .ToArrayAsync(cancellationToken);

            var dtos = this.mapper.Map<IEnumerable<MenuItemDto>>(entities);

            return QueryResult<IEnumerable<MenuItemDto>>.Success(dtos);
        }
    }
}
