using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowMovieController : ControllerBase
{
    private readonly IShowMovieCommandHandler _showMovieCommand;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    private readonly SecurityService _securityService;

    public ShowMovieController(IShowMovieCommandHandler showMovieCommand, IShowMovieQueryHandler showMovieQuery,
        SecurityService securityService)
    {
        this._showMovieCommand = showMovieCommand;
        this._showMovieQuery = showMovieQuery;
        this._securityService = securityService;
    }

    [HttpGet]
    public async Task<IEnumerable<ShowMovie>> Get()
    {
        return await this._showMovieQuery.Handler();
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<int>> Post(CreateShowMovie request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, AccountType.Manager);
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._showMovieCommand.Handler(request);

        if (response.Ok) return response.Value;

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}