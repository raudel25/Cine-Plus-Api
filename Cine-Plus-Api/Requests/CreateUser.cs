using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateUser
{
    public long IdentityCard { get; set; }
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual User User() => new() { IdentityCard=this.IdentityCard,Name = this.Name, Email = this.Email, Points = 0, Password = this.Password };
}