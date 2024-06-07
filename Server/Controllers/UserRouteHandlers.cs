using Server.Services.Interfaces;
using Server.Models;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

[assembly: InternalsVisibleTo("Server.UnitTest")]
namespace Server.Controllers;

public static class UserRouteHandlers
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", GetListAsync);
        builder.MapPost("/", CreateAsync);
        builder.MapGet("/{id}", ReadAsync);
        builder.MapPut("/{id}", UpdateAsync);
        builder.MapDelete("/{id}", DeleteAsync);
        return builder;
    }

    internal static async Task<IResult> GetListAsync(
        [FromQuery] int? page,
        [FromQuery] string search,
        [FromServices] IUserService userService)
    {
        DirectoryModel result = string.IsNullOrWhiteSpace(search)
            ? await userService.GetListAsync(page ?? 0)
            : await userService.SearchListAsync(search);
        return Results.Ok(result);
    }

    internal static async Task<IResult> CreateAsync(
        [FromBody] UserModel input,
        [FromServices] IUserService userService)
    {
        UserModel result = await userService.CreateAsync(input);
        return Results.Created($"{result.Id}", result);
    }

    internal static async Task<IResult> ReadAsync(
        [FromRoute] string id,
        [FromServices] IUserService userService)
    {
        UserModel result = await userService.ReadAsync(id);
        return Results.Ok(result);
    }

    internal static async Task<IResult> UpdateAsync(
        [FromRoute] string id,
        [FromBody] UserModel input,
        [FromServices] IUserService userService)
    {
        UserModel result = await userService.UpdateAsync(id, input);
        return Results.Ok(result);
    }

    internal static async Task<IResult> DeleteAsync(
        [FromRoute] string id,
        [FromServices] IUserService userService)
    {
        await userService.DeleteAsync(id);
        return Results.NoContent();
    }
}
