using Lewee.Shared;
using MediatR;

namespace Lewee.Application.Mediation;

/// <summary>
/// Command Interface
/// </summary>
public interface ICommand : IApplicationRequest, IRequest<CommandResult>
{
}
