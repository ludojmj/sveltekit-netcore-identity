using Server.Shared;

namespace Server.Controllers;

public static class Routes
{
    public static void MapStuff(this RouteGroupBuilder group) => StuffRouteHandlers
        .Map(group).WithTags("Stuff").RequireAuthorization().AddEndpointFilter<TraceEndpointFilter>();
    public static void MapUser(this RouteGroupBuilder group) => UserRouteHandlers
        .Map(group).WithTags("User").RequireAuthorization().AddEndpointFilter<TraceEndpointFilter>();
}
