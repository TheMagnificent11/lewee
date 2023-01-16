using Lewee.Application.Data;
using Lewee.Application.Mediation.Responses;
using Lewee.Domain;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Base Aggregate Root Query Handler
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TDto">DTO result type</typeparam>
public abstract class BaseAggregateRootQueryHandler<TAggregate, TDto> : IRequestHandler<AggregateRootQuery<TAggregate, TDto>, QueryResult<IEnumerable<TDto>>>
    where TAggregate : class, IAggregateRoot
    where TDto : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAggregateRootQueryHandler{TAggregate, TDto}"/> class
    /// </summary>
    /// <param name="dbContext">Database context</param>
    /// <param name="mapper">Mapper</param>
    /// <param name="logger">Logger</param>
    protected BaseAggregateRootQueryHandler(IDbContext dbContext, IMapper mapper, ILogger logger)
    {
        this.DbContext = dbContext;
        this.Mapper = mapper;
        this.Logger = logger;
    }

    /// <summary>
    /// Gets the database context
    /// </summary>
    protected IDbContext DbContext { get; }

    /// <summary>
    /// Gets the mapper
    /// </summary>
    protected IMapper Mapper { get; }

    /// <summary>
    /// Gets the logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <inheritdoc />
    public async Task<QueryResult<IEnumerable<TDto>>> Handle(AggregateRootQuery<TAggregate, TDto> request, CancellationToken cancellationToken)
    {
        var entities = await this.ApplyFilter(this.DbContext.AggregateRoot<TAggregate>())
            .ToArrayAsync(cancellationToken);

        var dtos = this.MapToDataTransferObjects(entities);

        this.Logger.Debug("{Count} {EntityType} read from database", entities.Length, nameof(TAggregate));

        return QueryResult<IEnumerable<TDto>>.Success(dtos);
    }

    /// <summary>
    /// Applies a filter to aggregate root DB set
    /// </summary>
    /// <param name="source">Query source</param>
    /// <returns>Filters queryable collection</returns>
    protected abstract IQueryable<TAggregate> ApplyFilter(IQueryable<TAggregate> source);

    /// <summary>
    /// Maps the entites to DTOs
    /// </summary>
    /// <param name="source">Source entities</param>
    /// <returns>Enumerable collection of DTos</returns>
    protected virtual IEnumerable<TDto> MapToDataTransferObjects(IEnumerable<TAggregate> source)
    {
        return this.Mapper.Map<IEnumerable<TDto>>(source);
    }
}
