using FluentValidation;
using FluentValidation.Results;
using Lewee.Application.Mediation.Requests;
using MediatR;

namespace Lewee.Application.Mediation.Behaviors;

internal class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICommand
    where TResponse : CommandResult
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var tasks = this.validators.Select(v => v.ValidateAsync(request, cancellationToken));
        var results = await Task.WhenAll(tasks);
        var failures = results
            .SelectMany(result => result.Errors)
            .Where(x => x != null)
            .ToList();

        if (failures.Any())
        {
            return (TResponse)GetBadRequestCommandResult(failures);
        }

        return await next();
    }

    private static CommandResult GetBadRequestCommandResult(List<ValidationFailure> failures)
    {
        return CommandResult.Fail(ResultStatus.BadRequest, failures);
    }
}
