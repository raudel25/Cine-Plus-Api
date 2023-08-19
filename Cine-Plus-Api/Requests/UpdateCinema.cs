using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class UpdateCinema : CreateCinema
{
    public int Id { get; set; }

    public override Cinema Cinema()
    {
        var cinema = base.Cinema();
        cinema.Id = this.Id;

        return cinema;
    }
}