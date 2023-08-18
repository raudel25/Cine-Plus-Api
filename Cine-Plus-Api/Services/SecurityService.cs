using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public class SecurityService
{
    private readonly IConfiguration _configuration;

    public SecurityService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public string Jwt(int id, string name, AccountType accountType)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, AccountTypeMethods.ToString(accountType))
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }

    public ApiResponse<(int, string, AccountType)> TokenToIdAccountType(string authHeader)
    {
        if (!authHeader.StartsWith("Bearer "))
            return new ApiResponse<(int, string, AccountType)>(HttpStatusCode.Unauthorized, "Unauthorized");

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");

        if (idClaim is null || nameClaim is null || roleClaim is null)
            return new ApiResponse<(int, string, AccountType)>(HttpStatusCode.Unauthorized, "Unauthorized");

        try
        {
            return new ApiResponse<(int, string, AccountType)>((int.Parse(idClaim.Value), roleClaim.Value,
                AccountTypeMethods.ToAccountType(roleClaim.Value)));
        }
        catch
        {
            return new ApiResponse<(int, string, AccountType)>(HttpStatusCode.Unauthorized, "Unauthorized");
        }
    }

    public bool AdminCredentials(string user, string password)
    {
        var userSystem = _configuration["Admin:User"];
        var passwordSystem = _configuration["Admin:Password"];

        return user == userSystem && password == passwordSystem;
    }

    public ApiResponse Authorize(string authorization, AccountType accountType)
    {
        var responseSecurity = TokenToIdAccountType(authorization);
        if (!responseSecurity.Ok)
            return responseSecurity.ConvertApiResponse();

        var accountTypeCurrent = responseSecurity.Value.Item3;
        return !AccountTypeMethods.Authorize(accountTypeCurrent, accountType)
            ? new ApiResponse(HttpStatusCode.Unauthorized, "Unauthorized")
            : new ApiResponse();
    }
}