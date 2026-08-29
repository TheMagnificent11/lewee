using Lewee.Auth.Application;
using Lewee.Infrastructure.FastEndpoints;

namespace Lewee.Auth.Api;

internal sealed class CreateUserEndpoint : CommandEndpoint<CreateUserRequest>
{
    protected override string Route => "/users";

    protected override string Name => "CreateUser";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(CreateUserRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(new CreateUserCommand(request.ExternalUserId), ct);
        await this.ToResponseAsync(result, ct);
    }
}
