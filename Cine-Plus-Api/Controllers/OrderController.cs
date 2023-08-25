using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    private readonly SecurityService _securityService;

    public OrderController(SecurityService securityService,
        IOrderService orderService)
    {
        this._securityService = securityService;
        this._orderService = orderService;
    }

    [HttpPost]
    public async Task<ResponseGeneratePayOrder> Generate(GeneratePayOrder request)
    {
        return await this._orderService.GenerateOrderWeb(request);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(CancelOrder request)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != OrderService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var response = await this._orderService.CancelOrder(responseToken.Value.Item1);
        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}