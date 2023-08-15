using Cine_Plus_Api.CommandsRequest;
using Microsoft.AspNetCore.Mvc;
using Cine_Plus_Api.Commands;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieCommandHandler _movieCommand;

    public MovieController(IMovieCommandHandler movieCommand)
    {
        this._movieCommand = movieCommand;
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post(CreateMovie request)
    {
        return await this._movieCommand.Handler(request);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remove(int id)
    {
        await this._movieCommand.Handler(id);

        return Ok();
    }
}