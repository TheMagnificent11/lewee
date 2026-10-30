using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts.Orders;
using Refit;

namespace Pizzeria.Ordering.StateManagement.Orders;

internal static class OrderReconciliation
{
    public static async Task<OrderDto> GetOrderAsync(
        IBffApiClient bffApiClient,
        Guid orderId,
        ILogger logger)
    {
        var retryCount = 0;

        while (true)
        {
            try
            {
                return await bffApiClient.GetOrderAsync(orderId);
            }
            catch (Exception exception) when (IsTransient(exception))
            {
                logger.LogWarning(
                    exception,
                    "Failed to reconcile order {OrderId}; retrying",
                    orderId);

                retryCount = Math.Min(retryCount + 1, 50);
                await Task.Delay(TimeSpan.FromMilliseconds(100 * retryCount));
            }
        }
    }

    private static bool IsTransient(Exception exception)
    {
        return exception is HttpRequestException or TimeoutException or TaskCanceledException
            || (exception is ApiException apiException
                && (apiException.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
                    || (int)apiException.StatusCode >= 500));
    }
}
