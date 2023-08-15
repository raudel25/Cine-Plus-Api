using Cine_Plus_Api.CommandsRequest;
using Microsoft.AspNetCore.Mvc;
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
        return await this._cinemaCommand.Handler(request);
    }
    
    [HttpPut]
    public async Task<ActionResult> Put(UpdateCinema request)
    {
        await this._cinemaCommand.Handler(request);
        
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remove(int id)
    {
        await this._cinemaCommand.Handler(id);

        return Ok();
    }
}