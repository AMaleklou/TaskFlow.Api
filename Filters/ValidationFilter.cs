using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskFlow.Api.Common.Exceptions;

namespace TaskFlow.Api.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                                .Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();

            throw new ValidationException(errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}