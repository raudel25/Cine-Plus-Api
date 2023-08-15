using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.CommandsRequest;

public class UpdateCinema : CreateCinema
{
    public int Id { get; set; }

    public new Cinema Cinema() => new Cinema { Id = this.Id, Name = this.Name, CantSeats = this.CantSeats };
}