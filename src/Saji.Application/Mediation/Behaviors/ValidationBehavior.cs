using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Saji.Application.Mediation.Responses;

namespace Saji.Application.Mediation.Behaviors;

/// <summary>
/// Validation Behaviour
/// </summary>
/// <typeparam name="TRequest">
/// Request type
/// </typeparam>
public class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest, CommandResult>
    where TRequest : ICommand
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest}"/> class
    /// </summary>
    /// <param name="validators">
    /// Validators
    /// </param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    /// <summary>
    /// Handles request and runs FluentValidation and returns early if validation fails
    /// </summary>
    /// <param name="request">
    /// Request to validate
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <param name="next">
    /// Next behaviour
    /// </param>
    /// <returns>
    /// Response
    /// </returns>
    public async Task<CommandResult> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<CommandResult> next)
    {
        var tasks = this.validators.Select(v => v.ValidateAsync(request, cancellationToken));
        var results = await Task.WhenAll(tasks);
        var failures = results
            .SelectMany(result => result.Errors)
            .Where(x => x != null)
            .ToList();

        if (failures.Any())
        {
            return GetBadRequestCommandResult(failures);
        }

        return await next();
    }

    private static CommandResult GetBadRequestCommandResult(List<ValidationFailure> failures)
    {
        var errors = new Dictionary<string, List<string>>();

        foreach (var item in failures)
        {
            if (!errors.ContainsKey(item.PropertyName))
            {
                errors[item.PropertyName] = new List<string>();
            }

            errors[item.PropertyName].Add(item.ErrorMessage);
        }

        return CommandResult.Fail(ResultStatus.BadRequest, errors);
    }
}
