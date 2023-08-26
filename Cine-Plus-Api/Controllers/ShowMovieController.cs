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

    private readonly ISeatQueryHandler _seatQuery;

    private readonly ISeatCommandHandler _seatCommand;


    public ShowMovieController(IShowMovieCommandHandler showMovieCommand, IShowMovieQueryHandler showMovieQuery,
        SecurityService securityService, ISeatCommandHandler seatCommand, ISeatQueryHandler seatQuery)
    {
        this._showMovieCommand = showMovieCommand;
        this._showMovieQuery = showMovieQuery;
        this._securityService = securityService;
        this._seatQuery = seatQuery;
        this._seatCommand = seatCommand;
    }

    [HttpGet, Authorize]
    public async Task<ActionResult<IEnumerable<ShowMovie>>> Get([FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var result = await this._showMovieQuery.Handler();

        return result.ToList();
    }

    [HttpGet("{showMovieId:int}")]
    public async Task<IEnumerable<Seat>> Get(int showMovieId)
    {
        await this._seatCommand.UpdateSeats();
        return await this._seatQuery.AvailableSeat(showMovieId);
    }

    [HttpGet("Available")]
    public async Task<IEnumerable<ShowMovie>> GetAvailable()
    {
        return await this._showMovieQuery.AvailableShowMovie();
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<int>> Post(CreateShowMovie request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._showMovieCommand.Handler(request);

        if (response.Ok) return response.Value;

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}