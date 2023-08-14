using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Services;
using Microsoft.AspNetCore.Mvc;

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
}