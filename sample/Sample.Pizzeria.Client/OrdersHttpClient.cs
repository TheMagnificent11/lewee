using System.Net.Http.Json;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client;

public sealed class OrdersHttpClient : IOrdersApi, IDisposable
{
    private readonly HttpClient httpClient;
    private bool disposedValue;

    public OrdersHttpClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task<OrderDto[]> GetUserOrders(CancellationToken cancellationToken)
    {
        using (var request = new HttpRequestMessage(HttpMethod.Get, OrderRoutes.GetUserOrders))
        using (var response = await this.httpClient.SendAsync(request, cancellationToken))
        {
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OrderDto[]>(cancellationToken);
            if (result == null)
            {
                return [];
            }

            return result;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.httpClient?.Dispose();
            }

            this.disposedValue = true;
        }
    }
}
