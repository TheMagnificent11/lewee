using Microsoft.AspNetCore.SignalR.Client;

namespace Lewee.Blazor.Events;
internal class MessageReceiver
{
    private readonly HubConnection hubConnection;

    public MessageReceiver(string hubUrl)
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();
    }

    public async Task StartAsync()
    {
        await this.hubConnection.StartAsync();
    }

    public async Task StopAsync()
    {
        await this.hubConnection.StopAsync();
    }

    public HubConnection GetHubConnection()
    {
        return this.hubConnection;
    }
}
