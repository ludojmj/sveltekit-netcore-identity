using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Shared;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TraceHandlerFilterAttribute(ILogger<TraceHandlerFilterAttribute> logger) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userInfo = context.HttpContext.GetCurrentUser();
        context.RequestLogger(logger, userInfo.ToString());
        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var userInfo = context.HttpContext.GetCurrentUser();
        context.ResponseLogger(logger, userInfo.ToString());
        base.OnActionExecuted(context);
    }
}
