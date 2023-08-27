namespace Cine_Plus_Api.Responses;

public class CreateSystemUserResponse
{
    public string User { get; private set; }

    public string Password { get; private set; }

    public CreateSystemUserResponse(string user, string password)
    {
        this.User = user;
        this.Password = password;
    }
}