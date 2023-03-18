using MediatR;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Command Interface
/// </summary>
public interface ICommand : IApplicationRequest, IRequest<CommandResult>
{
}
