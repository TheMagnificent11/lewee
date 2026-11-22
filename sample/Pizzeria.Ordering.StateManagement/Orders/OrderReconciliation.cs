using System.Net;
using System.Net.Http;
using Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Refit;

namespace Pizzeria.Ordering.StateManagement.Orders;

internal static class OrderReconciliation
{
    public const int MaxAttempts = 5;
    public const int RetryDelayMilliseconds = 100;
    public const string ErrorMessage =
        "Your order was updated, but it could not be refreshed. Please try again.";

    public static async Task ReconcileOrderAsync(
        IBffApiClient bffApiClient,
        Guid orderId,
        Guid correlationId,
        IDispatcher dispatcher,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var order = await GetOrderAsync(bffApiClient, orderId, logger, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (order is null)
        {
            dispatcher.Dispatch(new OrderCommandReconciliationFailedAction
            {
                CorrelationId = correlationId,
                ErrorMessage = ErrorMessage,
            });

            return;
        }

        dispatcher.Dispatch(new OrderCommandReconciledAction
        {
            Data = order,
            CorrelationId = correlationId,
        });
    }

    private static async Task<OrderDto?> GetOrderAsync(
        IBffApiClient bffApiClient,
        Guid orderId,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            try
            {
                return await bffApiClient.GetOrderAsync(orderId, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return null;
            }
            catch (Exception exception)
            {
                if (!IsTransient(exception))
                {
                    logger.LogError(exception, "Failed to reconcile order {OrderId}", orderId);
                    return null;
                }

                if (attempt >= MaxAttempts)
                {
                    logger.LogError(
                        exception,
                        "Failed to reconcile order {OrderId} after {AttemptCount} attempts",
                        orderId,
                        MaxAttempts);
                    return null;
                }

                logger.LogWarning(
                    exception,
                    "Failed to reconcile order {OrderId} on attempt {Attempt}; retrying",
                    orderId,
                    attempt);

                try
                {
                    await Task.Delay(
                        TimeSpan.FromMilliseconds(RetryDelayMilliseconds * attempt),
                        cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
            }
        }

        return null;
    }

    private static bool IsTransient(Exception exception)
    {
        return exception is HttpRequestException or TimeoutException or TaskCanceledException
            || (exception is ApiException apiException
                && (apiException.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
                    || (int)apiException.StatusCode >= 500));
    }
}
