using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services.Interfaces;
using Server.Shared;

namespace Server.Controllers;

[Route("api/[controller]")]
public class StuffController : ControllerBase
{
    // GET STUFF LIST api/stuff?page=2&search=xxx
    [HttpGet]
    [ProducesResponseType(typeof(StuffModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList(
        [FromQuery] int page,
        [FromQuery] string search,
        [FromServices] IStuffService stuffService)
    {
        StuffModel result = string.IsNullOrWhiteSpace(search)
            ? await stuffService.GetListAsync(page)
            : await stuffService.SearchListAsync(search);
        return Ok(result);
    }

    // CREATE STUFF api/stuff + {body}
    [HttpPost]
    [ProducesResponseType(typeof(DatumModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] DatumModel input,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.CreateAsync(input);
        return CreatedAtAction(nameof(Read), new { id = result.Id }, result);
    }

    // READ STUFF api/stuff/id
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DatumModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Read(
        string id,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.ReadAsync(id);
        return Ok(result);
    }

    // UPDATE STUFF api/stuff/id + {body}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DatumModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] DatumModel input,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.UpdateAsync(id, input);
        return Ok(result);
    }

    // DELETE STUFF api/stuff/id
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(StuffModel), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] IStuffService stuffService)
    {
        await stuffService.DeleteAsync(id);
        return NoContent();
    }
}
