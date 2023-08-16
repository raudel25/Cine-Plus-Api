using Cine_Plus_Api.Requests;
using Microsoft.AspNetCore.Mvc;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountCommandHandler _discountCommand;

    private readonly IDiscountQueryHandler _discountQuery;

    public DiscountController(IDiscountCommandHandler discountCommand, IDiscountQueryHandler discountQuery)
    {
        this._discountCommand = discountCommand;
        this._discountQuery = discountQuery;
    }

    [HttpGet]
    public async Task<IEnumerable<Discount>> Get()
    {
        return await this._discountQuery.Handler();
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post(CreateDiscount request)
    {
        var response = await this._discountCommand.Handler(request);

        if (response.Ok) return response.Value;

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPut]
    public async Task<ActionResult> Put(UpdateDiscount request)
    {
        var response = await this._discountCommand.Handler(request);

        if (response.Ok) return Ok();
        
        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remove(int id)
    {
        await this._discountCommand.Handler(id);

        return Ok();
    }
}