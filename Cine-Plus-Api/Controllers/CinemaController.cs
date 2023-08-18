using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Helpers;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : ControllerBase
{
    private readonly ICinemaCommandHandler _cinemaCommand;

    private readonly ICinemaQueryHandler _cinemaQuery;

    private readonly SecurityService _securityService;

    public CinemaController(ICinemaCommandHandler cinemaCommand, ICinemaQueryHandler cinemaQuery,
        SecurityService securityService)
    {
        this._cinemaCommand = cinemaCommand;
        this._cinemaQuery = cinemaQuery;
        this._securityService = securityService;
    }

    [HttpGet]
    public async Task<IEnumerable<Cinema>> Get()
    {
        return await this._cinemaQuery.Handler();
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<int>> Post(CreateCinema request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._cinemaCommand.Handler(request);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPut, Authorize]
    public async Task<ActionResult> Put(UpdateCinema request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._cinemaCommand.Handler(request);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("{id:int}"), Authorize]
    public async Task<ActionResult> Remove(int id, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._cinemaCommand.Handler(id);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}