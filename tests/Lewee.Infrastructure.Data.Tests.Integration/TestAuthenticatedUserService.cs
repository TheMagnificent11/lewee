using Lewee.Domain;

namespace Lewee.Infrastructure.Data.Tests.Integration;

/// <summary>
/// Test implementation of IAuthenticatedUserService
/// </summary>
internal sealed class TestAuthenticatedUserService : IAuthenticatedUserService
{
    public string UserId => "test-user";
}
