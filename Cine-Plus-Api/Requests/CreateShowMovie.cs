using System.Runtime.InteropServices.JavaScript;
using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateShowMovie
{
    public long Date { get; set; }

    public int MovieId { get; set; }

    public int CinemaId { get; set; }

    public double Price { get; set; }

    public int PricePoints { get; set; }

    public int AddPoints { get; set; }

    public ICollection<int> Discounts { get; set; } = null!;

    public virtual ShowMovie ShowMovie() =>
        new()
        {
            Date = this.Date, MovieId = this.MovieId, CinemaId = this.CinemaId
        };
}