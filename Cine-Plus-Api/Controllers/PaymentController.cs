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

    public PaymentController(IPaymentService paymentService)
    {
        this._paymentService = paymentService;
    }

    [HttpPost, Authorize]
    public async Task<ResponseGeneratePayOrder> Post(GeneratePayOrder request)
    {
        return await this._paymentService.GeneratePayOrder(request);
    }

    [HttpDelete("{id:int}"), Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await this._paymentService.CancelPayOrder(id);
        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}