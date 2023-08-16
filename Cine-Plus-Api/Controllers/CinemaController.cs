using Microsoft.AspNetCore.Mvc;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : ControllerBase
{
    private readonly ICinemaCommandHandler _cinemaCommand;

    private readonly ICinemaQueryHandler _cinemaQuery;

    public CinemaController(ICinemaCommandHandler cinemaCommand, ICinemaQueryHandler cinemaQuery)
    {
        this._cinemaCommand = cinemaCommand;
        this._cinemaQuery = cinemaQuery;
    }

    [HttpGet]
    public async Task<IEnumerable<Cinema>> Get()
    {
        return await this._cinemaQuery.Handler();
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post(CreateCinema request)
    {
        var response = await this._cinemaCommand.Handler(request);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPut]
    public async Task<ActionResult> Put(UpdateCinema request)
    {
        var response = await this._cinemaCommand.Handler(request);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remove(int id)
    {
        var response = await this._cinemaCommand.Handler(id);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}