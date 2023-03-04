using Microsoft.AspNetCore.Components;

namespace Lewee.Blazor.Messaging;

/// <summary>
/// Message Receiver Service Initializer
/// </summary>
public class MessageReceiverServiceInitializer : ComponentBase
{
    [Inject]
    private MessageReceiverService MessageReceiverService { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await this.MessageReceiverService.StartAsync();
    }
}
