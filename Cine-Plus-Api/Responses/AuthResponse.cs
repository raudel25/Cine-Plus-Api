namespace Cine_Plus_Api.Responses;

public class AuthResponse
{
    public int Id { get; set; }

    public string Token { get; set; }

    public string User { get; set; }

    public string AccountType { get; set; }

    public AuthResponse(int id, string user, string token, Helpers.AccountType accountType)
    {
        this.Id = id;
        this.Token = token;
        this.AccountType = Helpers.AccountTypeMethods.ToString(accountType);
        this.User = user;
    }
}