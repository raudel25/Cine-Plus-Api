using Microsoft.AspNetCore.Mvc;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieCommandHandler _movieCommand;

    private readonly IMovieQueryHandler _movieQuery;

    public MovieController(IMovieCommandHandler movieCommand, IMovieQueryHandler movieQuery)
    {
        this._movieCommand = movieCommand;
        this._movieQuery = movieQuery;
    }

    [HttpGet]
    public async Task<IEnumerable<Movie>> Get()
    {
        return await this._movieQuery.Handler();
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post(CreateMovie request)
    {
        var response = await this._movieCommand.Handler(request);

        if (response.Ok) return response.Value;

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPut]
    public async Task<ActionResult> Put(UpdateMovie request)
    {
        var response = await this._movieCommand.Handler(request);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remove(int id)
    {
        var response = await this._movieCommand.Handler(id);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}