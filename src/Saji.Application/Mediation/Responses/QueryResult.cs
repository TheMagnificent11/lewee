namespace Saji.Application.Mediation.Responses;

/// <summary>
/// Query Result
/// </summary>
/// <typeparam name="T">
/// Query response type
/// </typeparam>
public class QueryResult<T> : BaseResult
    where T : class
{
    private QueryResult(
        T? data,
        ResultStatus status,
        Dictionary<string, string[]>? errors)
        : base(status, errors)
    {
        this.Data = data;
    }

    /// <summary>
    /// Gets the query response data
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    /// Successful <see cref="CommandResult"/>
    /// </summary>
    /// <param name="data">
    /// Query response data
    /// </param>
    /// <returns>
    /// <see cref="CommandResult"/>
    /// </returns>
    public static QueryResult<T> Success(T data)
    {
        return new QueryResult<T>(data, ResultStatus.Success, null);
    }

    /// <summary>
    /// Failure <see cref="QueryResult{T}"/>
    /// </summary>
    /// <param name="status">
    /// Result status
    /// </param>
    /// <param name="errorMessage">
    /// Error message
    /// </param>
    /// <returns>
    /// <see cref="QueryResult{T}"/>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="ResultStatus"/> is <see cref="ResultStatus.Success"/> or <see cref="ResultStatus.NotApplicable"/>
    /// </exception>
    public static QueryResult<T> Fail(ResultStatus status, string errorMessage)
    {
        CheckIfFailure(status);

        var errors = new Dictionary<string, string[]>()
        {
            { string.Empty, new string[] { errorMessage } }
        };

        return new QueryResult<T>(null, status, errors);
    }

    /// <summary>
    /// Failure <see cref="QueryResult{T}"/>
    /// </summary>
    /// <param name="status">
    /// Result status
    /// </param>
    /// <param name="errors">
    /// Errors keyed by request property name
    /// </param>
    /// <returns>
    /// <see cref="QueryResult{T}"/>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="ResultStatus"/> is <see cref="ResultStatus.Success"/> or <see cref="ResultStatus.NotApplicable"/>
    /// </exception>
    public static QueryResult<T> Fail(ResultStatus status, Dictionary<string, string[]> errors)
    {
        CheckIfFailure(status);

        return new QueryResult<T>(null, status, errors);
    }
}
