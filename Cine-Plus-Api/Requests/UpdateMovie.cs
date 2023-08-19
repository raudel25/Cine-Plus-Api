using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class UpdateMovie : CreateMovie
{
    public int Id { get; set; }

    public override Movie Movie()
    {
        var movie = base.Movie();
        movie.Id = this.Id;

        return movie;
    }
}