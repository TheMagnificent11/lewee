using Lewee.Application.Mediation;

namespace Sample.Restaurant.Application.Tables;

public interface ITableRequest : IApplicationRequest
{
    int TableNumber { get; }
}
