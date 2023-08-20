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

        var token = this._securityService.JwtAuth(response.Value, request.Name, new UserAccount());

        return new AuthResponse(response.Value, request.Name, token, new UserAccount());
    }

    [HttpPut("user/update"), Authorize]
    public async Task<ActionResult<AuthResponse>> UpdateUser([FromHeader] string authorization, UpdateUser request)
    {
        var responseAuth = this._securityService.DecodingAuth(authorization);
        if (!responseAuth.Ok) return StatusCode((int)responseAuth.Status, new { message = responseAuth.Message });

        if (responseAuth.Value.Item1 != request.Id) return Unauthorized(new { message = "Unauthorized" });

        var response = await this._authCommand.User(request);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        return Ok();
    }

    [HttpPost("user/login")]
    public async Task<ActionResult<AuthResponse>> LoginUser(Login request)
    {
        var user = await this._authQuery.User(request.User);

        if (user is null || !Password.CheckPassword(user.Password, request.Password))
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.JwtAuth(user.Id, user.Name, new UserAccount());

        return new AuthResponse(user.Id, user.Name, token, new UserAccount());
    }

    [HttpPost("employ/register"), Authorize]
    public async Task<ActionResult<CreateEmployResponse>> CreateEmploy([FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        return await this._authCommand.Employ();
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

    [HttpDelete("employ/{id:int}"), Authorize]
    public async Task<IActionResult> DeleteEmploy(int id, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new ManagerAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var responseEmploy = await this._authCommand.Employ(id);
        if (responseEmploy.Ok) return Ok();

        return StatusCode((int)responseEmploy.Status, new { message = responseEmploy.Message });
    }

    [HttpPost("manager/register"), Authorize]
    public async Task<ActionResult<CreateManagerResponse>> CreateManager([FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new AdminAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        return await this._authCommand.Manager();
    }

    [HttpPost("manager/login")]
    public async Task<ActionResult<AuthResponse>> LoginManager(Login request)
    {
        var manager = await this._authQuery.Manager(request.User);

        if (manager is null || manager.Password != request.Password)
            return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.JwtAuth(manager.Id, "Manager", new ManagerAccount());

        return new AuthResponse(manager.Id, "Manager", token, new ManagerAccount());
    }

    [HttpDelete("manager/{id:int}")]
    public async Task<IActionResult> DeleteManager(int id, [FromHeader] string authorization)
    {
        var responseSecurity = this._securityService.Authorize(authorization, new AdminAccount());
        if (!responseSecurity.Ok)
            return StatusCode((int)responseSecurity.Status, new { message = responseSecurity.Message });

        var responseManager = await this._authCommand.Manager(id);
        if (responseManager.Ok) return Ok();

        return StatusCode((int)responseManager.Status, new { message = responseManager.Message });
    }

    [HttpPost("admin")]
    public ActionResult<AuthResponse> Admin(Login request)
    {
        var auth = this._securityService.AdminCredentials(request.User, request.Password);
        if (!auth) return BadRequest(new { message = "Incorrect email or password" });

        var token = this._securityService.JwtAuth(0, "Admin", new AdminAccount());

        return new AuthResponse(0, "Admin", token, new AdminAccount());
    }

    [HttpGet("renew"), Authorize]
    public ActionResult<AuthResponse> Renew([FromHeader] string authorization)
    {
        var response = this._securityService.DecodingAuth(authorization);
        if (!response.Ok) return StatusCode((int)response.Status, new { message = response.Message });

        var (id, name, accountType) = response.Value;

        var token = this._securityService.JwtAuth(id, name, accountType);

        return new AuthResponse(id, name, token, accountType);
    }
}