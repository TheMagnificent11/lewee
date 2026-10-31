using System.Reflection;

namespace Lewee.Auth.Api;

/// <summary>
/// Provides access to the assembly containing the auth API endpoints.
/// </summary>
public static class AuthApiAssemblyMarker
{
    /// <summary>
    /// Gets the assembly containing the auth API endpoints.
    /// </summary>
    public static Assembly Assembly => typeof(CreateUserEndpoint).Assembly;
}
