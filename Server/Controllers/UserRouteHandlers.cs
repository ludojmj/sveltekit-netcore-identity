using Server.Services.Interfaces;
using Server.Models;

namespace Server.Controllers;

public static class UserRouteHandlers
{
    public static RouteGroupBuilder Map(RouteGroupBuilder group)
    {
        group.MapGet("", GetListAsync);
        group.MapPost("", CreateAsync);
        group.MapGet("{id}", ReadAsync);
        group.MapPut("{id}", UpdateAsync);
        group.MapDelete("{id}", DeleteAsync);
        return group;
    }

    private static async Task<IResult> GetListAsync(int? page, string search, IUserService userService)
    {
        DirectoryModel result = string.IsNullOrWhiteSpace(search)
            ? await userService.GetListAsync(page ?? 0)
            : await userService.SearchListAsync(search);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateAsync(UserModel input, IUserService userService)
    {
        UserModel result = await userService.CreateAsync(input);
        return Results.Created($"{result.Id}", result);
    }

    private static async Task<IResult> ReadAsync(string id, IUserService userService)
    {
        UserModel result = await userService.ReadAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateAsync(string id, UserModel input, IUserService userService)
    {
        UserModel result = await userService.UpdateAsync(id, input);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteAsync(string id, IUserService userService)
    {
        await userService.DeleteAsync(id);
        return Results.NoContent();
    }
}
