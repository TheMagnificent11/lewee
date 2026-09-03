using Lewee.Auth.Domain;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

internal class AdministratorAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IAdministratorRequest
    where TResponse : Result
{
    private readonly IAuthenticatedUserService authenticatedUserService;
    private readonly IRepository<User> userRepository;
    private readonly ILogger<AdministratorAuthorizationBehavior<TRequest, TResponse>> logger;

    public AdministratorAuthorizationBehavior(
        IAuthenticatedUserService authenticatedUserService,
        IRepository<User> userRepository,
        ILogger<AdministratorAuthorizationBehavior<TRequest, TResponse>> logger)
    {
        this.authenticatedUserService = authenticatedUserService;
        this.userRepository = userRepository;
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var externalId = this.authenticatedUserService.UserId;

        if (string.IsNullOrEmpty(externalId))
        {
            this.logger.LogAdministratorUnauthenticated(typeof(TRequest).Name);

            return AuthorizationResultFactory.CreateFailure<TResponse>(
                ResultStatus.Unauthenticated,
                "No authenticated caller.");
        }

        var user = await this.userRepository.QueryOneAsync(
            new UserByExternalIdSpecification(externalId),
            cancellationToken);

        if (user == null || !user.IsSiteAdministrator)
        {
            this.logger.LogAdministratorUnauthorized(externalId, typeof(TRequest).Name);

            return AuthorizationResultFactory.CreateFailure<TResponse>(
                ResultStatus.Unauthorized,
                "Caller is not a site administrator.");
        }

        return await next(cancellationToken);
    }
}
