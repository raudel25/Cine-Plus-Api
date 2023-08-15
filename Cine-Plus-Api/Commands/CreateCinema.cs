using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Commands;

public class CreateCinema
{
    public string Name { get; set; } = null!;

    public int CantSeats { get; set; }

    public Cinema Cinema() => new Cinema { Name = this.Name, CantSeats = this.CantSeats };
}