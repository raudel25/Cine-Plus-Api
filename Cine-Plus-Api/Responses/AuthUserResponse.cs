using Cine_Plus_Api.Helpers;

namespace Cine_Plus_Api.Responses;

public class AuthUserResponse : AuthResponse
{
    public int Points { get; private set; }

    public AuthUserResponse(int id, string user, string token, Account account, int points) : base(id, user, token,
        account)
    {
        this.Points = points;
    }
}