using Server.Services.Interfaces;
using Server.Models;

namespace Server.Controllers;

public static class StuffRouteHandlers
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

    private static async Task<IResult> GetListAsync(int? page, string search, IStuffService stuffService)
    {
        StuffModel result = string.IsNullOrWhiteSpace(search)
            ? await stuffService.GetListAsync(page ?? 0)
            : await stuffService.SearchListAsync(search);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateAsync(DatumModel input, IStuffService stuffService)
    {
        DatumModel result = await stuffService.CreateAsync(input);
        return Results.Created($"{result.Id}", result);
    }

    private static async Task<IResult> ReadAsync(Guid id, IStuffService stuffService)
    {
        DatumModel result = await stuffService.ReadAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateAsync(Guid id, DatumModel input, IStuffService stuffService)
    {
        DatumModel result = await stuffService.UpdateAsync(id, input);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteAsync(Guid id, IStuffService stuffService)
    {
        await stuffService.DeleteAsync(id);
        return Results.NoContent();
    }
}
