using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ReturnController : ControllerBase
{
    private readonly IReturnService _returnService;

    private readonly SecurityService _securityService;

    public ReturnController(IReturnService returnService, SecurityService securityService)
    {
        this._returnService = returnService;
        this._securityService = securityService;
    }

    [HttpPost("ReturnCreditCard")]
    public async Task<IActionResult> ReturnCreditCard(TokenRequest request)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != OrderService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var response = await this._returnService.ReturnCreditCard(responseToken.Value.Item1);
        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPost("ReturnPointsUser")]
    public async Task<IActionResult> ReturnPointsUser(TokenRequest request)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != OrderService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var response = await this._returnService.ReturnPointsUser(responseToken.Value.Item1);
        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}