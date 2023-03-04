using MediatR;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Command Interface
/// </summary>
public interface ICommand : IApplicationRequest, IRequest<CommandResult>
{
    /// <summary>
    /// Gets the SignalR client ID
    /// </summary>
    string? ClientId { get; }
}
