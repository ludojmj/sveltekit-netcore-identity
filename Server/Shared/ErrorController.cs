using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Server.Shared;

[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    public IActionResult Error([FromServices] IHostEnvironment env, [FromServices] ILogger<ErrorController> logger)
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (context == null)
        {
            return Ok();
        }

        var exception = context.Error;
        logger.LogCritical(exception, "API_ERROR");
        var msg = exception.InnerException == null
            ? exception.Message
            : exception.InnerException.Message;

        if (exception is KeyNotFoundException)
        {
            return NotFound(new ErrorModel { Error = msg });
        }

        var error = new ErrorModel { Error = env.IsDevelopment() ? msg : "An error occured. Please try again later." };
        return BadRequest(error);
    }
}
