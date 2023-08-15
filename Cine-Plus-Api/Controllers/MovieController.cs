using Cine_Plus_Api.Requests;
using Microsoft.AspNetCore.Mvc;
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
        return await this._movieCommand.Handler(request);
    }
    
    [HttpPut]
    public async Task<ActionResult> Put(UpdateMovie request)
    {
        await this._movieCommand.Handler(request);
        
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remove(int id)
    {
        await this._movieCommand.Handler(id);

        return Ok();
    }
}