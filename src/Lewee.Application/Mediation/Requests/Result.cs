﻿using System.Text;
using FluentValidation.Results;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Result
/// </summary>
public abstract class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class
    /// </summary>
    /// <param name="status">
    /// Result status
    /// </param>
    /// <param name="errors">
    /// Error messages (keyed by request property)
    /// </param>
    protected Result(ResultStatus status, List<ValidationFailure>? errors)
    {
        this.Status = status;
        this.Errors = errors ?? new List<ValidationFailure>();
        this.IsSuccess = status == ResultStatus.Success;
    }

    /// <summary>
    /// Gets a value indicating whether request was successfully processed
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the status for the result
    /// </summary>
    public ResultStatus Status { get; }

    /// <summary>
    /// Gets a dictionary of error messages keyed by request property
    /// </summary>
    public List<ValidationFailure> Errors { get; }

    /// <summary>
    /// Generates an error message from the <see cref="Errors"/> dictionary.
    /// </summary>
    /// <returns>Error message</returns>
    /// <remarks>Each string in the dictionary is separated by a new-line character</remarks>
    public string GenerateErrorMessage()
    {
        if (!this.Errors.Any())
        {
            return string.Empty;
        }

        var errorMessage = new StringBuilder();

        this.Errors
            .ForEach(x => errorMessage.AppendLine($"{x.PropertyName}: {x.ErrorMessage}"));

        return errorMessage.ToString();
    }

    /// <summary>
    /// Checks if the status provided is a failure status
    /// </summary>
    /// <param name="status">
    /// Status to check
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="ResultStatus"/> is <see cref="ResultStatus.Success"/> or <see cref="ResultStatus.NotApplicable"/>
    /// </exception>
    protected static void CheckIfFailure(ResultStatus status)
    {
        if (status == ResultStatus.Success || status == ResultStatus.NotApplicable)
        {
            throw new InvalidOperationException("Status cannot be 'Success' or 'Not Applicable' for a failure result");
        }
    }
}
