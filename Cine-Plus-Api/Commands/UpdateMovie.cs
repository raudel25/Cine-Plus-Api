using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Commands;

public class UpdateMovie : CreateMovie
{
    public int Id { get; set; }

    public new Movie Movie()
    {
        var director = new Director { Name = this.Director };
        var genre = new Genre { Name = this.Director };
        var actors = Actors.Select((actor) => new Actor { Name = actor }).ToList();

        return new Movie
        {
            Id = this.Id,
            Actors = actors, CubanCine = this.CubanCine, Director = director, Genre = genre, Name = this.Name,
            Rating = this.Rating
        };
    }
}