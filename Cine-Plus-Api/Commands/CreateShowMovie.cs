using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Commands;

public class CreateShowMovie
{
    public long Date { get; set; }

    public int MovieId { get; set; }

    public int CinemaId { get; set; }

    public ShowMovie ShowMovie() =>
        new ShowMovie { Date = this.Date, MovieId = this.MovieId, CinemaId = this.CinemaId };
}