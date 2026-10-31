using Pizzeria.Ordering.StateManagement.Orders;

namespace Pizzeria.Ordering.Web.Infrastructure;

internal sealed class ClientLifetimeCancellationTokenProvider :
    IOrderReconciliationCancellationTokenProvider,
    IDisposable
{
    private CancellationTokenSource cancellationTokenSource = new();

    public CancellationToken CancellationToken => this.cancellationTokenSource.Token;

    public void UpdateAuthenticationState(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                this.cancellationTokenSource.Dispose();
                this.cancellationTokenSource = new CancellationTokenSource();
            }

            return;
        }

        if (!this.cancellationTokenSource.IsCancellationRequested)
        {
            this.cancellationTokenSource.Cancel();
        }
    }

    public void Dispose()
    {
        this.UpdateAuthenticationState(isAuthenticated: false);
        this.cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}
