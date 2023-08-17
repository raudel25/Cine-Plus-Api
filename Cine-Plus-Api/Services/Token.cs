using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public class Token
{
    private readonly IConfiguration _configuration;

    public Token(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public string Jwt(int id, AccountType accountType)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
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

    public ApiResponse<(int, AccountType)> TokenToIdAccountType(string authHeader)
    {
        if (!authHeader.StartsWith("Bearer "))
            return new ApiResponse<(int, AccountType)>(HttpStatusCode.Unauthorized, "Unauthorized");

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
        //TODO:Verificar tipo
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");

        if (idClaim is null || roleClaim is null)
            return new ApiResponse<(int, AccountType)>(HttpStatusCode.Unauthorized, "Unauthorized");

        try
        {
            return new ApiResponse<(int, AccountType)>((int.Parse(idClaim.Value),
                AccountTypeMethods.ToAccountType(roleClaim.Value)));
        }
        catch (Exception _)
        {
            return new ApiResponse<(int, AccountType)>(HttpStatusCode.Unauthorized, "Unauthorized");
        }
    }
}