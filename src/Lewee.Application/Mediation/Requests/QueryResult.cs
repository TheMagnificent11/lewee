namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Query Result
/// </summary>
/// <typeparam name="T">Query response type</typeparam>
public class QueryResult<T> : BaseResult
    where T : class
{
    private QueryResult(T? data, ResultStatus status, Dictionary<string, List<string>>? errors)
        : base(status, errors)
    {
        this.Data = data;
    }

    /// <summary>
    /// Gets the query result data
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Successful <see cref="QueryResult{T}"/>
    /// </summary>
    /// <param name="data">Result data</param>
    /// <returns>A query result contain the specified data</returns>
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

        var errors = new Dictionary<string, List<string>>()
        {
            { string.Empty, new List<string> { errorMessage } }
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
    public static QueryResult<T> Fail(ResultStatus status, Dictionary<string, List<string>> errors)
    {
        CheckIfFailure(status);

        return new QueryResult<T>(null, status, errors);
    }
}
