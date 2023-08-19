namespace Cine_Plus_Api.Requests;

public class CreatePayOrder
{
    public ICollection<SeatOrder> Seats { get; set; } = null!;
}

public class SeatOrder
{
    public int Id { get; set; }

    public ICollection<int> Discounts { get; set; } = null!;
}