using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Commands;

public class UpdateShowMovie : CreateShowMovie
{
    public int Id { get; set; }

    public new ShowMovie ShowMovie() =>
        new ShowMovie { Date = this.Date, MovieId = this.MovieId, CinemaId = this.CinemaId };
}