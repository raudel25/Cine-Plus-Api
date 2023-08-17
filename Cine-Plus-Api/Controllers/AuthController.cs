using Microsoft.AspNetCore.Mvc;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Cine_Plus_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthCommandHandler _authCommand;

    private readonly IAuthQueryHandler _authQuery;

    private readonly Token _token;

    public AuthController(IAuthCommandHandler authCommand, Token token, IAuthQueryHandler authQuery)
    {
        this._authCommand = authCommand;
        this._token = token;
        this._authQuery = authQuery;
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponse>> CreateUser(CreateUser request)
    {
        request.Password = Password.EncryptPassword(request.Password);

        var response = await this._authCommand.User(request);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var token = this._token.Jwt(response.Value, AccountType.User);

        return new AuthResponse(response.Value, token, AccountType.User);
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponse>> LoginUser(LoginUser request)
    {
        var user = await this._authQuery.User(request.Email);

        if (user is null || Password.CheckPassword(request.Password, user.Password))
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._token.Jwt(user.Id, AccountType.User);

        return new AuthResponse(user.Id, token, AccountType.User);
    }

    [HttpPost("employ"), Authorize]
    public async Task<ActionResult<CreateEmployResponse>> PostEmploy([FromHeader] string authorization)
    {
        var response = this._token.TokenToIdAccountType(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var accountType = response.Value.Item2;
        if (!AccountTypeMethods.Authorice(accountType, AccountType.Manager))
            return Unauthorized(new { message = "Unauthorized" });

        return await this._authCommand.Employ();
    }

    [HttpPost("manager"), Authorize]
    public async Task<ActionResult<CreateManagerResponse>> PostManager([FromHeader] string authorization)
    {
        var response = this._token.TokenToIdAccountType(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var accountType = response.Value.Item2;
        if (!AccountTypeMethods.Authorice(accountType, AccountType.Admin))
            return Unauthorized(new { message = "Unauthorized" });

        return await this._authCommand.Manager();
    }

    [HttpDelete("employ/{id:int}")]
    public async Task<IActionResult> DeleteEmploy(int id)
    {
        var response = await this._authCommand.Employ(id);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("manager/{id:int}")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        var response = await this._authCommand.Manager(id);

        if (response.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}