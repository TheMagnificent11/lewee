using Lewee.Common;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Client Message Event Args
/// </summary>
public sealed class ClientMessageEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientMessageEventArgs"/> class
    /// </summary>
    /// <param name="message">Client message</param>
    public ClientMessageEventArgs(ClientMessage message)
    {
        this.Message = message;
    }

    /// <summary>
    /// Gets the client message
    /// </summary>
    public ClientMessage Message { get; }
}
