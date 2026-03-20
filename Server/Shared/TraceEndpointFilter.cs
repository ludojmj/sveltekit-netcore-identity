namespace Server.Shared;

public class TraceEndpointFilter(ILogger<TraceEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var userInfo = context.HttpContext.GetCurrentUser();
        var req = context.Arguments.CleanTrace().IndentSerialize();

        var result = await next(context);
        var resp = result.IndentSerialize().Truncate(4096);
        logger.LogInformation("{UserInfo} Request: {Req} Response: {Resp}", userInfo, req, resp);
        return result;
    }
}
