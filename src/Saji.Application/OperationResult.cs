using System.Net;
using FluentValidation.Results;

namespace Saji.Application;

/// <summary>
/// Command Result
/// </summary>
public class OperationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; protected set; }

    /// <summary>
    /// Gets or sets the HTTP status
    /// </summary>
    public HttpStatusCode Status { get; protected set; }

    /// <summary>
    /// Gets or sets a the operation errors
    /// </summary>
    public IDictionary<string, IEnumerable<string>>? Errors { get; protected set; }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.OK"/><see cref="OperationResult"/>
    /// </summary>
    /// <returns>
    /// A <see cref="HttpStatusCode.OK"/><see cref="OperationResult"/>
    /// </returns>
    public static OperationResult Success()
    {
        return new OperationResult()
        {
            IsSuccess = true,
            Status = HttpStatusCode.OK
        };
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.OK"/><see cref="OperationResult{T}"/> with data
    /// </summary>
    /// <typeparam name="T">
    /// Data type
    /// </typeparam>
    /// <param name="data">
    /// Data
    /// </param>
    /// <returns>
    /// A <see cref="HttpStatusCode.OK"/><see cref="OperationResult{T}"/> with data
    /// </returns>
    public static OperationResult<T> Success<T>(T data)
    {
        return new OperationResult<T>(data)
        {
            IsSuccess = true,
            Status = HttpStatusCode.OK
        };
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult"/> with a set of errors
    /// </summary>
    /// <param name="errors">
    /// Errors
    /// </param>
    /// <returns>
    /// A <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult"/>
    /// </returns>
    public static OperationResult Fail(IDictionary<string, IEnumerable<string>> errors)
    {
        if (errors == null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        return new OperationResult()
        {
            IsSuccess = false,
            Status = HttpStatusCode.BadRequest,
            Errors = errors
        };
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult{T}"/> with a set
    /// of errors
    /// </summary>
    /// <typeparam name="T">
    /// Data type
    /// </typeparam>
    /// <param name="errors">
    /// Errors
    /// </param>
    /// <returns>
    /// A <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult{T}"/> containing no data
    /// </returns>
    public static OperationResult<T> Fail<T>(IDictionary<string, IEnumerable<string>> errors)
    {
        if (errors == null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        return new OperationResult<T>(null)
        {
            IsSuccess = false,
            Status = HttpStatusCode.BadRequest,
            Errors = errors
        };
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult"/> with a set of errors
    /// </summary>
    /// <param name="validationErrors">
    /// Validation errors
    /// </param>
    /// <returns>
    /// A <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult"/>
    /// </returns>
    public static OperationResult Fail(IEnumerable<ValidationFailure> validationErrors)
    {
        if (validationErrors == null)
        {
            throw new ArgumentNullException(nameof(validationErrors));
        }

        return Fail(GetErrors(validationErrors));
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult{T}"/> with a set
    /// of errors
    /// </summary>
    /// <typeparam name="T">
    /// Data type
    /// </typeparam>
    /// <param name="validationErrors">
    /// Validation errors
    /// </param>
    /// <returns>
    /// A <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult{T}"/> containing no data
    /// </returns>
    public static OperationResult<T> Fail<T>(IEnumerable<ValidationFailure> validationErrors)
    {
        if (validationErrors == null)
        {
            throw new ArgumentNullException(nameof(validationErrors));
        }

        return Fail<T>(GetErrors(validationErrors));
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult"/> with a single
    /// non-field-specific error
    /// </summary>
    /// <param name="errorMessage">
    /// Error message
    /// </param>
    /// <returns>
    /// A <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult"/>
    /// </returns>
    public static OperationResult Fail(string errorMessage)
    {
        if (errorMessage == null)
        {
            throw new ArgumentNullException(nameof(errorMessage));
        }

        var errors = new Dictionary<string, IEnumerable<string>>()
            {
                { string.Empty, new string[] { errorMessage } }
            };

        return Fail(errors);
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult{T}"/> with a
    /// single non-field-specific error
    /// </summary>
    /// <typeparam name="T">
    /// Data type
    /// </typeparam>
    /// <param name="errorMessage">
    /// Error message
    /// </param>
    /// <returns>
    /// A <see cref="HttpStatusCode.BadRequest"/><see cref="OperationResult{T}"/> containing no data
    /// </returns>
    public static OperationResult<T> Fail<T>(string errorMessage)
    {
        if (errorMessage == null)
        {
            throw new ArgumentNullException(nameof(errorMessage));
        }

        var errors = new Dictionary<string, IEnumerable<string>>()
            {
                { string.Empty, new string[] { errorMessage } }
            };

        return Fail<T>(errors);
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.NotFound"/> failure <see cref="OperationResult"/>
    /// </summary>
    /// <returns>
    /// A <see cref="HttpStatusCode.NotFound"/><see cref="OperationResult"/>
    /// </returns>
    public static OperationResult NotFound()
    {
        return new OperationResult
        {
            IsSuccess = false,
            Status = HttpStatusCode.NotFound
        };
    }

    /// <summary>
    /// Creates a <see cref="HttpStatusCode.NotFound"/> failure <see cref="OperationResult{T}"/>
    /// </summary>
    /// <typeparam name="T">
    /// Data type
    /// </typeparam>
    /// <returns>
    /// A <see cref="HttpStatusCode.NotFound"/><see cref="OperationResult{T}"/>
    /// </returns>
    public static OperationResult<T> NotFound<T>()
    {
        return new OperationResult<T>(null)
        {
            IsSuccess = false,
            Status = HttpStatusCode.NotFound
        };
    }

    private static IDictionary<string, IEnumerable<string>> GetErrors(IEnumerable<ValidationFailure> failures)
    {
        var errors = new Dictionary<string, IEnumerable<string>>();

        foreach (var item in failures)
        {
            var propertyErrors = errors.ContainsKey(item.PropertyName) ? errors[item.PropertyName].ToList() : new List<string>();

            propertyErrors.Add(item.ErrorMessage);

            if (errors.ContainsKey(item.PropertyName))
            {
                errors[item.PropertyName] = propertyErrors;
            }
            else
            {
                errors.Add(item.PropertyName, propertyErrors);
            }
        }

        return errors;
    }
}

/// <summary>
/// Operation Result
/// </summary>
/// <typeparam name="T">
/// Data type
/// </typeparam>
public class OperationResult<T> : OperationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult{T}"/> class
    /// </summary>
    /// <param name="data">
    /// Data
    /// </param>
    internal OperationResult(T data)
    {
        this.Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult{T}"/> class
    /// </summary>
    /// <param name="errors">
    /// Errors
    /// </param>
    internal OperationResult(IDictionary<string, IEnumerable<string>>? errors)
    {
        this.Errors = errors;
        this.IsSuccess = false;
        this.Status = HttpStatusCode.BadRequest;
    }

    /// <summary>
    /// Gets or sets the operation data
    /// </summary>
    public T? Data { get; protected set; }
}