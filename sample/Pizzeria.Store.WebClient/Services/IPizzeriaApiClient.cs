using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.WebClient.Services;

public interface IPizzeriaApiClient
{
    Task<PizzaDto[]> GetPizzasAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse> StartOrderAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse> AddPizzaToOrderAsync(Guid orderId, Guid pizzaId, CancellationToken cancellationToken = default);
}

public record ApiResponse(bool IsSuccess, string? ErrorMessage = null);