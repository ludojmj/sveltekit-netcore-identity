using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Shared;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ModelValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            base.OnActionExecuting(context);
            return;
        }

        var err = context.ModelState.Values.SelectMany(value => value.Errors).FirstOrDefault();
        throw new ArgumentException($"Model state is invalid: {err?.ErrorMessage}");
    }
}
