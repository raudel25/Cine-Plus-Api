using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Responses;

public class AuthResponse
{
    public int Id { get; set; }

    public string Token { get; set; }

    public string AccountType { get; set; }

    public AuthResponse(int id, string token, string accountType)
    {
        this.Id = id;
        this.Token = token;
        this.AccountType = accountType;
    }
}