using Cine_Plus_Api.Helpers;

namespace Cine_Plus_Api.Responses;

public class AuthResponse
{
    public int Id { get; set; }

    public string Token { get; set; }

    public string User { get; set; }

    public string Account { get; set; }

    public AuthResponse(int id, string user, string token, Account account)
    {
        this.Id = id;
        this.Token = token;
        this.Account = account.ToString();
        this.User = user;
    }
}