namespace Cine_Plus_Api.Responses;

public class AuthResponse
{
    public int Id { get; set; }

    public string Token { get; set; }

    public Helpers.AccountType AccountType { get; set; }

    public AuthResponse(int id, string token, Helpers.AccountType accountType)
    {
        this.Id = id;
        this.Token = token;
        this.AccountType = accountType;
    }
}