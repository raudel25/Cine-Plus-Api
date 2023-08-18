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

        var token = this._securityService.Jwt(response.Value, request.Name, AccountType.User);

        return new AuthResponse(response.Value, request.Name, token, AccountType.User);
    }

    [HttpPost("user/login")]
    public async Task<ActionResult<AuthResponse>> LoginUser(LoginUser request)
    {
        var user = await this._authQuery.User(request.Email);

        if (user is null || !Password.CheckPassword(user.Password, request.Password))
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.Jwt(user.Id, user.Name, AccountType.User);

        return new AuthResponse(user.Id, user.Name, token, AccountType.User);
    }

    [HttpPost("employ/register"), Authorize]
    public async Task<ActionResult<CreateEmployResponse>> CreateEmploy([FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, AccountType.Manager);
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        return await this._authCommand.Employ();
    }

    [HttpPost("employ/login")]
    public async Task<ActionResult<AuthResponse>> LoginEmploy(LoginEmploy request)
    {
        var employ = await this._authQuery.Employ(request.User);

        if (employ is null || employ.Password != request.Password)
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.Jwt(employ.Id, "Employ", AccountType.Employ);

        return new AuthResponse(employ.Id, "Employ", token, AccountType.Employ);
    }

    [HttpDelete("employ/{id:int}"), Authorize]
    public async Task<IActionResult> DeleteEmploy(int id, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, AccountType.Admin);
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var responseEmploy = await this._authCommand.Employ(id);
        if (responseEmploy.Ok) return Ok();

        return StatusCode((int)responseEmploy.Status, new { message = responseEmploy.Message });
    }

    [HttpPost("manager/register"), Authorize]
    public async Task<ActionResult<CreateManagerResponse>> CreateManager([FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, AccountType.Admin);
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        return await this._authCommand.Manager();
    }

    [HttpPost("manager/login")]
    public async Task<ActionResult<AuthResponse>> LoginManager(LoginManager request)
    {
        var manager = await this._authQuery.Manager(request.User);

        if (manager is null || manager.Password != request.Password)
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.Jwt(manager.Id, "Manager", AccountType.Manager);

        return new AuthResponse(manager.Id, "Manager", token, AccountType.Manager);
    }

    [HttpDelete("manager/{id:int}")]
    public async Task<IActionResult> DeleteManager(int id, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, AccountType.Admin);
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var responseManager = await this._authCommand.Manager(id);
        if (responseManager.Ok) return Ok();

        return StatusCode((int)responseManager.Status, new { message = responseManager.Message });
    }

    [HttpPost("admin")]
    public ActionResult<AuthResponse> Admin(LoginAdmin request)
    {
        var auth = this._securityService.AdminCredentials(request.User, request.Password);
        if (!auth) return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.Jwt(0, "Admin", AccountType.Admin);

        return new AuthResponse(0, "Admin", token, AccountType.Admin);
    }

    [HttpGet("renew"), Authorize]
    public ActionResult<AuthResponse> Renew([FromHeader] string authorization)
    {
        var response = this._securityService.TokenToIdAccountType(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var (id, name, accountType) = response.Value;

        var token = this._securityService.Jwt(id, name, accountType);

        return new AuthResponse(id, name, token, accountType);
    }
}