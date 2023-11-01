using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Server.Shared;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TraceHandlerFilterAttribute : ActionFilterAttribute
{
    private readonly ILogger _logger;

    public TraceHandlerFilterAttribute(ILogger<TraceHandlerFilterAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null)
        {
            base.OnActionExecuting(null!);
            return;
        }

        var userInfo = context.HttpContext.GetCurrentUser();
        foreach (var elt in context.ActionArguments)
        {
            if (elt.Key == "service")
            {
                return;
            }

            var flux = JsonSerializer.Serialize(elt.Value, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation($"{userInfo} Request: {elt.Key}={flux}");
        }

        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context == null)
        {
            base.OnActionExecuted(null!);
            return;
        }

        var userInfo = context.HttpContext.GetCurrentUser();
        if (context.Result is ObjectResult elt)
        {
            string flux = JsonSerializer.Serialize(elt.Value, new JsonSerializerOptions { WriteIndented = true }).Truncate();
            _logger.LogInformation($"{userInfo} Response: {flux}");
        }

        base.OnActionExecuted(context);
    }
}
