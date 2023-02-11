using Lewee.Application.Mediation;
using Lewee.Application.Mediation.Responses;
using Lewee.Domain;
using MapsterMapper;
using MediatR;
using Sample.Restaurant.Domain;
using Serilog;

namespace Sample.Restaurant.Application;

public sealed class GetTableDetailsQuery : IQuery<QueryResult<TableDetailsDto>>
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
        private readonly IReadModelService readModelService;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public GetTableDetailsQueryHandler(
            IReadModelService readModelService,
            IMapper mapper,
            ILogger logger)
        {
            this.readModelService = readModelService;
            this.mapper = mapper;
            this.logger = logger.ForContext<GetTableDetailsQueryHandler>();
        }

        public async Task<QueryResult<TableDetailsDto>> Handle(GetTableDetailsQuery request, CancellationToken cancellationToken)
        {
            var readModel = await this.readModelService.RetrieveByKey<TableDetailsReadModel>(
                request.TableNumber.ToString(),
                cancellationToken);
            if (readModel == null)
            {
                this.logger.Warning("Table read model does not exist");
                return QueryResult<TableDetailsDto>.Fail(ResultStatus.NotFound, "Could not find details for the table");
            }

            var dto = this.mapper.Map<TableDetailsDto>(readModel);

            return QueryResult<TableDetailsDto>.Success(dto);
        }
    }
}
