using System.Net.Http.Json;
using Pizzeria.Common;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.WebClient.Services;

public class PizzeriaApiClient : IPizzeriaApiClient
{
    private readonly HttpClient httpClient;

    public PizzeriaApiClient(string baseAddress, HttpClient? httpClient = null)
    {
        this.httpClient = httpClient ?? new HttpClient();
        this.httpClient.BaseAddress = new Uri(baseAddress);
    }

    public async Task<PizzaDto[]> GetPizzasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await this.httpClient.GetAsync(Endpoints.StoreApi.Pizzas, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var pizzas = await response.Content.ReadFromJsonAsync<PizzaDto[]>(cancellationToken);
            return pizzas ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<ApiResponse> StartOrderAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await this.httpClient.PostAsync(Endpoints.StoreApi.Orders, null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse(true);
            }
            
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return new ApiResponse(false, $"Failed to start order: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return new ApiResponse(false, $"Error starting order: {ex.Message}");
        }
    }

    public async Task<ApiResponse> AddPizzaToOrderAsync(Guid orderId, Guid pizzaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = Endpoints.StoreApi.GetAddPizzaToOrderEndpoint(orderId, pizzaId);
            var response = await this.httpClient.PutAsync(endpoint, null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse(true);
            }
            
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return new ApiResponse(false, $"Failed to add pizza to order: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return new ApiResponse(false, $"Error adding pizza to order: {ex.Message}");
        }
    }
}