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

    private readonly SecurityService _securityService;

    public AuthController(IAuthCommandHandler authCommand, SecurityService securityService, IAuthQueryHandler authQuery)
    {
        this._authCommand = authCommand;
        this._securityService = securityService;
        this._authQuery = authQuery;
    }

    [HttpPost("user/register")]
    public async Task<ActionResult<AuthResponse>> CreateUser(CreateUser request)
    {
        request.Password = Password.EncryptPassword(request.Password);

        var response = await this._authCommand.User(request);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var token = this._securityService.Jwt(response.Value, AccountType.User);

        return new AuthResponse(response.Value, token, AccountType.User);
    }

    [HttpPost("user/login")]
    public async Task<ActionResult<AuthResponse>> LoginUser(LoginUser request)
    {
        var user = await this._authQuery.User(request.Email);

        if (user is null || !Password.CheckPassword(request.Password, user.Password))
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.Jwt(user.Id, AccountType.User);

        return new AuthResponse(user.Id, token, AccountType.User);
    }

    [HttpPost("employ/login")]
    public async Task<ActionResult<AuthResponse>> LoginEmploy(LoginEmploy request)
    {
        var employ = await this._authQuery.Employ(request.User);

        if (employ is null || employ.Password != request.Password)
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.Jwt(employ.Id, AccountType.Employ);

        return new AuthResponse(employ.Id, token, AccountType.Employ);
    }

    [HttpPost("manager/login")]
    public async Task<ActionResult<AuthResponse>> LoginManager(LoginManager request)
    {
        var manager = await this._authQuery.Manager(request.User);

        if (manager is null || manager.Password != request.Password)
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.Jwt(manager.Id, AccountType.Manager);

        return new AuthResponse(manager.Id, token, AccountType.Manager);
    }

    [HttpPost("employ/register"), Authorize]
    public async Task<ActionResult<CreateEmployResponse>> CreateEmploy([FromHeader] string authorization)
    {
        var response = this._securityService.TokenToIdAccountType(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var accountType = response.Value.Item2;
        if (!AccountTypeMethods.Authorice(accountType, AccountType.Manager))
            return Unauthorized(new { message = "Unauthorized" });

        return await this._authCommand.Employ();
    }

    [HttpPost("manager/register"), Authorize]
    public async Task<ActionResult<CreateManagerResponse>> CreateManager([FromHeader] string authorization)
    {
        var response = this._securityService.TokenToIdAccountType(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var accountType = response.Value.Item2;
        if (!AccountTypeMethods.Authorice(accountType, AccountType.Admin))
            return Unauthorized(new { message = "Unauthorized" });

        return await this._authCommand.Manager();
    }

    [HttpDelete("employ/{id:int}"),Authorize]
    public async Task<IActionResult> DeleteEmploy(int id,[FromHeader] string authorization)
    {
        var response = this._securityService.TokenToIdAccountType(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var accountType = response.Value.Item2;
        if (!AccountTypeMethods.Authorice(accountType, AccountType.Manager))
            return Unauthorized(new { message = "Unauthorized" });

        var responseEmploy = await this._authCommand.Employ(id);
        if (responseEmploy.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }

    [HttpDelete("manager/{id:int}")]
    public async Task<IActionResult> DeleteManager(int id,[FromHeader] string authorization)
    {
        var response = this._securityService.TokenToIdAccountType(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var accountType = response.Value.Item2;
        if (!AccountTypeMethods.Authorice(accountType, AccountType.Admin))
            return Unauthorized(new { message = "Unauthorized" });
        
        var responseManager = await this._authCommand.Manager(id);
        if (responseManager.Ok) return Ok();

        return StatusCode((int)response.Status, new { message = response.Message });
    }
}