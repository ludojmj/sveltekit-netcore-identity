using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Server.Shared;

public class ErrorHandler(ILogger<ErrorHandler> logger, IHostEnvironment env, IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogCritical(exception, "ERROR");
        bool showRealError = true;
        switch (exception)
        {
            case KeyNotFoundException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case BusinessException:
            case DbUpdateException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                break;

            default:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                showRealError = env.IsDevelopment();
                break;
        }

        var msg = exception.InnerException == null
            ? exception.Message
            : exception.InnerException.Message;

        var result = new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails =
            {
                Detail = showRealError ? msg : "An error occured. Please try again later.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Title = "An error occurred.",
                Type = exception.GetType().Name,
            }
        };

        await problemDetailsService.WriteAsync(result);
        return true;
    }
}
