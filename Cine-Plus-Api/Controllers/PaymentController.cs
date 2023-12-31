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

    [HttpPost("PayCreditCard")]
    public async Task<ActionResult<IEnumerable<ResponsePaidSeat>>> PayCreditCard(PayCreditCard request)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != OrderService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var response = await this._paymentService.PayCreditCard(responseToken.Value.Item1, request);
        if (response.Ok) return response.Value!.ToList();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPost("PayCreditCardUser"), Authorize]
    public async Task<ActionResult<IEnumerable<ResponsePaidSeat>>> PayCreditCardUser(PayCreditCard request,
        [FromHeader] string authorization)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != OrderService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var responseAuth = this._securityService.DecodingAuth(authorization);
        if (!responseAuth.Ok) return StatusCode((int)responseAuth.Status, new { message = responseAuth.Message });

        var (id, _, account) = responseAuth.Value;
        if (account is not UserAccount) return BadRequest(new { message = "Unauthorized" });

        var response = await this._paymentService.PayCreditCardWithUser(responseToken.Value.Item1, id, request);
        if (response.Ok) return response.Value!.ToList();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpPost("PayPointsUser"), Authorize]
    public async Task<ActionResult<IEnumerable<ResponsePaidSeat>>> PayPointsUser(PayPoints request,
        [FromHeader] string authorization)
    {
        if (!this._securityService.ValidateToken(request.Token)) return BadRequest(new { message = "Invalid token" });

        var responseToken = this._securityService.DecodingPay(request.Token);
        if (!responseToken.Ok || responseToken.Value.Item2 != OrderService.Payment)
            return BadRequest(new { message = "Invalid token" });

        var responseAuth = this._securityService.DecodingAuth(authorization);
        if (!responseAuth.Ok) return StatusCode((int)responseAuth.Status, new { message = responseAuth.Message });

        var (id, _, account) = responseAuth.Value;
        if (account is not UserAccount) return BadRequest(new { message = "Unauthorized" });

        var response = await this._paymentService.PayPointsUser(responseToken.Value.Item1, request, id);
        if (response.Ok) return response.Value!.ToList();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}