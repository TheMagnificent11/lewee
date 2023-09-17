using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public class OrderItemErrorAction : IRequestErrorAction
{
    public OrderItemErrorAction(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }

    public string RequestType => "OrderItem";
}
