using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SolicitorFinder.Attributes;

public sealed class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );

            context.Result = new BadRequestObjectResult(new
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            });
        }
    }
}
