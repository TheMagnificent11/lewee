﻿using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MapsterMapper;
using MediatR;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application;

public sealed class GetTablesQuery : IQuery<IEnumerable<TableDto>>
{
    public GetTablesQuery(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }

    internal class GetTablesQueryHandler : IRequestHandler<GetTablesQuery, QueryResult<IEnumerable<TableDto>>>
    {
        private readonly IRepository<Table> repository;
        private readonly IMapper mapper;

        public GetTablesQueryHandler(IRepository<Table> dbContext, IMapper mapper)
        {
            this.repository = dbContext;
            this.mapper = mapper;
        }

        public async Task<QueryResult<IEnumerable<TableDto>>> Handle(
            GetTablesQuery request,
            CancellationToken cancellationToken)
        {
            var entites = await this.repository.All(cancellationToken);
            var sortedEntities = entites
                .OrderBy(x => x.TableNumber)
                .ToArray();

            var dtos = this.mapper.Map<IEnumerable<TableDto>>(sortedEntities);

            return QueryResult<IEnumerable<TableDto>>.Success(dtos);
        }
    }
}
