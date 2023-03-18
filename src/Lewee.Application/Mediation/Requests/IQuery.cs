using MediatR;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Query Interface
/// </summary>
/// <typeparam name="T">
/// Query response type
/// </typeparam>
public interface IQuery<T> : IApplicationRequest, IRequest<T>
    where T : class
{
}
