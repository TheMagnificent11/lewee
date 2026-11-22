using Microsoft.AspNetCore.Components.Server.Circuits;
using Pizzeria.Ordering.StateManagement.Orders;

namespace Pizzeria.Ordering.Web.Infrastructure;

internal sealed class OrderReconciliationCircuitHandler :
    CircuitHandler,
    IOrderReconciliationCancellationTokenProvider,
    IDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public CancellationToken CancellationToken => this.cancellationTokenSource.Token;

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await this.cancellationTokenSource.CancelAsync();
    }

    public void Dispose()
    {
        this.cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}
