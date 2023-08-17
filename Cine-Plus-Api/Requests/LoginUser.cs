namespace Cine_Plus_Api.Requests;

public class LoginUser
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}