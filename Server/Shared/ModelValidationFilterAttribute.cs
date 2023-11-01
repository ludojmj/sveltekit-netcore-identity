using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Shared;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ModelValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null)
        {
            base.OnActionExecuting(null!);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            var err = context.ModelState.Values.SelectMany(value => value.Errors).FirstOrDefault();
            if (err == null)
            {
                throw new ArgumentException("Model state is invalid.");
            }

            throw new ArgumentException(err.ErrorMessage);
        }

        base.OnActionExecuting(context);
    }
}
