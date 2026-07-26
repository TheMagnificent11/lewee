using System.Diagnostics.CodeAnalysis;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Message Received Action Inteface
/// </summary>
[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1649:File name should match first type name",
    Justification = "False positive")]
public interface IMessageReceivedAction : IRequestAction;

/// <summary>
/// Message Received Action Interface
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public interface IMessageReceivedAction<T> : IMessageReceivedAction
    where T : class
{
    /// <summary>
    /// Gets the data
    /// </summary>
    T Data { get; init; }
}
