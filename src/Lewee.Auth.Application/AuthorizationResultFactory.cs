using System.Collections.Concurrent;
using System.Reflection;
using Lewee.Common;

namespace Lewee.Auth.Application;

/// <summary>
/// Builds a failure <see cref="Result"/> for a pipeline response type, regardless of whether it is a
/// <see cref="CommandResult"/> or a <c>QueryResult&lt;T&gt;</c>, since both expose a compatible
/// <c>static Fail(ResultStatus, string)</c> factory method but share no common interface exposing it.
/// </summary>
internal static class AuthorizationResultFactory
{
    private static readonly ConcurrentDictionary<Type, Func<ResultStatus, string, object>> FailFactories = new();

    /// <summary>
    /// Creates a failure <typeparamref name="TResponse"/> with the given status and message.
    /// </summary>
    /// <typeparam name="TResponse">Response type.</typeparam>
    /// <param name="status">Result status.</param>
    /// <param name="message">Error message.</param>
    /// <returns>The failure response.</returns>
    public static TResponse CreateFailure<TResponse>(ResultStatus status, string message)
        where TResponse : Result
    {
        var factory = FailFactories.GetOrAdd(typeof(TResponse), BuildFailFactory);

        return (TResponse)factory(status, message);
    }

    private static Func<ResultStatus, string, object> BuildFailFactory(Type responseType)
    {
        var method = responseType.GetMethod(
            "Fail",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [typeof(ResultStatus), typeof(string)],
            modifiers: null);

        if (method == null)
        {
            throw new InvalidOperationException(
                $"Type '{responseType.FullName}' does not expose a static Fail(ResultStatus, string) method.");
        }

        return (status, message) => method.Invoke(null, [status, message])!;
    }
}
