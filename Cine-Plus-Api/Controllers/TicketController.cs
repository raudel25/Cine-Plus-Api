using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TicketController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    private readonly SecurityService _securityService;

    private readonly IAuthQueryHandler _authQuery;

    public TicketController(IPaymentService paymentService, SecurityService securityService,
        IAuthQueryHandler authQuery)
    {
        this._paymentService = paymentService;
        this._securityService = securityService;
        this._authQuery = authQuery;
    }

    [HttpPost("employ/login")]
    public async Task<ActionResult<AuthResponse>> LoginEmploy(Login request)
    {
        var employ = await this._authQuery.Employ(request.User);

        if (employ is null || employ.Password != request.Password)
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.JwtAuth(employ.Id, "Employ", new EmployAccount());

        return new AuthResponse(employ.Id, "Employ", token, new EmployAccount());
    }

    [HttpPost("PayTicket"), Authorize]
    public async Task<ActionResult<ResponseGeneratePayOrder>> PayTicket(TicketRequest request,
        [FromHeader] string authorization)
    {
        var response = this._securityService.DecodingAuth(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var (id, _, account) = response.Value;
        if (account is not EmployAccount) return BadRequest(new { message = "Unauthorized" });

        var responsePay = await this._paymentService.PayTicket(request.Order, id);
        if (responsePay.Ok) return StatusCode((int)responsePay.Status, new { message = responsePay.Message });

        return responsePay.Value!;
    }

    [HttpPost("PayTicketUser"), Authorize]
    public async Task<ActionResult<ResponseGeneratePayOrder>> PayTicketUser(TicketRequestUser request,
        [FromHeader] string authorization)
    {
        var response = this._securityService.DecodingAuth(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var (id, _, account) = response.Value;
        if (account is not EmployAccount) return BadRequest(new { message = "Unauthorized" });

        var responsePay = await this._paymentService.PayTicketWithUser(request.Order, id, request.Id);
        if (responsePay.Ok) return StatusCode((int)responsePay.Status, new { message = responsePay.Message });

        return responsePay.Value!;
    }

    [HttpPost("PayTicketPointsUser"), Authorize]
    public async Task<ActionResult<ResponseGeneratePayOrder>> PayTicketPointsUser(TicketRequestUser request,
        [FromHeader] string authorization)
    {
        var response = this._securityService.DecodingAuth(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var (id, _, account) = response.Value;
        if (account is not EmployAccount) return BadRequest(new { message = "Unauthorized" });

        var responsePay = await this._paymentService.PayTicketPointsUser(request.Order, id, request.Id);
        if (responsePay.Ok) return StatusCode((int)responsePay.Status, new { message = responsePay.Message });

        return responsePay.Value!;
    }
}