using System.Threading.Channels;

namespace Lewee.Contracts;

/// <summary>
/// Channel for passing client events to Blazor circuits
/// </summary>
public sealed class ClientEventChannel
{
    private readonly Channel<ClientMessage> channel = Channel.CreateUnbounded<ClientMessage>(
        new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
        });

    /// <summary>
    /// Gets the channel writer
    /// </summary>
    public ChannelWriter<ClientMessage> Writer => this.channel.Writer;

    /// <summary>
    /// Gets the channel reader
    /// </summary>
    public ChannelReader<ClientMessage> Reader => this.channel.Reader;
}
