using Server.Services.Interfaces;
using Server.Models;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

[assembly: InternalsVisibleTo("Server.UnitTest")]
namespace Server.Controllers;

public static class StuffRouteHandlers
{
    public static IEndpointRouteBuilder MapStuffEndpoints(this IEndpointRouteBuilder builder)
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
        [FromServices] IStuffService stuffService)
    {
        StuffModel result = string.IsNullOrWhiteSpace(search)
            ? await stuffService.GetListAsync(page ?? 0)
            : await stuffService.SearchListAsync(search);
        return Results.Ok(result);
    }

    internal static async Task<IResult> CreateAsync(
        [FromBody] DatumModel input,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.CreateAsync(input);
        return Results.Created($"{result.Id}", result);
    }

    internal static async Task<IResult> ReadAsync(
        [FromRoute] Guid id,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.ReadAsync(id);
        return Results.Ok(result);
    }

    internal static async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] DatumModel input,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.UpdateAsync(id, input);
        return Results.Ok(result);
    }

    internal static async Task<IResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] IStuffService stuffService)
    {
        await stuffService.DeleteAsync(id);
        return Results.NoContent();
    }
}
