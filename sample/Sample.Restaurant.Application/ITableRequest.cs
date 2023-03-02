using Lewee.Application.Mediation.Requests;

namespace Sample.Restaurant.Application;

public interface ITableRequest : IApplicationRequest
{
    int TableNumber { get; }
}
