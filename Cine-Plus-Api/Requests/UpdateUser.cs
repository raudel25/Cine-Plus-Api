using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class UpdateUser
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;


    public User User(string email) => new User
        { Id = this.Id, Name = this.Name, Email = email, Password = this.Password };
}