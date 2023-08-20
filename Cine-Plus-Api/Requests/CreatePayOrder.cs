using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class CreatePayOrder
{
    public double Price { get; set; }

    public bool Paid { get; set; }

    public ICollection<PaidSeat> PaidSeats { get; set; } = null!;

    public PayOrder PayOrder() => new() { Price = this.Price, PaidSeats = this.PaidSeats, Paid = this.Paid };
}