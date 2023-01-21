using Lewee.Application.Mediation;

namespace Sample.Restaurant.Application;

public interface ITableRequest : IApplicationRequest
{
    int TableNumber { get; }
}
