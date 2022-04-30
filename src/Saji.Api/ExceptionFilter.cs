using FluentValidation;

namespace Saji.Api;

/// <summary>
/// Exception Filter
/// </summary>
public class ExceptionFilter : IAsyncExceptionFilter
{
    /// <summary>
    /// Handles exceptions thrown in API
    /// </summary>
    /// <param name="context">
    /// Exception contect
    /// </param>
    /// <returns>
    /// Asynchronous task
    /// </returns>
    public virtual Task OnExceptionAsync(ExceptionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context?.Exception == null)
        {
            return Task.CompletedTask;
        }

        switch (context.Exception)
        {
            case ValidationException validationException:
                context.Result = HandleValidationException(validationException);
                break;

            case UnauthorizedAccessException _:
                context.Result = new ForbidResult();
                break;
        }

        return Task.CompletedTask;
    }

    private static IActionResult HandleValidationException(ValidationException exception)
    {
        var modelState = new ModelStateDictionary();

        exception.Errors
            .ToList()
            .ForEach(x => modelState.AddModelError(x.PropertyName, x.ErrorMessage));

        return new BadRequestObjectResult(modelState);
    }
}
