using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    private readonly SecurityService _securityService;

    public PaymentController(IPaymentService paymentService, SecurityService securityService)
    {
        this._paymentService = paymentService;
        this._securityService = securityService;
    }

    [HttpPost("Generate")]
    public async Task<ResponseGeneratePayOrder> Generate(GeneratePayOrder request)
    {
        return await this._paymentService.GenerateOrder(request);
    }

    [HttpPost("PayCreditCard")]
    public async Task<ActionResult<IEnumerable<ResponsePaidSeat>>> PayCreditCard(PayCreditCard request)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != PaymentService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var response = await this._paymentService.PayCreditCard(responseToken.Value.Item1, request);
        if (response.Ok) return response.Value!.ToList();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<ResponseGeneratePayOrder>> PayTicket(GeneratePayOrder request,
        [FromHeader] string authorization)
    {
        var response = this._securityService.DecodingAuth(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var (id, _, account) = response.Value;
        if (account is not EmployAccount) return BadRequest(new { message = "Unauthorized" });

        return await this._paymentService.PayTicket(request, id);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(CancelOrder request)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != PaymentService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var response = await this._paymentService.CancelPayOrder(responseToken.Value.Item1);
        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}