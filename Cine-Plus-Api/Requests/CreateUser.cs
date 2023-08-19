using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateUser
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual User User() => new() { Name = this.Name, Email = this.Email, Password = this.Password };
}