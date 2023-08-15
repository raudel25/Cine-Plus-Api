using Cine_Plus_Api.CommandsRequest;
using Microsoft.AspNetCore.Mvc;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowMovieController : ControllerBase
{
    private readonly IShowMovieCommandHandler _showMovieCommand;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    public ShowMovieController(IShowMovieCommandHandler showMovieCommand, IShowMovieQueryHandler showMovieQuery)
    {
        this._showMovieCommand = showMovieCommand;
        this._showMovieQuery = showMovieQuery;
    }

    [HttpGet]
    public async Task<IEnumerable<ShowMovie>> Get()
    {
        return await this._showMovieQuery.Handler();
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post(CreateShowMovie request)
    {
        return await this._showMovieCommand.Handler(request);
    }
}