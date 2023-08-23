using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public static class MyClaims
{
    public const string Id = "id";
    public const string Name = "name";
    public const string Role = "role";
    public const string Date = "date";
    public const string TypePayment = "type_payment";
}

public class SecurityService
{
    private readonly IConfiguration _configuration;

    public SecurityService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public string JwtAuth(int id, string name, Account account)
    {
        var claims = new[]
        {
            new Claim(MyClaims.Id, id.ToString()),
            new Claim(MyClaims.Name, name),
            new Claim(MyClaims.Role, account.ToString())
        };

        var expires = DateTime.UtcNow.AddHours(2);

        return Jwt(expires, claims);
    }

    public string JwtPay(int id, string type, DateTime expires)
    {
        var now = DateTime.UtcNow;
        var claims = new[]
        {
            new Claim(MyClaims.Id, id.ToString()),
            new Claim(MyClaims.TypePayment, type),
            new Claim(MyClaims.Date, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString())
        };

        return Jwt(expires, claims);
    }

    private string Jwt(DateTime expires, IEnumerable<Claim> claims)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
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

    public ApiResponse<(int, string, Account)> DecodingAuth(string authHeader)
    {
        var response = DecodingToken(authHeader);
        if (!response.Ok) return response.ConvertApiResponse<(int, string, Account)>();

        var jwtToken = response.Value!;

        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == MyClaims.Id);
        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == MyClaims.Name);
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == MyClaims.Role);

        if (idClaim is null || nameClaim is null || roleClaim is null)
            return new ApiResponse<(int, string, Account)>(HttpStatusCode.Unauthorized, "Unauthorized");

        try
        {
            return new ApiResponse<(int, string, Account)>((int.Parse(idClaim.Value), roleClaim.Value,
                Account.StringToAccount(roleClaim.Value)));
        }
        catch
        {
            return new ApiResponse<(int, string, Account)>(HttpStatusCode.Unauthorized, "Unauthorized");
        }
    }

    public ApiResponse<(int, string, long)> DecodingPay(string authHeader)
    {
        var response = DecodingToken(authHeader);
        if (!response.Ok) return response.ConvertApiResponse<(int, string, long)>();

        var jwtToken = response.Value!;

        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == MyClaims.Id);
        var dateClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == MyClaims.Date);
        var typeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == MyClaims.TypePayment);

        if (idClaim is null || dateClaim is null || typeClaim is null)
            return new ApiResponse<(int, string, long)>(HttpStatusCode.Unauthorized, "Unauthorized");

        return new ApiResponse<(int, string, long)>((int.Parse(idClaim.Value),typeClaim.Value,
            long.Parse(dateClaim.Value)));
    }

    private ApiResponse<JwtSecurityToken> DecodingToken(string authHeader)
    {
        if (!authHeader.StartsWith("Bearer "))
            return new ApiResponse<JwtSecurityToken>(HttpStatusCode.Unauthorized, "Unauthorized");

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var handler = new JwtSecurityTokenHandler();
        return new ApiResponse<JwtSecurityToken>(handler.ReadJwtToken(token));
    }

    public bool AdminCredentials(string user, string password)
    {
        var userSystem = _configuration["Admin:User"];
        var passwordSystem = _configuration["Admin:Password"];

        return user == userSystem && password == passwordSystem;
    }

    public ApiResponse Authorize(string authorization, Account account)
    {
        var responseSecurity = DecodingAuth(authorization);
        if (!responseSecurity.Ok)
            return responseSecurity.ConvertApiResponse();

        var accountCurrent = responseSecurity.Value.Item3;
        return accountCurrent.Level() < account.Level()
            ? new ApiResponse(HttpStatusCode.Unauthorized, "Unauthorized")
            : new ApiResponse();
    }
}