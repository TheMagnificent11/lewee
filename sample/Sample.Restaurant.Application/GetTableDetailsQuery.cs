using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MapsterMapper;
using MediatR;
using Sample.Restaurant.Domain;
using Serilog;

namespace Sample.Restaurant.Application;

public sealed class GetTableDetailsQuery : IQuery<TableDetailsDto>
{
    public GetTableDetailsQuery(Guid? correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid? CorrelationId { get; }
    public int TableNumber { get; }

    internal class GetTableDetailsQueryHandler : IRequestHandler<GetTableDetailsQuery, QueryResult<TableDetailsDto>>
    {
        private readonly IQueryProjectionService queryProjectionService;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public GetTableDetailsQueryHandler(
            IQueryProjectionService queryProjectionService,
            IMapper mapper,
            ILogger logger)
        {
            this.queryProjectionService = queryProjectionService;
            this.mapper = mapper;
            this.logger = logger.ForContext<GetTableDetailsQueryHandler>();
        }

        public async Task<QueryResult<TableDetailsDto>> Handle(GetTableDetailsQuery request, CancellationToken cancellationToken)
        {
            var projection = await this.queryProjectionService.RetrieveByKey<TableDetails>(
                request.TableNumber.ToString(),
                cancellationToken);
            if (projection == null)
            {
                this.logger.Error("Table read model does not exist");
                return QueryResult<TableDetailsDto>.Fail(ResultStatus.NotFound, "Could not find details for the table");
            }

            var dto = this.mapper.Map<TableDetailsDto>(projection);

            return QueryResult<TableDetailsDto>.Success(dto);
        }
    }
}
