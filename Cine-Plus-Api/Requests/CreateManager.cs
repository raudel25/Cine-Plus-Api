using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateManager : CreateEmploy
{
    public Manager Manager() => new Manager { Name = this.Name, Password = this.Password };
}