using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Shared;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ModelValidationFilterAttribute(ILogger<ModelValidationFilterAttribute> logger) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var err = context.ModelState.Values.SelectMany(value => value.Errors).FirstOrDefault();
            logger.LogError("Model state is invalid: {err?.ErrorMessage}", err?.ErrorMessage);
            context.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
            return;
        }

        base.OnActionExecuting(context);
    }
}
