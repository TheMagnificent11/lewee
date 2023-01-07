﻿using Lewee.Application.Mediation;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application.Tables;

public sealed class GetTablesQuery : IQuery<IEnumerable<TableDto>>
{
    public GetTablesQuery(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid? TenantId { get; }
    public Guid CorrelationId { get; }

    internal class GetTablesQueryHandler : IRequestHandler<GetTablesQuery, IEnumerable<TableDto>>
    {
        private readonly IRestaurantDbContext dbContext;
        private readonly IMapper mapper;

        public GetTablesQueryHandler(IRestaurantDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TableDto>> Handle(GetTablesQuery request, CancellationToken cancellationToken)
        {
            var entites = await this.dbContext
                .AggregateRoot<Table>()
                .OrderBy(x => x.TableNumber)
                .ToListAsync(cancellationToken);

            return this.mapper.Map<IEnumerable<TableDto>>(entites);
        }
    }
}
