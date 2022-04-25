using MediatR;
using Saji.Application.Mediation.Responses;

namespace Saji.Application.Mediation;

/// <summary>
/// Command Interface
/// </summary>
public interface ICommand : IApplicationRequest, IRequest<CommandResult>
{
}
