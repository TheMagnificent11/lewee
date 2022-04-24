﻿namespace Saji.Application.Errors;

/// <summary>
/// Authentication Error
/// </summary>
public class AuthenticationError : BaseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationError"/> class
    /// </summary>
    public AuthenticationError()
        : base("Not authenitcated")
    {
    }
}