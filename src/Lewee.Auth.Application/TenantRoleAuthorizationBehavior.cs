using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

internal class TenantRoleAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITenantRoleRequest
    where TResponse : Result
{
    private readonly IAuthenticatedUserService authenticatedUserService;
    private readonly IQueryProjectionService queryProjectionService;
    private readonly ILogger<TenantRoleAuthorizationBehavior<TRequest, TResponse>> logger;

    public TenantRoleAuthorizationBehavior(
        IAuthenticatedUserService authenticatedUserService,
        IQueryProjectionService queryProjectionService,
        ILogger<TenantRoleAuthorizationBehavior<TRequest, TResponse>> logger)
    {
        this.authenticatedUserService = authenticatedUserService;
        this.queryProjectionService = queryProjectionService;
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
            this.logger.LogTenantRoleUnauthenticated(typeof(TRequest).Name);

            return AuthorizationResultFactory.CreateFailure<TResponse>(
                ResultStatus.Unauthenticated,
                "No authenticated caller.");
        }

        var key = TenantMembershipRolesQueryProjection.BuildKey(request.TenantId, externalId);

        var projection = await this.queryProjectionService.RetrieveByKeyAsync<TenantMembershipRolesQueryProjection>(
            key,
            cancellationToken);

        var isAuthorized = projection is { IsMember: true } &&
            projection.RoleCodes.Intersect(request.SatisfyingRoles, StringComparer.Ordinal).Any();

        if (!isAuthorized)
        {
            this.logger.LogTenantRoleUnauthorized(externalId, request.TenantId, typeof(TRequest).Name);

            return AuthorizationResultFactory.CreateFailure<TResponse>(
                ResultStatus.Unauthorized,
                "Caller does not satisfy the required tenant roles.");
        }

        return await next(cancellationToken);
    }
}
