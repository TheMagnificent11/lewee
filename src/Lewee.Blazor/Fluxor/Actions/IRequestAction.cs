﻿namespace Lewee.Blazor.Fluxor.Actions;

/// <summary>
/// Query Action
/// </summary>
public interface IRequestAction
{
    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    Guid CorrelationId { get; }
}
