namespace Cine_Plus_Api.Responses;

public class ResponseGeneratePayOrder
{
    public string? Token { get; set; }

    public double Price { get; private set; }

    public int PricePoints { get; private set; }

    public int AddPoints { get; private set; }

    public long Date { get; private set; }

    public ICollection<ResponseSeatOrder> Seats { get; set; } = null!;

    public ResponseGeneratePayOrder(double price, int pricePoints, int addPoints, ICollection<ResponseSeatOrder> seats,
        long date)
    {
        this.Price = price;
        this.PricePoints = pricePoints;
        this.AddPoints = addPoints;
        this.Date = date;
        this.Seats = seats;
    }
}

public class ResponseSeatOrder
{
    public int Id { get; set; }

    public string Message { get; set; }

    public ResponseSeatOrder(int id, string message)
    {
        this.Id = id;
        this.Message = message;
    }
}