using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Shared;

public class ErrorHandler(IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is KeyNotFoundException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await httpContext.Response.WriteAsJsonAsync(
                GetProblemDetails(true, httpContext, exception, httpContext.Response.StatusCode),
                cancellationToken);
            return true;
        }

        if (exception is BusinessException
         || exception is DbUpdateException
        )
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await httpContext.Response.WriteAsJsonAsync(
                GetProblemDetails(true, httpContext, exception, httpContext.Response.StatusCode),
                cancellationToken);
            return true;
        }

        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await httpContext.Response.WriteAsJsonAsync(
            GetProblemDetails(env.IsDevelopment(), httpContext, exception, httpContext.Response.StatusCode),
            cancellationToken);
        return true;
    }

    private static ProblemDetails GetProblemDetails(bool isDevelopment, HttpContext httpContext, Exception exception, int httpStatusCode)
    {
        var msg = exception.InnerException == null
            ? exception.Message
            : exception.InnerException.Message;

        var result = new ProblemDetails
        {
            Status = httpStatusCode,
            Type = exception.GetType().Name,
            Title = "An unexpected error occurred",
            Detail = isDevelopment ? msg : "An error occured. Please try again later.",
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        return result;
    }
}
