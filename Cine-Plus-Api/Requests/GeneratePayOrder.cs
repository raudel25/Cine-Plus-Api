namespace Cine_Plus_Api.Requests;

public class GeneratePayOrder
{
    public ICollection<GenerateSeatOrder> Seats { get; set; } = null!;
}

public class GenerateSeatOrder
{
    public int Id { get; set; }

    public ICollection<int> Discounts { get; set; } = null!;
}