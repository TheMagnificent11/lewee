using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application;

public sealed class GetTableDetailsQuery : IQuery<TableDetailsDto>
{
    public GetTableDetailsQuery(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }

    internal class GetTableDetailsQueryHandler : IRequestHandler<GetTableDetailsQuery, QueryResult<TableDetailsDto>>
    {
        private readonly IQueryProjectionService queryProjectionService;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public GetTableDetailsQueryHandler(
            IQueryProjectionService queryProjectionService,
            IMapper mapper,
            ILogger<GetTableDetailsQueryHandler> logger)
        {
            this.queryProjectionService = queryProjectionService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<QueryResult<TableDetailsDto>> Handle(GetTableDetailsQuery request, CancellationToken cancellationToken)
        {
            var projection = await this.queryProjectionService.RetrieveByKey<TableDetails>(
                request.TableNumber.ToString(),
                cancellationToken);
            if (projection == null)
            {
                this.logger.LogError("Table read model does not exist");
                return QueryResult<TableDetailsDto>.Fail(ResultStatus.NotFound, "Could not find details for the table");
            }

            var dto = this.mapper.Map<TableDetailsDto>(projection);

            return QueryResult<TableDetailsDto>.Success(dto);
        }
    }
}
