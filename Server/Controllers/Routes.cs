using Server.Shared;

namespace Server.Controllers;

public static class Routes
{
    private const string CstStuff = "stuff";
    private const string CstUser = "user";

    public static IEndpointRouteBuilder MapRoutes(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("api").RequireAuthorization().AddEndpointFilter<TraceEndpointFilter>();

        api.MapGroup(CstStuff).WithTags(CstStuff).MapStuffEndpoints();
        api.MapGroup(CstUser).WithTags(CstUser).MapUserEndpoints();
        return builder;
    }
}
