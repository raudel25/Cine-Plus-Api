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
public class DiscountController : ControllerBase
{
    private readonly IDiscountCommandHandler _discountCommand;

    private readonly IDiscountQueryHandler _discountQuery;

    private readonly SecurityService _securityService;

    public DiscountController(IDiscountCommandHandler discountCommand, IDiscountQueryHandler discountQuery,
        SecurityService securityService)
    {
        this._discountCommand = discountCommand;
        this._discountQuery = discountQuery;
        this._securityService = securityService;
    }

    [HttpGet,Authorize]
    public async Task<ActionResult<IEnumerable<Discount>>> Get([FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var result= await this._discountQuery.Handler();

        return result.ToList();
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<int>> Post(CreateDiscount request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._discountCommand.Handler(request);

        if (response.Ok) return response.Value;

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPut, Authorize]
    public async Task<ActionResult> Put(UpdateDiscount request, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._discountCommand.Handler(request);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("{id:int}"), Authorize]
    public async Task<ActionResult> Delete(int id, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var response = await this._discountCommand.Handler(id);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}