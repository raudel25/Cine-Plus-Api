using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreateSeat
{
    public ShowMovie ShowMovie { get; set; } = null!;

    public double Price { get; set; }

    public int PricePoints { get; set; }

    public int AddPoints { get; set; }

    public Seat Seat(int number) => new Seat
    {
        ShowMovie = this.ShowMovie, Price = this.Price, PricePoints = this.PricePoints, AddPoints = this.AddPoints,
        Number = number, State = SeatState.Available
    };
}