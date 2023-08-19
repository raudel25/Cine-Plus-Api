using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateCinema
{
    public string Name { get; set; } = null!;

    public int CantSeats { get; set; }

    public virtual Cinema Cinema() => new() { Name = this.Name, CantSeats = this.CantSeats };
}