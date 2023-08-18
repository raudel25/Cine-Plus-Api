using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieCommandHandler _movieCommand;

    private readonly IMovieQueryHandler _movieQuery;

    private readonly SecurityService _securityService;

    public MovieController(IMovieCommandHandler movieCommand, IMovieQueryHandler movieQuery,
        SecurityService securityService)
    {
        this._movieCommand = movieCommand;
        this._movieQuery = movieQuery;
        this._securityService = securityService;
    }

    [HttpGet]
    public async Task<IEnumerable<Movie>> Get()
    {
        return await this._movieQuery.Handler();
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<int>> Post(CreateMovie request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._movieCommand.Handler(request);

        if (response.Ok) return response.Value;

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPut, Authorize]
    public async Task<ActionResult> Put(UpdateMovie request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._movieCommand.Handler(request);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("{id:int}"), Authorize]
    public async Task<ActionResult> Delete(int id, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._movieCommand.Handler(id);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}