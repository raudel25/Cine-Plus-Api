using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateMovie
{
    public string Name { get; set; } = null!;

    public int Rating { get; set; }

    public long Duration { get; set; }

    public string Director { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public ICollection<string> Actors { get; set; } = null!;

    public virtual Movie Movie()
    {
        var director = new Director { Name = this.Director };
        var genre = new Genre { Name = this.Genre };
        var country = new Country { Name = this.Country };
        var actors = Actors.Select((actor) => new Actor { Name = actor }).ToList();

        return new Movie
        {
            Actors = actors, Country = country, Duration = this.Duration, Director = director, Genre = genre,
            Name = this.Name,
            Rating = this.Rating
        };
    }
}