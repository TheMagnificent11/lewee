using Lewee.Auth.Application;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

/// <summary>
/// Pipeline behavior that verifies the authenticated caller owns the order targeted by the request,
/// so individual command/query handlers do not need to perform this check themselves.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
internal sealed class OrderOwnershipAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IOrderOwnerRequest
    where TResponse : Result
{
    private readonly IRepository<Order> orderRepository;
    private readonly IAuthenticatedUserService authenticatedUserService;

    public OrderOwnershipAuthorizationBehavior(
        IRepository<Order> orderRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        this.orderRepository = orderRepository;
        this.authenticatedUserService = authenticatedUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var order = await this.orderRepository.QueryOneAsync(
            new GetOrderQuerySpec(request.OrderId),
            cancellationToken);

        if (order is null)
        {
            return AuthorizationResultFactory.CreateFailure<TResponse>(
                ResultStatus.NotFound,
                $"Order {request.OrderId} not found");
        }

        if (!OrderOwnership.IsOwnedByCaller(order, this.authenticatedUserService))
        {
            return AuthorizationResultFactory.CreateFailure<TResponse>(
                ResultStatus.Unauthorized,
                "Order does not belong to the caller.");
        }

        return await next(cancellationToken);
    }
}
