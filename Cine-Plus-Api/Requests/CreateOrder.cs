using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateOrder
{
    public double Price { get; set; }

    public int PricePoints { get; set; }

    public int AddPoints { get; set; }

    public ICollection<Seat> Seats { get; set; } = null!;

    public Order Order() => new() { Price = this.Price, Seats = this.Seats };
}