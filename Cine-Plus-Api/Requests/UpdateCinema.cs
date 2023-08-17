using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class UpdateCinema : CreateCinema
{
    public int Id { get; set; }

    public new Cinema Cinema() => new() { Id = this.Id, Name = this.Name, CantSeats = this.CantSeats };
}