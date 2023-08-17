using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateEmploy
{
    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Employ Employ() => new Employ { Name = this.Name, Password = this.Password };
}